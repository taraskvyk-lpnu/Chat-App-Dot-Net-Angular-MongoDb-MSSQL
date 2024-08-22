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

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        /*builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));*/
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        //app.UseMiddleware<ExceptionHandler>();

        app.MapControllers();
        
        ApplyMigrations(app);

        app.Run();
    }
    
    public static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        /*using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }*/
    }
}