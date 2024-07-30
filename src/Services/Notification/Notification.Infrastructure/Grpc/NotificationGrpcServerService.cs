using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Notification.Appication.Infrastructure.Services;
using Notificationproto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Grpc;

public class NotificationGrpcServerService : NotificationProtoService.NotificationProtoServiceBase
{
    private readonly ILogger<NotificationGrpcServerService> _logger;
    private readonly IEmailService _emailService;

    public NotificationGrpcServerService(
        ILogger<NotificationGrpcServerService> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public override Task<Empty> SendEmail(NotificationEmailRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Recipient))
        {
            throw new ArgumentException("The value cannot be an empty string.", nameof(request.Recipient));
        }

        if (string.IsNullOrEmpty(request.Subject))
        {
            throw new ArgumentException("The value cannot be an empty string.", nameof(request.Subject));
        }

        if (string.IsNullOrEmpty(request.Message))
        {
            throw new ArgumentException("The value cannot be an empty string.", nameof(request.Message));
        }

        _emailService.SendAsync(request.Recipient, request.Subject, request.Message);

        return Task.FromResult(new Empty());
    }

    public override Task<Empty> SendSms(NotificationSmsRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}
