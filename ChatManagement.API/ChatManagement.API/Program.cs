using ChatManagement.DataAccess;
using ChatManagement.Domain;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Repositories;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.Middlewares;
using ChatManagement.Infrastructure.Repositories;
using ChatManagement.Services.Services;
using Microsoft.EntityFrameworkCore;

namespace ChatManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        builder.Services.AddDbContext<ChatManagementDbContext>(options => 
            options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
        
        builder.Services.AddScoped<IRepository<Chat>, Repository<Chat>>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IChatService, ChatService>();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
       
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseMiddleware<GlobalExceptionHandler>();

        app.MapControllers();
        
        ApplyMigrations(app);

        app.Run();
    }
    
    public static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ChatManagementDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}