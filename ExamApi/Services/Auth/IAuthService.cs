public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}