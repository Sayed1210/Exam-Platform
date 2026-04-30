namespace Exam.Service;

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var host = _configuration["Email:Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new InvalidOperationException("Email:Smtp:Host is not configured.");
        }

        var port = int.TryParse(_configuration["Email:Smtp:Port"], out var configuredPort)
            ? configuredPort
            : 587;

        var from = _configuration["Email:From"];
        if (string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException("Email:From is not configured.");
        }

        using var message = new MailMessage(from, to, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        using var smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = bool.TryParse(_configuration["Email:Smtp:EnableSsl"], out var enableSsl) && enableSsl
        };

        var username = _configuration["Email:Smtp:Username"];
        var password = _configuration["Email:Smtp:Password"];
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            smtpClient.Credentials = new NetworkCredential(username, password);
        }

        await smtpClient.SendMailAsync(message);
    }
}
