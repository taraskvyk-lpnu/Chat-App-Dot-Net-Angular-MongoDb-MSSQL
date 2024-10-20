namespace User.Management.API;

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
        
        builder.Services.AddUserDbContext(builder.Configuration["ConnectionStrings:DefaultConnection"]!);
        builder.Services.AddScopedServices();
        builder.Services.AddMapper();
        builder.AddSwaggerGen();
        builder.AddCustomCors();
        builder.AddJwtAuth();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("FullAccess");
        app.MapControllers();
        
        app.Run();
    }
}