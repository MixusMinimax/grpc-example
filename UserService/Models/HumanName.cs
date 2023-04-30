using Microsoft.EntityFrameworkCore;

namespace UserService.Models;

[Owned]
public class HumanName
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public List<string>? MiddleNames { get; set; }
    public string? Title { get; set; }
}