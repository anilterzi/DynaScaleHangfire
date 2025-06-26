using Microsoft.Extensions.DependencyInjection;
using Hangfire.DynaScale.Models;
using Hangfire.DynaScale.Services;

namespace Hangfire.DynaScale.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireDynaScale(this IServiceCollection services, HangfireSettings hangfireSettings)
    {
        services.AddSingleton(hangfireSettings);
        services.AddSingleton<IHangfireServerManager, HangfireServerManager>();
        
        return services;
    }
} 