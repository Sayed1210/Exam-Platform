using Exam.Models;

namespace Exam.Service;

public interface IUserManagementService
{
    Task<List<SystemUserResponse>> GetUsersAsync(CancellationToken cancellationToken);
    Task<(SystemUserResponse? User, string? Error)> CreateAdminAsync(CreateUserRequest request, CancellationToken cancellationToken);
}
