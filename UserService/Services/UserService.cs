using Grpc.Core;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Proto.User;
using UserService.Models;
using User = UserService.Models.User;

namespace UserService.Services;

public class UserService : Proto.User.UserService.UserServiceBase
{
    public override Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        return Task.FromResult(new SignInResponse { Jwt = "jwt" });
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