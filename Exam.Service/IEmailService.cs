namespace Exam.Service;

public interface IEmailService
{
   
    Task SendInvitationEmail(string email, string token);
}