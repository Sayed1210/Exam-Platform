namespace Exam.Service;

using System.Security.Cryptography;
using System.Text;
using Exam.Models;
using Exam.Models.Dtos.Requests;
using Exam.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

public interface IAuthService
{
    Task ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
}

public class AuthService : IAuthService
{
    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromHours(1);
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailService emailService,
        IConfiguration configuration,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailService = emailService;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
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
        var htmlBody = $"""
            <p>Hello {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>
            <p>Use the link below to reset your password. This link expires in 1 hour.</p>
            <p><a href="{System.Net.WebUtility.HtmlEncode(resetLink)}">Reset password</a></p>
            <p>If you did not request a password reset, you can ignore this email.</p>
            """;

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
        var resetPasswordUrl = _configuration["Auth:ResetPasswordUrl"] ?? "https://localhost:5001/reset-password";
        var separator = resetPasswordUrl.Contains('?') ? "&" : "?";

        return $"{resetPasswordUrl}{separator}token={Uri.EscapeDataString(token)}";
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
