using Grpc.Core;
using Grpc.Net.Client;
using Proto.User;

using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var stub = new UserService.UserServiceClient(channel);
var reply = await stub.SignInAsync(new SignInRequest
    { Username = "maxi.barmetler@gmail.com", Password = "test1234" });
Console.WriteLine($"Response: {reply}");

var currentUser = await stub.GetCurrentUserAsync(new GetCurrentUserRequest(), new Metadata
{
    new("Authorization", $"Bearer {reply.Jwt}"),
});
Console.WriteLine($"Response: {currentUser}");