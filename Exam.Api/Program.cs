using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Exam.Api;
using Exam.Data;
using Exam.Api.Endpoints;
using Exam.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Exam.Service.Auth;
using Exam.Repo;




var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddRepositories();
builder.Services.AddServices();

// DI Candidate
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
// DI ExamSubmit
builder.Services.AddScoped<ICandidateAnswerRepository, CandidateAnswerRepository>();
builder.Services.AddScoped<ICandidateExamRepository, CandidateExamRepository>();
builder.Services.AddScoped<IExamSubmissionService, ExamSubmissionService>();
// DI Verify Link
builder.Services.AddScoped<IVerifyInvitationService, VerifyInvitationService>();

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));
<<<<<<< Exam.Api/Program.cs

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.AddRepositoryLayer(); //Dependency injection for repository layer, exists in Exam.Repo/DependencyInjection.cs 
builder.Services.AddServiceLayer(); //Dependency injection for service layer, exists in Exam.Service/DependencyInjection.cs 

var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

=======
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ICandidateExamRepository, CandidateExamRepository>();
>>>>>>> Exam.Api/Program.cs
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapInvitationsEndpoints();
app.UseHttpsRedirection();

<<<<<<< Exam.Api/Program.cs
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
// Candidate
app.MapCandidateEndpoints();
// Submit Exam
app.MapSubmitExamEndpoints();
// Link Verification
app.MapVerifyLinkEndpoints();

app.Run();

public partial class Program { }