using FinalProject.Backend.Data.Entities;

namespace FinalProject.Backend.Modules.Auth.Repositories;

public interface IJwtRepository
{
    public Task<string> GenerateAndSaveRefreshTokenAsync(ApplicationUser user);

    public string GenerateRefreshTokenAsync();
}
