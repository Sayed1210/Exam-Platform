using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _repo;

        public ExamService(IExamRepository repo)
        {
            _repo = repo;
        }

        // CREATE
        public async Task<ExamResponseDto> CreateExamAsync(CreateExamDto dto)
        {
            // 1. Create entity from DTO
            var exam = new Exam
            {
                Title = dto.Title,
                DurationMins = dto.DurationMins,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            // 2. Save to database
            await _repo.AddAsync(exam);

            // 3. Return response DTO
            return MapToResponseDto(exam);
        }

        
        // READ - Get All
   
        public async Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync()
        {
            // 1. Get all exams from database
            var exams = await _repo.GetAllAsync();

            // 2. Convert each exam to DTO
            var examDtos = exams.Select(exam => MapToResponseDto(exam));

            // 3. Return list of DTOs
            return examDtos;
        }

        
        // READ - Get By Id
      
        public async Task<ExamResponseDto?> GetExamByIdAsync(int id)
        {
            // 1. Find exam by id
            var exam = await _repo.GetByIdAsync(id);

            // 2. If not found, return null
            if (exam == null)
                return null;

            // 3. Return response DTO
            return MapToResponseDto(exam);
        }

       
        // UPDATE
        
        public async Task<ExamResponseDto?> UpdateExamAsync(int id, CreateExamDto dto)
        {
            // 1. Find existing exam
            var exam = await _repo.GetByIdAsync(id);

            // 2. If not found, return null
            if (exam == null)
                return null;

            // 3. Update entity properties
            exam.Title = dto.Title;
            exam.DurationMins = dto.DurationMins;
            exam.StartTime = dto.StartTime;
            exam.EndTime = dto.EndTime;

            // 4. Save changes
            await _repo.UpdateAsync(exam);

            // 5. Return updated DTO
            return MapToResponseDto(exam);
        }

      
        // DELETE
       
        public async Task<bool> DeleteExamAsync(int id)
        {
            // 1. Find exam
            var exam = await _repo.GetByIdAsync(id);

            // 2. If not found, return false
            if (exam == null)
                return false;

            // 3. Delete exam
            await _repo.DeleteAsync(exam);

            // 4. Return success
            return true;
        }

        
        // HELPER - Map Entity to DTO
        
        private ExamResponseDto MapToResponseDto(Exam exam)
        {
            return new ExamResponseDto
            {
                Id = exam.Id,
                Title = exam.Title,
                DurationMins = exam.DurationMins,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime
            };
        }
    }
}
