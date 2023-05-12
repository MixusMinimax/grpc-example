using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Common.Attributes;
using Grpc.Core;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Proto.User;
using UserService.Models;
using User = UserService.Models.User;

namespace UserService.Services;

[GrpcService]
public class UserService : Proto.User.UserService.UserServiceBase
{
    private readonly IJwtProvider _jwtProvider;

    public UserService(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        var jwt = await _jwtProvider.GenerateJwtForUserAsync(new User
        {
            Username = "maxi.barmetler@gmail.com"
        });
        var jwtString = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new SignInResponse { Jwt = jwtString };
    }

    [Authorize]
    public override Task<GetCurrentUserResponse> GetCurrentUser(GetCurrentUserRequest request,
        ServerCallContext context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "maxi.barmetler@gmail.com",
            Name = new HumanName
            {
                FirstName = "Maximilian", LastName = "Barmetler", MiddleNames = new List<string> { "Erich" }
            }
        };

        var userMsg = user.Adapt<Proto.User.User>();

        return Task.FromResult(new GetCurrentUserResponse { User = userMsg });
    }
}