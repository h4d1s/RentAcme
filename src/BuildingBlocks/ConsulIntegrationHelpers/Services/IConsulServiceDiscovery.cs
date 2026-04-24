namespace ConsulIntegrationHelpers.Services;

public interface IConsulServiceDiscovery
{
    public Task<string> GetServiceAddress(string serviceName, int retryInterval = 5000, int maxRetries = 12);
}
