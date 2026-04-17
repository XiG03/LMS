using FinalProject.Backend.Modules.Auth.DTOs;
using FinalProject.Backend.Modules.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Backend.Modules.Auth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tokenResponse = await _authService.LoginAsync(request);

        if (tokenResponse == null)
            return Unauthorized(new { Message = "Nguoi dung hoac mat khau khong dung." }); // Translation: Invalid username or password

        return Ok(tokenResponse);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] AuthDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _authService.RegisterAsync(request);
            if (user == null)
                return BadRequest("Failed to register.");

            return Ok(new { Message = "Dang ky thanh cong!", UserId = user.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize] // Require client to have valid JWT
    public async Task<IActionResult> Logout()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            return Unauthorized(new { Message = "User ID invalid or token missing." });

        var success = await _authService.LogoutAsync(userId);
        
        if (!success)
            return NotFound(new { Message = "User not found to logout." });

        return Ok(new { Message = "Logout thanh cong (Da invalidate token hien tai)." });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var message = await _authService.ForgotPasswordAsync(request.Username, request.Email);
        
        // Luôn trả Http 200 OK kề cả khi không tìm thấy user hoặc email sai vì mục đích bảo mật Security (chống lộ lọt username/email)
        return Ok(new { Message = message });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _authService.ResetPasswordAsync(request.Email, request.ResetToken, request.NewPassword);

        if (!success)
            return BadRequest(new { Message = "Email sai, hoac mang ResetToken khong hop le." });

        return Ok(new { Message = "Doi mat khau thanh cong! Ban co the Login voi mat khau moi." });
    }
}
