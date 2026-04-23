using Microsoft.AspNetCore.Mvc;
using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Services;

namespace ExamApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public static class ExamEndpoints
    {
        public static void MapExamEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/exams")
                           .WithTags("Exams");

            // GET /api/exams
            group.MapGet("/", async (IExamService svc) =>
            {
                var exams = await svc.GetAllExamsAsync();
                return Results.Ok(exams);
            })
            .WithName("GetAllExams");

            // GET /api/exams/{id}
            group.MapGet("/{id:int}", async (int id, IExamService svc) =>
            {
                var exam = await svc.GetExamByIdAsync(id);

                return exam is null
                    ? Results.NotFound(new { message = $"Exam with Id {id} not found" })
                    : Results.Ok(exam);
            })
            .WithName("GetExamById");

            // POST /api/exams
            group.MapPost("/", async (CreateExamDto dto, IExamService svc) =>
            {
                var exam = await svc.CreateExamAsync(dto);

                return Results.CreatedAtRoute("GetExamById", new { id = exam.Id }, exam);
            })
            .WithName("CreateExam");

            // PUT /api/exams/{id}
            group.MapPut("/{id:int}", async (int id, CreateExamDto dto, IExamService svc) =>
            {
               

                var exam = await svc.UpdateExamAsync(id, dto);

                return exam is null
                    ? Results.NotFound(new { message = $"Exam with Id {id} not found" })
                    : Results.Ok(exam);
            })
            .WithName("UpdateExam");

            // DELETE /api/exams/{id}
            group.MapDelete("/{id:int}", async (int id, IExamService svc) =>
            {
                var result = await svc.DeleteExamAsync(id);

                return result
                    ? Results.NoContent()
                    : Results.NotFound(new { message = $"Exam with Id {id} not found" });
            })
            .WithName("DeleteExam");
        }
    }
}
