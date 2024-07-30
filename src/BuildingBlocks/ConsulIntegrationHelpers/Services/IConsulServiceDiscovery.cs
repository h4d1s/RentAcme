using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulIntegrationHelpers.Services;

public interface IConsulServiceDiscovery
{
    public Task<string> GetServiceAddress(string serviceName, int retryInterval = 5000, int maxRetries = 12);
}
