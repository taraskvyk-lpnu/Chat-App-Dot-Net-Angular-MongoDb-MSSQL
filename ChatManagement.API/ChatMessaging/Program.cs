using ChatMessaging.Contracts;
using ChatMessaging.Implementations;

namespace ChatMessaging;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IChatRepository, MongoChatRepository>();
        builder.Services.AddSignalR();

        builder.Services.AddEndpointsApiExplorer();
        
        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatHub>("/chathub");
        });
        app.Run();
    }
}