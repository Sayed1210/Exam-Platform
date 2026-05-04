using Exam.Models;

namespace Exam.Service;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);
}
