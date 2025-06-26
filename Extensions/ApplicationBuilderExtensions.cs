using Microsoft.AspNetCore.Builder;
using Hangfire.DynaScale.Controllers;

namespace Hangfire.DynaScale.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseHangfireDynaScale(this IApplicationBuilder app)
    {
        // DynaScaleController'ı otomatik olarak route'lara ekle
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "dynascale",
                pattern: "dynamic-scaling/{action=Index}/{id?}",
                defaults: new { controller = "DynaScale" });
        });
        
        return app;
    }
} 