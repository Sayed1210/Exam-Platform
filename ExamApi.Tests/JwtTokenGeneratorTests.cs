using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using ExamApi.Models; 
using System.Security.Claims;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "THIS_IS_A_VERY_LONG_SECRET_KEY_123456789" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();

        var generator = new JwtTokenGenerator(config);

        var user = new User
        {
            Id = 1,
            Email = "admin@test.com",
            Role = UserRole.Admin
        };

        // Act
        var token = generator.GenerateToken(user); 

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Assert
        jwt.Claims.Should().Contain(c => c.Value == "admin@test.com");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        jwt.Claims.Should().Contain(c => c.Value == "1");
    }


    [Fact]
public void GenerateToken_ShouldSetIssuerAndAudience()
{
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:Key", "THIS_IS_A_VERY_LONG_SECRET_KEY_123456789" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        })
        .Build();

    var generator = new JwtTokenGenerator(config);

    var user = new User
    {
        Id = 1,
        Email = "admin@test.com",
        Role = UserRole.Admin
    };

    var token = generator.GenerateToken(user);

    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

    jwt.Issuer.Should().Be("TestIssuer");
    jwt.Audiences.Should().Contain("TestAudience");
}


[Fact]
public void GenerateToken_ShouldSetExpiration()
{
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:Key", "THIS_IS_A_VERY_LONG_SECRET_KEY_123456789" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        })
        .Build();

    var generator = new JwtTokenGenerator(config);

    var user = new User { Id = 1, Email = "admin@test.com", Role = UserRole.Admin };

    var token = generator.GenerateToken(user);

    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

    jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
}



[Fact]
public void GenerateToken_ShouldProduceValidJwtFormat()
{
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:Key", "THIS_IS_A_VERY_LONG_SECRET_KEY_123456789" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        })
        .Build();

    var generator = new JwtTokenGenerator(config);

    var user = new User { Id = 1, Email = "admin@test.com", Role = UserRole.Admin };

    var token = generator.GenerateToken(user);

    token.Split('.').Length.Should().Be(3); // header.payload.signature
}



[Fact]
public void GenerateToken_ShouldThrow_WhenKeyMissing()
{
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>())
        .Build();

    var generator = new JwtTokenGenerator(config);

    var user = new User { Id = 1, Email = "admin@test.com", Role = UserRole.Admin };

    Action act = () => generator.GenerateToken(user);

    act.Should().Throw<Exception>();
}
}