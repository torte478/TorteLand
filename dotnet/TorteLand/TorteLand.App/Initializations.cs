using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TorteLand.Core.Contracts;

internal sealed class Initializations : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<Initializations> _logger;

    public Initializations(IServiceProvider services, ILogger<Initializations> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken token)
    {
        foreach (var initialization in _services.GetServices<IInitialization>())
        {
            _logger.LogInformation("Initialization '{Name}' started", initialization.Name);
            await initialization.Initialize(token);
            _logger.LogInformation("Initialization '{Name}' completed", initialization.Name);
        }
        _logger.LogInformation("All initialization completed");
    }

    public Task StopAsync(CancellationToken token)
        => Task.CompletedTask;
}