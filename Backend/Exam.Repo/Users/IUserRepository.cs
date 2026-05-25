using Exam.Models;

namespace Exam.Repo;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken);
    Task<List<User>> GetSystemUsersAsync(CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task UpdateUserPasswordAsync(User user, string passwordHash, CancellationToken cancellationToken);
}
