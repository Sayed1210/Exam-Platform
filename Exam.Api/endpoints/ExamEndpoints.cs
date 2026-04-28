using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using Exam.Service;

namespace Exam.Api.endpoints
{

   
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

            // POST /api/exams/{examId}/questions
            // Body: { "questionIds": [1, 2, 3] }
            group.MapPost("/{examId:int}/questions", async (
                int examId,
                AssignQuestionsRequest request,
                IExamService svc) =>
            {
                await svc.AssignQuestionsAsync(examId, request.QuestionIds);
                return Results.Ok(new { message = "Questions assigned successfully." });
            })
            .WithName("AssignQuestionsToExam")
            .WithSummary("Assign one or more questions to an exam")
            .Produces(200)
            .Produces(400)
            .Produces(404);


            // GET /api/exams/{id}/questions
            group.MapGet("/{id:int}/questions", async (int id, IExamService svc) =>
            {
                var result = await svc.GetExamWithQuestionsAsync(id);
                return result is null
                    ? Results.NotFound(new { message = $"Exam {id} not found." })
                    : Results.Ok(result);
            })
            .WithName("GetExamWithQuestions")
            .WithSummary("Get an exam with all its questions and choices")
            .Produces<ExamResponseDto>(200)
            .Produces(404);

            // DELETE /api/exams/{examId}/questions/{questionId}
            group.MapDelete("/{examId:int}/questions/{questionId:int}", async (
                int examId,
                int questionId,
                IExamService svc) =>
            {
                await svc.RemoveQuestionAsync(examId, questionId);
                return Results.NoContent();
            })
            .WithName("RemoveQuestionFromExam")
            .WithSummary("Remove a question from an exam")
            .Produces(204)
            .Produces(400)
            .Produces(404);
        }

    }
}
