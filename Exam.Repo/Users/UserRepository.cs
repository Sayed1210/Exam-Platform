using Exam.Data;
using Exam.Models;
using Microsoft.EntityFrameworkCore;

namespace Exam.Repo.Users;

public class UserRepository : IUserRepository
{
    private readonly ApiContext _context;

    public UserRepository(ApiContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.User
            .AsNoTracking()
            .Where(user => user.Email == email)
            .Select(user => new User
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
