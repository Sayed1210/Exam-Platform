using System.Net;
using System.Net.Http.Json;
using Exam.Models;
using Exam.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

namespace Exam.Tests.APIs;

public class AuthApiTests
{
    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(service => service.LoginAsync(
                It.Is<LoginRequest>(request =>
                    request.Email == "admin@example.com" &&
                    request.Password == "Password123!"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResponse
            {
                Token = "mock-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                FirstName = "Admin",
                LastName = "User"
            });

        await using var factory = CreateFactory(authService);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "admin@example.com",
            Password = "Password123!"
        });

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("mock-jwt-token", result.Token);
        Assert.Equal("Admin", result.FirstName);
        Assert.Equal("User", result.LastName);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginResponse?)null);

        await using var factory = CreateFactory(authService);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "admin@example.com",
            Password = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory(Mock<IAuthService> authService)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IAuthService>();
                    services.AddScoped(_ => authService.Object);
                });
            });
    }
}
