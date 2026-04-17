using System.ComponentModel.DataAnnotations;

namespace FinalProject.Backend.Modules.Auth.DTOs;

public class ForgotPasswordDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
