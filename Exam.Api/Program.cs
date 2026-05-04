using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Exam.Data;

var builder = WebApplication.CreateBuilder(args);

=======
using Exam.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Exam.Repo;




var builder = WebApplication.CreateBuilder(args);


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

// Repositories
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();

// Services
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ITopicService, TopicService>();
=======
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

builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ICandidateExamRepository, CandidateExamRepository>();
>>>>>>> Exam.Api/Program.cs
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapInvitationsEndpoints();
app.UseHttpsRedirection();
// Endpoints
app.MapExamEndpoints();
app.MapQuestionEndpoints();
app.MapTopicEndpoints();

app.Run();

public partial class Program;
=======
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
