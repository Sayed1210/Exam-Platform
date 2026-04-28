using Exam.Models;
using Exam.Models.Dtos.Requests;
using Exam.Models.Dtos.Responses;
using Exam.Repo.Users;
using Microsoft.AspNetCore.Identity;

namespace Exam.Service.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var tokenResult = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResponse
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}
