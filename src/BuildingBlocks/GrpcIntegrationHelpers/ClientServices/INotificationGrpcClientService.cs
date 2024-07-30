using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcIntegrationHelpers.ClientServices;

public interface INotificationGrpcClientService
{
    public Task SendEmailAsync(string recipient, string subject, string message);
}
