using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinalProject.Backend.Data.DbContext;
using FinalProject.Backend.Data.Entities;
using FinalProject.Backend.Modules.Auth.DTOs;
using FinalProject.Backend.Modules.Auth.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinalProject.Backend.Modules.Auth.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IJwtRepository _jwtRepository;
    public JwtService(IConfiguration configuration, 
                        UserManager<ApplicationUser> userManager,
                        IJwtRepository jwtRepository)
    {
        _configuration = configuration;
        _userManager = userManager;
        _jwtRepository = jwtRepository;
    }
    public string CreateTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email) // Add email claim
            // Add Role for here to Claims
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetValue<string>("Jwt:Token")!
        ));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(

            issuer: _configuration.GetValue<string>("Jwt:Issuer"),
            audience: _configuration.GetValue<string>("Jwt:Audience"),
            claims: claims,
            expires: DateTime.Now.AddDays(1), // can be change on 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public async Task<TokenResponseDto> CreateTokenResponseAsync(ApplicationUser user)
    {
        return new TokenResponseDto
        {
            AccessToken = CreateTokenAsync(user),
            RefreshToken = await _jwtRepository.GenerateAndSaveRefreshTokenAsync(user)
        };
    }
  

    public async Task<TokenResponseDto> RefreshTokenAsync(TokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (user == null)
        {
            return null;
        }
        return await CreateTokenResponseAsync(user);
    }

    public async Task<ApplicationUser> ValidateRefreshTokenAsync(Guid userId,string refreshToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return null;
        }
        return user;
    }
}

