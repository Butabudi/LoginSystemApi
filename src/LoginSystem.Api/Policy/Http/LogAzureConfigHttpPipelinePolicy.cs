using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Serilog;

namespace LoginSystem.Api.Policy.Http;

/// <summary>
/// HTTP Pipeline policy to log whether the Azure App Configuration refresh was successful or not
/// This is to allow us to know if the application is using the configured flags or falling back to the appsettings values
/// </summary>
public class LogAzureConfigHttpPipelinePolicy(bool required) : HttpPipelinePolicy
{
    public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        // Continue the pipeline execution (this will eventually end up with the HTTP request being sent)
        // Once populated with the response, we can log whether the request was successful or not
        // If an exception is thrown, we catch it and log it as a failure before re-throwing it
        try
        {
            await ProcessNextAsync(message, pipeline);
        }
        catch (Exception ex) when (ex is AggregateException or RequestFailedException or HttpRequestException)
        {
            LogException(ex);
            throw;
        }

        LogMessage(message);
    }

    public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        // See ProcessAsync above for explanation
        try
        {
            ProcessNext(message, pipeline);
        }
        catch (Exception ex) when (ex is AggregateException or RequestFailedException or HttpRequestException)
        {
            LogException(ex);
            throw;
        }

        LogMessage(message);
    }

    private void LogMessage(HttpMessage message)
    {
        // Note that this uses the static Log class rather than ILogger because it is not easily accessible here.

        // Log a warning if we don't have a response or if the response is an error
        // Otherwise, log an information message to indicate that the Azure App Configuration was retrieved successfully
        if (!message.HasResponse || message.Response.IsError)
        {
            Log.Warning("Azure App Configuration refresh failed. Falling back: {FallingBackToAppSettings}", required);
        }
        else
        {
            Log.Information("Azure App Configuration retrieved successfully");
        }
    }

    private void LogException(Exception exception)
    {
        Log.Error(exception, "An exception occurred retrieving app configuration. Falling back: {FallingBackToAppSettings}", required);
    }
}
