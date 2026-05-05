using Exam.Models;
using Exam.Repo;
using Exam.Service;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;


namespace Exam.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_ShouldReturnNull_WhenUserNotFound()
    {
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher<User>>();
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        var passwordResetTokenRepository = new Mock<IPasswordResetTokenRepository>();
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "Jwt:Key", "test-key" },
        { "Jwt:Issuer", "test-issuer" },
        { "Jwt:Audience", "test-audience" }
    })
    .Build();

        userRepository
            .Setup(repository => repository.GetUserByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var authService = new AuthService(
            userRepository.Object,
            passwordResetTokenRepository.Object,
            emailService.Object,
            config,
            passwordHasher.Object,
            jwtTokenGenerator.Object
);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "password"
        });

        Assert.Null(result);
        passwordHasher.Verify(
            hasher => hasher.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        jwtTokenGenerator.Verify(generator => generator.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_ShouldReturnNullAndNotGenerateToken_WhenPasswordIsInvalid()
    {
        var user = CreateUser();
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher<User>>();
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        var passwordResetTokenRepository = new Mock<IPasswordResetTokenRepository>();
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "Jwt:Key", "test-key" },
        { "Jwt:Issuer", "test-issuer" },
        { "Jwt:Audience", "test-audience" }
    })
    .Build();
        

        userRepository
            .Setup(repository => repository.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwordHasher
            .Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, "wrong-password"))
            .Returns(PasswordVerificationResult.Failed);

        var authService = new AuthService(
            userRepository.Object,
            passwordResetTokenRepository.Object,
            emailService.Object,
            config,
            passwordHasher.Object,
            jwtTokenGenerator.Object
);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = user.Email,
            Password = "wrong-password"
        });

        Assert.Null(result);
        jwtTokenGenerator.Verify(generator => generator.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = CreateUser();
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher<User>>();
        var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        var passwordResetTokenRepository = new Mock<IPasswordResetTokenRepository>();
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "Jwt:Key", "test-key" },
        { "Jwt:Issuer", "test-issuer" },
        { "Jwt:Audience", "test-audience" }
    })
    .Build();

        userRepository
            .Setup(repository => repository.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwordHasher
            .Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, "Password123!"))
            .Returns(PasswordVerificationResult.Success);

        jwtTokenGenerator
            .Setup(generator => generator.GenerateToken(user))
            .Returns(new JwtTokenResult
            {
                Token = "mock-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });

        var authService = new AuthService(
            userRepository.Object,
            passwordResetTokenRepository.Object,
            emailService.Object,
            config,
            passwordHasher.Object,
            jwtTokenGenerator.Object
);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = user.Email,
            Password = "Password123!"
        });

        Assert.NotNull(result);
        Assert.Equal("mock-jwt-token", result.Token);
        Assert.Equal(DateTime.UtcNow.AddHours(1), result.ExpiresAt);
        Assert.Equal("Admin", result.FirstName);
        Assert.Equal("User", result.LastName);

        userRepository.Verify(
            repository => repository.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()),
            Times.Once);
        passwordHasher.Verify(
            hasher => hasher.VerifyHashedPassword(user, user.Password, "Password123!"),
            Times.Once);
        jwtTokenGenerator.Verify(generator => generator.GenerateToken(user), Times.Once);
    }


    private static User CreateUser()
    {
        return new User
        {
            Id = 1,
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            Password = "hashed-password",
            Role = UserRole.Admin
        };
    }
}
