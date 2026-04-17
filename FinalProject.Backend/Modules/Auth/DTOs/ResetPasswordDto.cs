using System.ComponentModel.DataAnnotations;

namespace FinalProject.Backend.Modules.Auth.DTOs;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string ResetToken { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}
