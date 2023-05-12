using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserService.Models;

namespace UserService.Services;

public interface IJwtProvider
{
    public Task<JwtSecurityToken> GenerateJwtForUserAsync(Models.User user);
}

[Service(ServiceLifetime = ServiceLifetime.Transient)]
public class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _config;

    public JwtProvider(IConfiguration configuration)
    {
        _config = configuration;
    }

    public Task<JwtSecurityToken> GenerateJwtForUserAsync(User user)
    {
        var securityKey = new SymmetricSecurityKey(File.ReadAllBytes(_config["JwtSettings:PrivateKeyPath"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
        };
        var token = new JwtSecurityToken(
            _config["JwtSettings:Issuer"],
            _config.GetSection("JwtSettings:ValidAudiences").Get<string[]?>()?.FirstOrDefault(),
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );

        return Task.FromResult(token);
    }
}