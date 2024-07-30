using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulIntegrationHelpers.HostedServices;

public class ConsulServiceRegistration : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly Guid _serviceId;
    private readonly ILogger<ConsulServiceRegistration> _logger;

    private readonly string _serviceName;
    private readonly string _serviceHost;
    private readonly int _servicePort;

    public ConsulServiceRegistration(
        IConsulClient consulClient,
        ILogger<ConsulServiceRegistration> logger,
        string serviceHost,
        string serviceName,
        int servicePort)
    {
        _consulClient = consulClient ?? throw new ArgumentNullException(nameof(consulClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _serviceHost = serviceHost ?? throw new ArgumentNullException(nameof(serviceHost));
        _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        _servicePort = servicePort;

        _serviceId = Guid.NewGuid();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var registration = new AgentServiceRegistration()
        {
            ID = _serviceId.ToString(),
            Name = _serviceName,
            Address = _serviceHost,
            Port = _servicePort,
            Check = new AgentServiceCheck()
            {
                HTTP = $"https://{_serviceHost}:{_servicePort}/hc",
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                TLSSkipVerify = true,
            }
        };

        _logger.LogInformation($"Registering {_serviceName} with Consul.");

        await _consulClient.Agent.ServiceRegister(registration);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deregistering {_serviceName} from Consul.");
        await _consulClient.Agent.ServiceDeregister(_serviceId.ToString());
    }
}
