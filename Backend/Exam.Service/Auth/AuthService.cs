namespace Exam.Service;

using System.Security.Cryptography;
using System.Text;
using Exam.Models;
using Exam.Repo;
using Exam.Service.EmailTemplates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

// public interface IAuthService
// {
//     Task ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken);
//     Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
// }

public class AuthService : IAuthService
{
    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromHours(1);
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    public AuthService(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailService emailService,
        IConfiguration configuration,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailService = emailService;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }


    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

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

    public async Task ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return;
        }

        var token = Guid.NewGuid().ToString("N");
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = HashToken(token),
            ExpiresAt = DateTime.UtcNow.Add(ResetTokenLifetime)
        };

        var resetLink = BuildResetLink(token);
        var htmlBody = EmailTemplateBuilder.BuildPasswordReset(resetLink, user.FirstName);

        await _passwordResetTokenRepository.AddPasswordResetTokenAsync(resetToken, cancellationToken);

        try
        {
            await _emailService.SendEmailAsync(user.Email, "Reset your password", htmlBody);
        }
        catch
        {
            await _passwordResetTokenRepository.DeletePasswordResetTokenAsync(resetToken, cancellationToken);
            
        }
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.Token);
        var resetToken = await _passwordResetTokenRepository.GetActivePasswordResetTokenAsync(
            tokenHash,
            DateTime.UtcNow,
            cancellationToken);
        if (resetToken is null)
        {
            return false;
        }

        var passwordHash = _passwordHasher.HashPassword(resetToken.User, request.NewPassword);
        await _userRepository.UpdateUserPasswordAsync(resetToken.User, passwordHash, cancellationToken);
        await _passwordResetTokenRepository.MarkPasswordResetTokenUsedAsync(resetToken, DateTime.UtcNow, cancellationToken);

        return true;
    }

    private string BuildResetLink(string token)
    {
        var resetPasswordUrl = _configuration["Auth:ResetPasswordUrl"] ?? "http://localhost:3000/reset-password";
        var separator = resetPasswordUrl.Contains('?') ? "&" : "?";

        return $"{resetPasswordUrl}{separator}token={Uri.EscapeDataString(token)}";
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
