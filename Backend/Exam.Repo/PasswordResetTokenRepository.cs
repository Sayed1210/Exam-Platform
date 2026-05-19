namespace Exam.Repo;

using Exam.Data;
using Exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public interface IPasswordResetTokenRepository
{
    Task AddPasswordResetTokenAsync(PasswordResetToken resetToken, CancellationToken cancellationToken);
    Task<PasswordResetToken?> GetActivePasswordResetTokenAsync(string tokenHash, DateTime utcNow, CancellationToken cancellationToken);
    Task MarkPasswordResetTokenUsedAsync(PasswordResetToken resetToken, DateTime utcNow, CancellationToken cancellationToken);
    Task DeletePasswordResetTokenAsync(PasswordResetToken resetToken, CancellationToken cancellationToken);
}

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ApiContext _context;
    private readonly ILogger<PasswordResetTokenRepository> _logger;

    public PasswordResetTokenRepository(ApiContext context, ILogger<PasswordResetTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddPasswordResetTokenAsync(PasswordResetToken resetToken, CancellationToken cancellationToken)
    {
        try
        {
            await _context.PasswordResetTokens.AddAsync(resetToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddPasswordResetTokenAsync failed — UserId={UserId}", resetToken.UserId);
        }
    }

    public Task<PasswordResetToken?> GetActivePasswordResetTokenAsync(
        string tokenHash,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        return _context.PasswordResetTokens
            .Include(resetToken => resetToken.User)
            .FirstOrDefaultAsync(resetToken =>
                resetToken.TokenHash == tokenHash &&
                resetToken.UsedAt == null &&
                resetToken.ExpiresAt > utcNow,
                cancellationToken);
    }

    public async Task MarkPasswordResetTokenUsedAsync(
        PasswordResetToken resetToken,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        try
        {
            resetToken.UsedAt = utcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MarkPasswordResetTokenUsedAsync failed — UserId={UserId}", resetToken.UserId);
        }
    }

    public async Task DeletePasswordResetTokenAsync(PasswordResetToken resetToken, CancellationToken cancellationToken)
    {
        try
        {
            _context.PasswordResetTokens.Remove(resetToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeletePasswordResetTokenAsync failed — UserId={UserId}", resetToken.UserId);
        }
    }
}