using Microsoft.Extensions.DependencyInjection;
using Hangfire.DynaScale.Services;
using Hangfire.DynaScale.Models;
using Hangfire.Storage;

namespace Hangfire.DynaScale.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireDynaScale(this IServiceCollection services, DynaScaleOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<IHangfireServerManager, HangfireServerManager>();
        
        return services;
    }
    
    public static IServiceCollection AddHangfireDynaScale(this IServiceCollection services)
    {
        var defaultOptions = new DynaScaleOptions();
        return AddHangfireDynaScale(services, defaultOptions);
    }
} 