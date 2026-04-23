using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using ExamApi.Models;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_ShouldReturnNull_WhenUserNotFound()
    {
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var hasher = new Mock<IPasswordHasher<User>>();

        userRepo
            .Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        var service = new AuthService(userRepo.Object, jwt.Object, hasher.Object);

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "password"
        };

        var result = await service.LoginAsync(request);

        result.Should().BeNull();
    }

    [Fact]
public async Task Login_ShouldReturnNull_WhenPasswordIsInvalid()
{
    var userRepo = new Mock<IUserRepository>();
    var jwt = new Mock<IJwtTokenGenerator>();
    var hasher = new Mock<IPasswordHasher<User>>();

    var user = new User
    {
        Id = 1,
        Email = "admin@test.com",
        Password = "hashed"
    };

    userRepo
        .Setup(r => r.GetByEmailAsync(user.Email))
        .ReturnsAsync(user);

    hasher
        .Setup(h => h.VerifyHashedPassword(user, user.Password, "wrong-password"))
        .Returns(PasswordVerificationResult.Failed);

    var service = new AuthService(userRepo.Object, jwt.Object, hasher.Object);

    var request = new LoginRequest
    {
        Email = user.Email,
        Password = "wrong-password"
    };

    var result = await service.LoginAsync(request);

    result.Should().BeNull();
}


[Fact]
public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
{
    var userRepo = new Mock<IUserRepository>();
    var jwt = new Mock<IJwtTokenGenerator>();
    var hasher = new Mock<IPasswordHasher<User>>();

    var user = new User
    {
        Id = 1,
        Email = "admin@test.com",
        Password = "hashed",
        Role = UserRole.Admin
    };

    userRepo
        .Setup(r => r.GetByEmailAsync(user.Email))
        .ReturnsAsync(user);

    hasher
        .Setup(h => h.VerifyHashedPassword(user, user.Password, "correct-password"))
        .Returns(PasswordVerificationResult.Success);

    jwt
        .Setup(j => j.GenerateToken(user))
        .Returns("fake-jwt");

    var service = new AuthService(userRepo.Object, jwt.Object, hasher.Object);

    var request = new LoginRequest
    {
        Email = user.Email,
        Password = "correct-password"
    };

    var result = await service.LoginAsync(request);

    result.Should().NotBeNull();
    result!.Token.Should().Be("fake-jwt");
    result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
}


[Fact]
public async Task Login_ShouldNotGenerateToken_WhenPasswordFails()
{
    var userRepo = new Mock<IUserRepository>();
    var jwt = new Mock<IJwtTokenGenerator>();
    var hasher = new Mock<IPasswordHasher<User>>();

    var user = new User
    {
        Email = "admin@test.com",
        Password = "hashed"
    };

    userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

    hasher
        .Setup(h => h.VerifyHashedPassword(user, user.Password, "wrong"))
        .Returns(PasswordVerificationResult.Failed);

    var service = new AuthService(userRepo.Object, jwt.Object, hasher.Object);

    await service.LoginAsync(new LoginRequest
    {
        Email = user.Email,
        Password = "wrong"
    });

    jwt.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
}

}