using FinalProject.Backend.Data.Entities;
using FinalProject.Backend.Modules.Auth.DTOs;

namespace FinalProject.Backend.Modules.Auth.Services;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto request);
    Task<ApplicationUser?> RegisterAsync(AuthDto request);
    Task<bool> LogoutAsync(Guid userId);
    Task<string> ForgotPasswordAsync(string username, string email);
    Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword);
}
