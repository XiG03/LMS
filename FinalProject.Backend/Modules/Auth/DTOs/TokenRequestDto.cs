namespace FinalProject.Backend.Modules.Auth.DTOs;

public class TokenRequestDto
{
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}
