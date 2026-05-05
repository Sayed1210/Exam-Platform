namespace Exam.Repo;

using Exam.Data;
using Exam.Models;
using Microsoft.EntityFrameworkCore;

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

    public PasswordResetTokenRepository(ApiContext context)
    {
        _context = context;
    }

    public async Task AddPasswordResetTokenAsync(PasswordResetToken resetToken, CancellationToken cancellationToken)
    {
        await _context.PasswordResetTokens.AddAsync(resetToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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
        resetToken.UsedAt = utcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePasswordResetTokenAsync(PasswordResetToken resetToken, CancellationToken cancellationToken)
    {
        _context.PasswordResetTokens.Remove(resetToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
