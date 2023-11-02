using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using ScottBrady.IdentityModel.Crypto;
using ScottBrady.IdentityModel.Tokens;
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
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    { 
        _configuration = configuration;
        var jwk = File.ReadAllText(configuration["JwtSettings:PrivateKeyPath"]!);
        var jsonWebKey = new JsonWebKey(jwk);

        if (jsonWebKey.Kty == "OKP")
        {
            var eddsaParams = new EdDsaParameters(jsonWebKey.Crv)
            {
                D = Base64UrlEncoder.DecodeBytes(jsonWebKey.D),
                X = Base64UrlEncoder.DecodeBytes(jsonWebKey.X)
            };

            var eddsa = EdDsa.Create(eddsaParams);
            _signingCredentials = new SigningCredentials(new EdDsaSecurityKey(eddsa), jsonWebKey.Alg);
        }
        else
        {
            _signingCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwk)), jsonWebKey.Alg);
        }
    }

    public Task<string> GenerateJwtForUserAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _configuration["JwtSettings:ValidAudiences:0"],
            Issuer = _configuration["JwtSettings:Issuer"],
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = _signingCredentials
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = _tokenHandler.WriteToken(token);

        return Task.FromResult(tokenString);
    }
}