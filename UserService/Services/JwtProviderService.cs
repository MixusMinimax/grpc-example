using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserService.Models;

namespace UserService.Services;

public interface IJwtProvider
{
    public Task<string> GenerateJwtForUserAsync(Models.User user);
}

[Service(ServiceLifetime = ServiceLifetime.Transient)]
public class JwtProvider : IJwtProvider
{
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtProvider(IConfiguration configuration)
    {
        var jwk = File.ReadAllText(configuration["JwtSettings:PrivateKeyPath"]!);
        var jsonWebKey = new JsonWebKey(jwk);
        _signingCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwk)), jsonWebKey.Alg);
        
        
        var ecdsa = ECDsa.Create();
        ecdsa.ImportSubjectPublicKeyInfo(Encoding.UTF8.GetBytes(jsonWebKey.N), out _);
        var securityKey = new ECDsaSecurityKey(ecdsa);
    }

    public Task<string> GenerateJwtForUserAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = _signingCredentials
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = _tokenHandler.WriteToken(token);

        return Task.FromResult(tokenString);
    }
}