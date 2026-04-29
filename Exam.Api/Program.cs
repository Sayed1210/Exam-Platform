using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Exam.Data;
using Exam.Api;
using Exam.Repo;
using Exam.Service;
using Exam.Api.endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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
builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();

// Services
builder.Services.AddScoped<IChoiceService, ChoiceService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ITopicService, TopicService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Endpoints
app.MapChoiceEndpoints();
app.MapExamEndpoints();
app.MapQuestionEndpoints();
app.MapTopicEndpoints();

app.Run();

public partial class Program;
