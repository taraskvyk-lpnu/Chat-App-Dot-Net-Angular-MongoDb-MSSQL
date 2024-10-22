using ChatManagement.Infrastructure.MappingProfilers;
using Microsoft.Extensions.DependencyInjection;

namespace ChatManagement.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ChatProfile));
    }
}