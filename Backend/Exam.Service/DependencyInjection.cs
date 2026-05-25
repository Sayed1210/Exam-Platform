namespace Exam.Service;
using Exam.Service;
using Exam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


public static class DependencyInjection
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Exam
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ITopicService, TopicService>();

        // Invitation
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IInvitationValidator, InvitationValidator>();

        // Exam Flow
        services.AddScoped<IStartExamService, StartExamService>();
        services.AddScoped<IBeforeStartExamService, BeforeStartExamService>();
        services.AddScoped<IExamSubmissionService, ExamSubmissionService>();
        services.AddScoped<IVerifyInvitationService, VerifyInvitationService>();

        // Candidate
        services.AddScoped<ICandidateService, CandidateService>();

        return services;
    }
    
}
