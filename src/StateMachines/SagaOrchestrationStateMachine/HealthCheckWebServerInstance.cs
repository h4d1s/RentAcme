using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace SagaOrchestrationStateMachine;

public sealed class HealthCheckWebServerInstance : IHostedService
{
    private WebApplication? _app;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var webBuilder = WebApplication.CreateBuilder();
        webBuilder.Services.AddHealthChecks();
        webBuilder.Logging.ClearProviders();

        _app = webBuilder.Build();

        _app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
        _app.MapHealthChecks("/health/ready");

        await _app.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_app != null)
        {
            await _app.StopAsync(cancellationToken);
            await _app.DisposeAsync();
        }
    }
}
