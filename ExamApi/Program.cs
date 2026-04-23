using ExamApi.Controllers;
using ExamApi.Repositories;
using ExamApi.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// Exam Repository
builder.Services.AddScoped<IExamRepository, ExamRepository>();

// Exam Service
builder.Services.AddScoped<IExamService, ExamService>();


builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ITopicService, TopicService>();

// Question Repository
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// Question Service
builder.Services.AddScoped<IQuestionService, QuestionService>();

//Choice Repository
builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();

//Choice Service
builder.Services.AddScoped<IChoiceService, ChoiceService>();


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
app.MapExamEndpoints();
app.MapQuestionEndpoints();
app.MapChoiceEndpoints();
app.MapTopicEndpoints();


app.Run();


