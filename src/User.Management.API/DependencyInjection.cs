using Microsoft.EntityFrameworkCore;
using User.Management.API.DataAccess;
using User.Management.API.DataAccess.Repository;
using User.Management.API.Helpers;
using User.Management.API.Services;

namespace User.Management.API;

public static class DependencyInjection
{
    public static void AddUserDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
    
    public static void AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
    }
    
    public static void AddMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserProfile));
    }
}