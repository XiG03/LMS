using FinalProject.Backend.Data.Entities;
using FinalProject.Backend.Modules.Auth.DTOs;
using FinalProject.Backend.Modules.Auth.Repositories;

namespace FinalProject.Backend.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IAuthRepository authRepository, IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto request)
    {
        var user = await _authRepository.AuthenticateAsync(request.Username, request.Password);
        
        if (user == null)
        {
            return null; // Authentication failed
        }

        return await _jwtService.CreateTokenResponseAsync(user);
    }

    public async Task<ApplicationUser?> RegisterAsync(AuthDto request)
    {
        // Simply delegate registration to Repository (which delegates to UserManager)
        return await _authRepository.RegisterAsync(request);
    }

    public async Task<bool> LogoutAsync(Guid userId)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);
        if (user != null)
        {
            // Updating SecurityStamp invalidates all previously issued JWTs 
            // if you configure JwtBearerOptions to validate the security stamp.
            user.RefreshToken = null; // Clear refresh token if you are using it
            user.RefreshTokenExpiryTime = null;

            await _authRepository.UpdateAsync(user);
            return true;
        }
        return false;
    }

    public async Task<string> ForgotPasswordAsync(string username, string email)
    {
        var user = await _authRepository.GetUserByUsernameAsync(username);
        if (user == null || user.Email != email)
        {
            return "User does not exist or email does not match";
        }

        var token = await _authRepository.GeneratePasswordResetTokenAsync(user);
        
        // TODO: Send token to the user's email via an EmailService
        
        return "Password reset code sent to email: " + token; 
    }

    public async Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword)
    {
        var user = await _authRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        return await _authRepository.ResetPasswordAsync(user, resetToken, newPassword);
    }
}
