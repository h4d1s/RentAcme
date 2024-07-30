using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcIntegrationHelpers.Models;

namespace GrpcIntegrationHelpers.ClientServices;

public interface IInventoryGrpcClientService
{
    public Task<bool> CheckIfExistsAsync(Guid id);
    public Task<VehicleDto> GetVehicleAsync(Guid id);
}
