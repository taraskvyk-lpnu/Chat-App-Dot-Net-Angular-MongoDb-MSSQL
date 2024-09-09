// ValidateTokenAttribute.cs
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Auth.API.Filters;

public class ValidateTokenAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var services = context.HttpContext.RequestServices;
        var configuration = services.GetService<IConfiguration>();

        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        
        if (!IsTokenValid(token, configuration))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsTokenValid(string token, IConfiguration configuration)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["ApiSettings:JwtOptions:Secret"]);
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["ApiSettings:JwtOptions:Issuer"],
                ValidAudience = configuration["ApiSettings:JwtOptions:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
            
            tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            return (securityToken is JwtSecurityToken jwtSecurityToken) && jwtSecurityToken.ValidTo > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }
}