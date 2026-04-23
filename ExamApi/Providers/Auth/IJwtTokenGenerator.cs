using ExamApi.Models;
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}