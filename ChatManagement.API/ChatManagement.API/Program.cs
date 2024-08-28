using ChatManagement.API.Extensions;
using ChatManagement.Infrastructure.Middlewares;
namespace ChatManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.AddAuthPolicies();
        builder.AddCustomCors();
        builder.AddJwtAuth();
        builder.AddDbContext();
        builder.AddScopedServices();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwaggerGen();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<GlobalExceptionHandler>();
        app.UseCors("FullAccess");

        app.MapControllers();
        //app.ApplyMigrations();
        app.Run();
    }
}