using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ExamApi.Services;
using ExamApi.Repositories;


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

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Candidate
app.MapCandidateEndpoints();
// Submit Exam
app.MapExamEndpoints();

app.Run();


