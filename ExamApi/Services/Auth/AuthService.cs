using Microsoft.AspNetCore.Identity;
using ExamApi.Models;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IPasswordHasher<User> _hasher;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenGenerator jwt,
        IPasswordHasher<User> hasher)
    {
        _userRepository = userRepository;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null) return null;

        var result = _hasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        return new LoginResponse
        {
            Token = _jwt.GenerateToken(user),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}