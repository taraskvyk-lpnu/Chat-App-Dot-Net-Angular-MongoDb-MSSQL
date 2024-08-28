using System.Text;
using ChatManagement.API.Extensions;
using ChatManagement.DataAccess;
using ChatManagement.Domain;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Repositories;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.Middlewares;
using ChatManagement.Infrastructure.Repositories;
using ChatManagement.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
        app.ApplyMigrations();
        app.Run();
    }
}