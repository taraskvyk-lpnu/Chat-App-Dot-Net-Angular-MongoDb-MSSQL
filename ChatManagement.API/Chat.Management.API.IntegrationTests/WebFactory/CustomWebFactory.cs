using ChatManagement.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChatDomain = ChatManagement.Domain.Models.Chat;

namespace Chat.Management.API.IntegrationTests.WebFactory;

public class CustomWebFactory<Program> : WebApplicationFactory<ChatManagement.API.Program>
{
    private ChatDomain _validChat;
    private ChatDomain _chatToRemove;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<ChatManagementDbContext>(options =>
                options.UseInMemoryDatabase("ChatManagement.Integration.Tests"));
            
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
        _validChat = new ChatDomain
        {
            CreatorId = Guid.NewGuid(),
            Title = "Test chat",
            CreatedAt = DateTime.Now,
            UserIds = [Guid.NewGuid()]
        };
        
        context.Chats.Add(_validChat);
        context.SaveChanges();
        
        _chatToRemove = new ChatDomain
        {
            CreatorId = Guid.NewGuid(),
            Title = "To remove chat",
            CreatedAt = DateTime.Now,
            UserIds = [Guid.NewGuid()]
        };
        
        
        context.Chats.Add(_chatToRemove);
        context.SaveChanges();
    }

    public ChatDomain GetValidChat()
    {
        return _validChat;
    }
    
    public ChatDomain GetChatToRemove()
    {
        return _chatToRemove;
    }
}