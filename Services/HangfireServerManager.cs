using Hangfire;
using Hangfire.DynaScale.Models;
using System.Linq;

namespace Hangfire.DynaScale.Services;

public sealed class HangfireServerManager : IHangfireServerManager
{
    private readonly HangfireSettings _hangfireSettings;
    private BackgroundJobServer? _server;

    public HangfireServerManager(HangfireSettings hangfireSettings)
    {
        _hangfireSettings = hangfireSettings;
    }

    public void RestartServer(string? queueName = null)
    {
        var options = _hangfireSettings.Queues
            .Where(x => x.WorkerCount > 0 && (queueName == null || x.QueueNames.Contains(queueName)))
            .Select(x => new BackgroundJobServerOptions
            {
                Queues = x.QueueNames.Distinct().ToArray(),
                WorkerCount = x.WorkerCount
            })
            .ToList();

        if (!options.Any())
            return;

        // Eski server'ı durdur
        _server?.Dispose();

        // Yeni server'ı başlat
        _server = new BackgroundJobServer(options.First());
    }
} 