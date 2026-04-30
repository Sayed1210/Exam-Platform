using Exam.Models.Dtos.Requests;
using Exam.Models.Dtos.Responses;

namespace Exam.Service.Auth;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
