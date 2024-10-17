using Microsoft.EntityFrameworkCore;
using User.Management.Domain.Models;

namespace User.Management.API.DataAccess;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }
    
    public DbSet<ApplicationUser?> AspNetUsers { get; set; } = null!;
}