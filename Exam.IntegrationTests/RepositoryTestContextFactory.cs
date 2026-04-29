using Exam.Data;
using Microsoft.EntityFrameworkCore;

namespace Exam.IntegrationTests;

internal static class RepositoryTestContextFactory
{
    public static ApiContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApiContext(options);
    }
}
