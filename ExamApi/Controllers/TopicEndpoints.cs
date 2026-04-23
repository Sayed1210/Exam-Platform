using ExamApi.DTOs.Requests;
using ExamApi.Services;

namespace ExamApi.Controllers
{
    public static class TopicEndpoints
    {
        public static void MapTopicEndpoints(this WebApplication app)
        {
            // Group all topic endpoints under /api/topics
            var group = app.MapGroup("/api/topics")
                           .WithTags("Topics");


            // GET /api/topics

            group.MapGet("/", GetAllTopics)
                 .WithName("GetAllTopics");

            // GET /api/topics/{id}

            group.MapGet("/{id:int}", GetTopicById)
                 .WithName("GetTopicById");


            // POST /api/topics

            group.MapPost("/", CreateTopic)
                 .WithName("CreateTopic");

            // 
            // PUT /api/topics/{id}
            // 
            group.MapPut("/{id:int}", UpdateTopic)
                 .WithName("UpdateTopic");

            // 
            // DELETE /api/topics/{id}
            // 
            group.MapDelete("/{id:int}", DeleteTopic)
                 .WithName("DeleteTopic");
        }

        // ════════════════════════════════════════════════════════════
        // HANDLERS
        // ════════════════════════════════════════════════════════════

        // GET ALL
        private static async Task<IResult> GetAllTopics(ITopicService service)
        {
            var topics = await service.GetAllTopicsAsync();
            return Results.Ok(topics);
        }

        // GET BY ID
        private static async Task<IResult> GetTopicById(
            int id,
            ITopicService service)
        {
            var topic = await service.GetTopicByIdAsync(id);

            if (topic == null)
                return Results.NotFound(
                    new { message = $"Topic with Id {id} not found" });

            return Results.Ok(topic);
        }

        // CREATE
        private static async Task<IResult> CreateTopic(
            TopicRequest dto,
            ITopicService service)
        {
            try
            {
                var topic = await service.CreateTopicAsync(dto);
                return Results.Created($"/api/topics/{topic.Id}", topic);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        // UPDATE
        private static async Task<IResult> UpdateTopic(
            int id,
            TopicRequest dto,
            ITopicService service)
        {
            // Check id in URL matches id in body


            try
            {
                var topic = await service.UpdateTopicAsync(id, dto);

                if (topic == null)
                    return Results.NotFound(
                        new { message = $"Topic with Id {id} not found" });

                return Results.Ok(topic);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        // DELETE
        private static async Task<IResult> DeleteTopic(int id, ITopicService service)
        {
            var result = await service.DeleteTopicAsync(id);

            if (!result)
                return Results.NotFound(
                    new { message = $"Topic with Id {id} not found" });

            return Results.NoContent();

            
        }
    }
}
    
