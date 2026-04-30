using Exam.Models;

namespace Exam.Service.Auth;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);
}
