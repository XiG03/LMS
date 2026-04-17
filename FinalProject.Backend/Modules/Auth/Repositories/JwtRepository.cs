using System.Security.Cryptography;
using FinalProject.Backend.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace FinalProject.Backend.Modules.Auth.Repositories;

public class JwtRepository : IJwtRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<string> GenerateAndSaveRefreshTokenAsync(ApplicationUser user)
    {
        var refreshToken = GenerateRefreshTokenAsync();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);
        return refreshToken;
    }

    public string GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
