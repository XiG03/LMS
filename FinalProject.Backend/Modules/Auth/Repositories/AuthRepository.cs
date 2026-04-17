using FinalProject.Backend.Data.DbContext;
using FinalProject.Backend.Data.Entities;
using FinalProject.Backend.Modules.Auth.DTOs;
using FinalProject.Backend.Modules.Auth.Services;
using Microsoft.AspNetCore.Identity;

namespace FinalProject.Backend.Modules.Auth.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthRepository(AppDbContext context, 
                            UserManager<ApplicationUser> userManager, 
                            SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ApplicationUser?> AuthenticateAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if(user == null)
        {
            return null;
        }
        
        if(!await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        return user;
    }

    public async Task<ApplicationUser?> RegisterAsync(AuthDto request)
    {
        var user = new ApplicationUser { UserName = request.Username, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to register user: {errors}");
        }
        return user;
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username) => await _userManager.FindByNameAsync(username);
    
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);
    
    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId) => await _userManager.FindByIdAsync(userId.ToString());

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(ApplicationUser user, string resetToken, string newPassword)
    {
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        return result.Succeeded;
    }

    public async Task UpdateSecurityStampAsync(ApplicationUser user)
    {
        await _userManager.UpdateSecurityStampAsync(user);
    }

    public Task UpdateAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        return _context.SaveChangesAsync();
        throw new NotImplementedException();
    }
}
