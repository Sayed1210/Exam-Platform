using Exam.Data;
using Exam.Models;
using Exam.Repo;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit;

namespace Exam.Tests.Repositories;

public class UserRepositoryTests : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithDatabase("ExamTestDb")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public Task InitializeAsync()
    {
        return _mySqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mySqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        context.User.Add(new User
        {
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            Password = "hashed-password",
            Role = UserRole.Admin
        });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetUserByEmailAsync("admin@example.com", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("admin@example.com", result.Email);
        Assert.Equal("Admin", result.FirstName);
        Assert.Equal("User", result.LastName);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailNotExists()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetUserByEmailAsync("missing@example.com", CancellationToken.None);

        Assert.Null(result);
    }

    private ApiContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApiContext>()
            .UseMySQL(_mySqlContainer.GetConnectionString())
            .Options;

        return new ApiContext(options);
    }
}
