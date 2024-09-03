using ChatMessaging.Extensions;

namespace ChatMessaging;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.AddChatServices();
        
        builder.AddJwtAuth();

        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwaggerGen();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatHub>("/chathub");
        });
        app.Run();
    }
}