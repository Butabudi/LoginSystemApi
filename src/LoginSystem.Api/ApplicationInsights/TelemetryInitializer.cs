using LoginSystem.Api.Options;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;

namespace LoginSystem.Api.ApplicationInsights;

public class TelemetryInitializer(IOptions<EnvironmentOptions> environmentOptions) : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        var assemblyName = typeof(Program).Assembly.GetName().Name;
        var environmentName = environmentOptions.Value.Name;
        telemetry.Context.Cloud.RoleName = $"{assemblyName}-{environmentName}";
    }
}
