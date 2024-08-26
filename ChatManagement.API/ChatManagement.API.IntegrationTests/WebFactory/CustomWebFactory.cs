using ChatManagement.DataAccess;
using ChatManagement.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatManagement.API.IntegrationTests.WebFactory;

public class CustomWebFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private Chat _chat = new Chat();
    private Chat _chatToRemove = new Chat();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatManagementDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<ChatManagementDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDbForIntegrationTesting"));
            
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ChatManagementDbContext>();

            db.Database.EnsureCreated();

            SeedDatabase(db);
        });
    }
    
    private void SeedDatabase(ChatManagementDbContext context)
    {
        var creatorId = Guid.NewGuid();
        _chat = new Chat
        {
            Title = "Test Chat",
            UserIds = [creatorId, Guid.NewGuid()],
            CreatorId = creatorId,
            CreatedAt = DateTime.Now
        };
        
        context.Chats.Add(_chat);
        context.SaveChanges();
        
        _chatToRemove = new Chat
        {
            Title = "Delete Chat",
            UserIds = [creatorId, Guid.NewGuid()],
            CreatorId = creatorId,
            CreatedAt = DateTime.Now
        };
        
        context.Chats.Add(_chatToRemove);
        context.SaveChanges();
    }

    public Chat GetValidChat()
    {
        return _chat;
    }
    
    public Chat GetChatToRemove()
    {
        return _chatToRemove;
    }
}