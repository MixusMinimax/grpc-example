using Grpc.Net.Client;
using Proto.User;

using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var stub = new Proto.User.UserService.UserServiceClient(channel);
var reply = await stub.SignInAsync(new SignInRequest { Username = "maxi.barmetler@gmail.com", Password = "test1234" });
Console.WriteLine($"Response: {reply}");

var currentUser = await stub.GetCurrentUserAsync(new GetCurrentUserRequest());
Console.WriteLine($"Response: {currentUser}");