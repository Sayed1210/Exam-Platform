using Exam.Models;
using Exam.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Exam.Service;

public class UserManagementService(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    ILogger<UserManagementService> logger) : IUserManagementService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly ILogger<UserManagementService> _logger = logger;

    public async Task<List<SystemUserResponse>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetSystemUsersAsync(cancellationToken);
        return users.Select(ToResponse).ToList();
    }

    public async Task<(SystemUserResponse? User, string? Error)> CreateAdminAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existingUser = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (existingUser is not null)
                return (null, "A user with this email already exists");

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Role = UserRole.Admin
            };

            user.Password = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddUserAsync(user, cancellationToken);

            return (ToResponse(user), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAdminAsync failed — Email={Email}", request.Email);
            return (null, "An unexpected error occurred");
        }
    }

    private static SystemUserResponse ToResponse(User user)
    {
        return new SystemUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}
