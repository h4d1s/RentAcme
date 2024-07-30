using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulIntegrationHelpers.Services;

public class ConsulServiceDiscovery : IConsulServiceDiscovery
{
    private readonly IConsulClient _consulClient;

    public ConsulServiceDiscovery(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }

    public async Task<string> GetServiceAddress(string serviceName, int retryInterval = 5000, int maxRetries = 12)
    {
        var retries = 0;
        var address = "";
        while (true)
        {
            var services = await _consulClient.Health.Service(serviceName, "", true);
            var service = services.Response.FirstOrDefault();

            if (service != null)
            {
                address = $"{service.Service.Address}:{service.Service.Port}";
                break;
            }

            retries++;
            if (retries >= maxRetries)
            {
                throw new Exception($"Service '{serviceName}' not found in Consul after {maxRetries} attempts.");
            }

            await Task.Delay(retryInterval);
        }

        return address;
    }
}
