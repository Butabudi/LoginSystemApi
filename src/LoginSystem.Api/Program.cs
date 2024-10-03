using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CorrelationId;
using CorrelationId.DependencyInjection;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Azure.Core;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using LoginSystem.Api.Policy.Http;

using LoginSystem.Api.ApplicationInsights;
using LoginSystem.Api.Extensions;
using LoginSystem.Api.OperationFilters;
using LoginSystem.Api.Options;
using LoginSystem.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var executingAssembly = typeof(Program).Assembly;

//-- Add options
builder.Services.AddOptions<EnvironmentOptions>()
    .Bind(builder.Configuration.GetSection("EnvironmentOptions"))
    .ValidateDataAnnotations();

//-- Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();

builder.Services.AddApplicationInsightsTelemetry();

var azureAppConfigurationConnectionString =
    builder.Configuration.GetConnectionString("AzureAppConfigurationConnection");
var isAppConfigurationEnabled = !string.IsNullOrEmpty(azureAppConfigurationConnectionString);

// If the connection string for App Configuration resource is provided, then we connect to it. Otherwise, we use local values
if (isAppConfigurationEnabled)
{
    var environmentName = builder.Configuration.GetValue<string>("EnvironmentOptions:Name");
    var cacheSeconds = builder.Configuration.GetValue("AzureAppConfiguration:CacheSeconds", 30);
    var keyPrefix = builder.Configuration.GetValue<string>("AzureAppConfiguration:KeyPrefix", KeyFilter.Any);
    var appConfigOptional = builder.Configuration.GetValue("AzureAppConfiguration:Optional", false);

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options
            .Connect(azureAppConfigurationConnectionString)
            .UseFeatureFlags(featureFlagOptions =>
            {
                // It's important to keep this order: first the expiration time, then the feature flag selection if a label is specified
                featureFlagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(cacheSeconds);
                featureFlagOptions.Select(keyPrefix, environmentName);
            })
            .ConfigureClientOptions(clientOptions =>
            {
                clientOptions.AddPolicy(new LogAzureConfigHttpPipelinePolicy(appConfigOptional), HttpPipelinePosition.PerCall);
            })
            .ConfigureStartupOptions(startupOptions =>
            {
                startupOptions.Timeout = TimeSpan.FromSeconds(15);
            });
    }, optional: appConfigOptional);

    //-- Add Azure App Configuration for managing the feature flags
    builder.Services.AddAzureAppConfiguration();
}

builder.Services.AddFeatureManagement();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var swaggerClientSettings = builder.Configuration.GetSection("SwaggerClientSettings").Get<SwaggerClientSettings>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LoginSystem.Api", Version = "v1" });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{executingAssembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Remove 'Dto' suffix from models
    c.CustomSchemaIds(type => Regex.Replace(type.Name, "^(.*?)Dto$", "$1"));

    // Include a required header on all requests - X-Correlation-Id
    c.OperationFilter<CorrelationIdHeaderOperationFilter>();

    if(swaggerClientSettings != null)
    {
        var scopes = new Dictionary<string, string>
        {
            { swaggerClientSettings.Scope, "Access application on user behalf" }
        };

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow()
                {
                    AuthorizationUrl = new Uri(swaggerClientSettings.AuthorizationUrl),
                    TokenUrl = new Uri(swaggerClientSettings.TokenUrl),
                    Scopes = scopes
                }
            }
        });
    }
});


// Add DB Context
builder.Services.AddDbContext<LoginSystemDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR, AutoMapper, FluentValidation
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(executingAssembly));
builder.Services.AddAutoMapper(executingAssembly);
builder.Services.AddValidatorsFromAssembly(executingAssembly);

builder.Services.AddProblemDetails();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDefaultCorrelationId(opt => opt.AddToLoggingScope = true);

//-- Add Logger/Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

//-- Build the app.
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

//-- Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    TelemetryDebugWriter.IsTracingDisabled = true;
}

app.UseCorrelationId();

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";

    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => {
        var swaggerPath = builder.Configuration.GetSection("Swagger:Path").Value;
        var protocol = httpReq.GetProtocol();

        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = $"{protocol}://{httpReq.Host.Value}{swaggerPath}" }
        };
    });
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "LoginSystem.Api v1");

    options.OAuthAppName("Swagger Client");
    options.OAuthClientId(swaggerClientSettings.ClientId);
    options.OAuthClientSecret(swaggerClientSettings.Secret);
    options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (isAppConfigurationEnabled)
{
    app.UseAzureAppConfiguration();
}

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
