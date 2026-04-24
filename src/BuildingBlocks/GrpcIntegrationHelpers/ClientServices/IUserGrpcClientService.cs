using GrpcIntegrationHelpers.Models;

namespace GrpcIntegrationHelpers.ClientServices;

public interface IUserGrpcClientService
{
    public Task<bool> CheckIfExistsAsync(Guid id);
    public Task<UserDto> GetUserAsync(Guid id);
    public Task<UserDto> GetUserByExternalIdAsync(string externalId);
}
