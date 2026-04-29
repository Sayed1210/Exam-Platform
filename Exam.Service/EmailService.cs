using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
namespace Exam.Service;
public class EmailService(IConfiguration config) : IEmailService
{
    public async Task SendEmailAsync(string toEmail,string subject,string htmlBody)
    {
        var settings=config.GetSection("EmailSettings");
        //var invitationLink=$"http://localhost:5173/join-exam?token={token}";

        var email=new MimeMessage();

        email.From.Add(new MailboxAddress(settings["SenderName"],settings["SenderEmail"]));

        email.To.Add(MailboxAddress.Parse(toEmail));

        email.Subject=subject;
        
        email.Body=new TextPart("html")
        {
            //Text=$@"
              //  <h1>Hello!</h1>
                //<p>You have been invited to take an exam on <b>Enozom</b> exam platform.</p>
               // <p>Please click the link below to begin:</p>
                //<a href='{invitationLink}'>Start Exam</a>
                //<p>This link will expire in 3 days.</p>"
                Text=htmlBody
        };
        using var smtp=new SmtpClient();
        try
        {
            
            await smtp.ConnectAsync(settings["SmtpServer"], int.Parse(settings["Port"]), SecureSocketOptions.StartTls); //MailKit.Security.
            await smtp.AuthenticateAsync(settings["SenderEmail"], settings["Password"]);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    } 
}