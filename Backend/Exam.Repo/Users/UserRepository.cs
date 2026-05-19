using Exam.Data;
using Exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exam.Repo;

public class UserRepository : IUserRepository
{
    private readonly ApiContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ApiContext context,
        ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _context.User
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
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetUserByEmailAsync failed — Email={Email}",
                email);

            return null;
        }
    }

    public async Task<User?> GetUserByIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _context.User
                .FirstOrDefaultAsync(
                    user => user.Id == userId,
                    cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetUserByIdAsync failed — UserId={UserId}",
                userId);

            return null;
        }
    }

    public async Task UpdateUserPasswordAsync(
        User user,
        string passwordHash,
        CancellationToken cancellationToken)
    {
        try
        {
            user.Password = passwordHash;

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "UpdateUserPasswordAsync failed — UserId={UserId}",
                user.Id);
        }
    }
}