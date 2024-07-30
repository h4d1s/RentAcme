using GrpcIntegrationHelpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcIntegrationHelpers.ClientServices;

public interface IUserGrpcClientService
{
    public Task<bool> CheckIfExistsAsync(Guid id);
    public Task<UserDto> GetUserAsync(Guid id);
}
