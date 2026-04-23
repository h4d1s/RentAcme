using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Notification.Appication.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _client;
    private readonly IConfiguration _configuration;

    public EmailService(
        IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        var server = _configuration["Smtp:Server"] ?? throw new ArgumentNullException("Smtp server is not configured");
        var port = int.Parse(_configuration["Smtp:Port"] ?? throw new ArgumentNullException("Smtp port is not configured"));
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];

        _client = new SmtpClient
        {
            Host = server,
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = false,
        };
    }

    public async Task SendAsync(string recipient, string subject, string message)
    {
        var from = _configuration["Smtp:FromEmail"] ?? throw new ArgumentNullException("Smtp from email is not configured");
        var mailMessage = new MailMessage
        {
            From = new MailAddress(from),
            Subject = subject,
            Body = message,
        };
        mailMessage.To.Add(recipient);

        _client.UseDefaultCredentials = true;
        await _client.SendMailAsync(mailMessage);
    }
}
