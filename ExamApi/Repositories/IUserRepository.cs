using ExamApi.Models;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}