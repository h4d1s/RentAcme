using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Appication.Infrastructure.Services;

public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string message);
}
