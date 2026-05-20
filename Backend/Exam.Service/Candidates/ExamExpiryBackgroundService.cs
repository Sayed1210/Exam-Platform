using Exam.Repo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Exam.Service;

public class ExamExpiryBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExamExpiryBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public ExamExpiryBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ExamExpiryBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExamExpiryBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateExpiredExamsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expired exams");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("ExamExpiryBackgroundService stopped");
    }

    private async Task UpdateExpiredExamsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var candidateExamRepo = scope.ServiceProvider.GetRequiredService<ICandidateExamRepository>();

        var updatedCount = await candidateExamRepo.UpdateExpiredExamsAsync();
        if (updatedCount > 0)
        {
            _logger.LogInformation("Automatically updated {Count} expired exam statuses", updatedCount);
        }
    }
}
