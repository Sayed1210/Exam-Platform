using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Exam.Data;
using Exam.Repo;
using Exam.Service;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ICandidateExamRepository, CandidateExamRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapInvitationsEndpoints();
app.UseHttpsRedirection();

// Candidate
app.MapCandidateEndpoints();
// Submit Exam
app.MapSubmitExamEndpoints();
// Link Verification
app.MapVerifyLinkEndpoints();

app.Run();


