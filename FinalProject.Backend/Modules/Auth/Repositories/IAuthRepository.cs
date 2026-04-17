using FinalProject.Backend.Data.Entities;
using FinalProject.Backend.Modules.Auth.DTOs;

namespace FinalProject.Backend.Modules.Auth.Repositories;

public interface IAuthRepository
{
    Task<ApplicationUser?> RegisterAsync(AuthDto request);
    Task<ApplicationUser?> AuthenticateAsync(string username, string password);
    Task<ApplicationUser?> GetUserByUsernameAsync(string username);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<bool> ResetPasswordAsync(ApplicationUser user, string resetToken, string newPassword);
    Task UpdateSecurityStampAsync(ApplicationUser user);
    Task UpdateAsync(ApplicationUser user);
    
}
