using Microsoft.EntityFrameworkCore;
using ExamApi.Models;

public class UserRepository : IUserRepository
{
    private readonly ApiContext _context;

    public UserRepository(ApiContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}