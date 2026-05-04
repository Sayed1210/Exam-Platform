// namespace Exam.Repo;

// using Exam.Data;
// using Exam.Models;
// using Microsoft.EntityFrameworkCore;

// public interface IUserRepository
// {
//     Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
//     Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken);
//     Task UpdateUserPasswordAsync(User user, string passwordHash, CancellationToken cancellationToken);
// }

// public class UserRepository : IUserRepository
// {
//     private readonly ApiContext _context;

//     public UserRepository(ApiContext context)
//     {
//         _context = context;
//     }

//     public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
//     {
//         return _context.User.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
//     }

//     public Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
//     {
//         return _context.User.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
//     }

//     public async Task UpdateUserPasswordAsync(User user, string passwordHash, CancellationToken cancellationToken)
//     {
//         user.Password = passwordHash;
//         await _context.SaveChangesAsync(cancellationToken);
//     }
// }
