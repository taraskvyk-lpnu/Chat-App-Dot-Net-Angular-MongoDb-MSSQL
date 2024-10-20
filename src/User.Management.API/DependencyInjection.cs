using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using User.Management.API.DataAccess;
using User.Management.API.DataAccess.Repository;
using User.Management.API.Helpers;
using User.Management.API.Models;
using User.Management.API.Services;

namespace User.Management.API;

public static class DependencyInjection
{
    public static void AddSwaggerGen(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    }, new string[]{}
                }
            });
        });
    }
    
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
    
    public static void AddCustomCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FullAccess", policyBuilder =>
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());
        });
    }
    
    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration.GetSection("ApiSettings:JwtOptions").Get<JwtOptions>();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });
    }
}