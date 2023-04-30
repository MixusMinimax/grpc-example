using Grpc.Core;
using Proto;
using Proto.User;

namespace UserService.Services;

public class UserService : Proto.User.UserService.UserServiceBase
{
    public override Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        return Task.FromResult(new SignInResponse
        {
            Jwt = "jwt"
        });
    }

    public override Task<GetCurrentUserResponse> GetCurrentUser(GetCurrentUserRequest request,
        ServerCallContext context)
    {
        return Task.FromResult(new GetCurrentUserResponse
        {
            User = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "maxi.barmetler@gmail.com",
                Name = new HumanName
                {
                    FirstName = "Maximilian",
                    MiddleNames = { "Erich" },
                    LastName = "Barmetler"
                }
            }
        });
    }
}