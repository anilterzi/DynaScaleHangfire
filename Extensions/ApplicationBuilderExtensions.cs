using DynaScaleHangfire.Pages;
using Microsoft.AspNetCore.Builder;
using Hangfire.Dashboard;

namespace Hangfire.DynaScale.Extensions;

public static class ApplicationBuilderExtensions
{

    public static IApplicationBuilder UseHangfireDynaScaleWithStaticFiles(this IApplicationBuilder app)
    {
        // wwwroot dizinini kontrol et ve oluştur
        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        if (!Directory.Exists(wwwrootPath))
        {
            Directory.CreateDirectory(wwwrootPath);
        }

        // Static files middleware'ini ekle
        app.UseStaticFiles();
        
        // DynaScaleController'ı ekle
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "dynascale",
                pattern: "dynamic-scaling/{action=Index}/{id?}",
                defaults: new { controller = "DynaScale" });
        });
        
        // Hangfire özel sayfa ve menü
        DashboardRoutes.Routes.AddRazorPage("/dynamic-scaling", x => new DynamicScalingPage());
        NavigationMenu.Items.Add(page => new MenuItem("Dynamic Scaling", page.Url.To("/dynamic-scaling"))
        {
            Active = page.RequestPath.StartsWith("/dynamic-scaling")
        });
        
        return app;
    }
}

public sealed class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
} 