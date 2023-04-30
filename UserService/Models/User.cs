namespace UserService.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public HumanName? Name { get; set; }
}