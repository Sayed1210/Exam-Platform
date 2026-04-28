using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Exam.Data;
using Exam.Repo;
using Exam.Service;
using Exam.Api.endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Database
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