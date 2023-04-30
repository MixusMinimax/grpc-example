using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}