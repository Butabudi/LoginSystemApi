namespace LoginSystem.Api.Options;

public class SwaggerClientSettings
{
    public required string ClientId { get; set; }

    public required string Secret { get; set; }

    public required string Scope { get; set; }

    public required string AuthorizationUrl { get; set; }

    public required string TokenUrl { get; set; }
}

