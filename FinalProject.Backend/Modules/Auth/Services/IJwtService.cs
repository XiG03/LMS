using FinalProject.Backend.Data.Entities;
using FinalProject.Backend.Modules.Auth.DTOs;

namespace FinalProject.Backend.Modules.Auth.Services;

public interface IJwtService
{
    public Task<TokenResponseDto> CreateTokenResponseAsync(ApplicationUser user);

    public string CreateTokenAsync(ApplicationUser user);

    public Task<ApplicationUser> ValidateRefreshTokenAsync(Guid userId, string refreshToken);

    public Task<TokenResponseDto> RefreshTokenAsync(TokenRequestDto request);

    

}
