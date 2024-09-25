using ChatManagement.DataAccess.Configurations;
using ChatManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatManagement.DataAccess;

public class ChatManagementDbContext : DbContext
{
    public ChatManagementDbContext(DbContextOptions<ChatManagementDbContext> options) : base(options)
    {
    }
    
    public DbSet<Chat> Chats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
    }
}