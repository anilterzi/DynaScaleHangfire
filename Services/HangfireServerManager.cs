using Hangfire.DynaScale.Models;

namespace Hangfire.DynaScale.Services;

public sealed class HangfireServerManager : IHangfireServerManager
{
    private readonly HangfireSettings _hangfireSettings;
    private readonly Dictionary<string, BackgroundJobServer> _servers;

    public HangfireServerManager(HangfireSettings hangfireSettings)
    {
        _hangfireSettings = hangfireSettings;
        _servers = new Dictionary<string, BackgroundJobServer>();
    }

    public void RestartServer(string? queueName = null)
    {
        if (queueName != null)
        {
            // Sadece belirtilen queue için server'ı yeniden başlat
            RestartSpecificQueue(queueName);
        }
        else
        {
            // Tüm queue'lar için server'ları yeniden başlat
            RestartAllQueues();
        }
    }

    public async Task RestartServerAsync(string? queueName = null)
    {
        if (queueName != null)
        {
            // Sadece belirtilen queue için server'ı yeniden başlat
            await RestartSpecificQueueAsync(queueName);
        }
        else
        {
            // Tüm queue'lar için server'ları yeniden başlat
            await RestartAllQueuesAsync();
        }
    }

    private void RestartSpecificQueue(string queueName)
    {
        var queueConfig = _hangfireSettings.Queues
            .FirstOrDefault(x => x.QueueNames.Contains(queueName));

        if (queueConfig == null || queueConfig.WorkerCount <= 0)
        {
            // Queue bulunamadı veya worker count 0, server'ı durdur
            StopQueueServer(queueName);
            return;
        }

        // Eski server'ı güvenli şekilde durdur (job'ları beklet)
        StopQueueServerSafely(queueName);

        // Yeni server'ı başlat
        var options = new BackgroundJobServerOptions
        {
            Queues = queueConfig.QueueNames.ToArray(),
            WorkerCount = queueConfig.WorkerCount
        };

        var server = new BackgroundJobServer(options);
        _servers[queueName] = server;
    }

    private async Task RestartSpecificQueueAsync(string queueName)
    {
        var queueConfig = _hangfireSettings.Queues
            .FirstOrDefault(x => x.QueueNames.Contains(queueName));

        if (queueConfig == null || queueConfig.WorkerCount <= 0)
        {
            // Queue bulunamadı veya worker count 0, server'ı durdur
            await StopQueueServerAsync(queueName);
            return;
        }

        // Eski server'ı güvenli şekilde durdur (job'ları beklet)
        await StopQueueServerSafelyAsync(queueName);

        // Yeni server'ı başlat
        var options = new BackgroundJobServerOptions
        {
            Queues = queueConfig.QueueNames.ToArray(),
            WorkerCount = queueConfig.WorkerCount
        };

        var server = new BackgroundJobServer(options);
        _servers[queueName] = server;
    }

    private void RestartAllQueues()
    {
        // Tüm mevcut server'ları güvenli şekilde durdur
        foreach (var kvp in _servers)
        {
            StopServerSafely(kvp.Value, kvp.Key);
        }
        _servers.Clear();

        // Her queue için ayrı server başlat
        foreach (var queueConfig in _hangfireSettings.Queues.Where(x => x.WorkerCount > 0))
        {
            foreach (var queueName in queueConfig.QueueNames)
            {
                var options = new BackgroundJobServerOptions
                {
                    Queues = queueConfig.QueueNames.ToArray(),
                    WorkerCount = queueConfig.WorkerCount
                };

                var server = new BackgroundJobServer(options);
                _servers[queueName] = server;
            }
        }
    }

    private async Task RestartAllQueuesAsync()
    {
        // Tüm mevcut server'ları güvenli şekilde durdur
        var stopTasks = _servers.Select(kvp => StopServerSafelyAsync(kvp.Value, kvp.Key)).ToArray();
        await Task.WhenAll(stopTasks);
        _servers.Clear();

        // Her queue için ayrı server başlat
        foreach (var queueConfig in _hangfireSettings.Queues.Where(x => x.WorkerCount > 0))
        {
            foreach (var queueName in queueConfig.QueueNames)
            {
                var options = new BackgroundJobServerOptions
                {
                    Queues = queueConfig.QueueNames.ToArray(),
                    WorkerCount = queueConfig.WorkerCount
                };

                var server = new BackgroundJobServer(options);
                _servers[queueName] = server;
            }
        }
    }

    private void StopQueueServer(string queueName)
    {
        if (_servers.TryGetValue(queueName, out var server))
        {
            StopServerSafely(server, queueName);
            _servers.Remove(queueName);
        }
    }

    private async Task StopQueueServerAsync(string queueName)
    {
        if (_servers.TryGetValue(queueName, out var server))
        {
            await StopServerSafelyAsync(server, queueName);
            _servers.Remove(queueName);
        }
    }

    private void StopQueueServerSafely(string queueName)
    {
        if (_servers.TryGetValue(queueName, out var server))
        {
            StopServerSafely(server, queueName);
            _servers.Remove(queueName);
        }
    }

    private async Task StopQueueServerSafelyAsync(string queueName)
    {
        if (_servers.TryGetValue(queueName, out var server))
        {
            await StopServerSafelyAsync(server, queueName);
            _servers.Remove(queueName);
        }
    }

    private void StopServerSafely(BackgroundJobServer server, string? queueName = null)
    {
        if (server == null) return;

        try
        {
            // Server'ı durdur ama mevcut job'ların tamamlanmasını bekle
            server.SendStop();
            
            // Belirtilen queue'da processing job var mı kontrol et ve bekle
            WaitForProcessingJobsToComplete(queueName);
        }
        catch
        {
            // Hata durumunda zorla durdur
        }
        finally
        {
            // Her durumda server'ı dispose et
            server.Dispose();
        }
    }

    private async Task StopServerSafelyAsync(BackgroundJobServer server, string? queueName = null)
    {
        if (server == null) return;

        try
        {
            // Server'ı durdur ama mevcut job'ların tamamlanmasını bekle
            server.SendStop();
            
            // Belirtilen queue'da processing job var mı kontrol et ve bekle
            await WaitForProcessingJobsToCompleteAsync(queueName);
        }
        catch
        {
            // Hata durumunda zorla durdur
        }
        finally
        {
            // Her durumda server'ı dispose et
            server.Dispose();
        }
    }

    private void WaitForProcessingJobsToComplete(string? queueName = null)
    {
        var maxWaitTime = System.TimeSpan.FromMinutes(5); // Maksimum 5 dakika bekle
        var checkInterval = System.TimeSpan.FromSeconds(1); // Her saniye kontrol et
        var elapsed = System.TimeSpan.Zero;

        while (elapsed < maxWaitTime)
        {
            // Belirtilen queue'da processing job var mı kontrol et
            if (!HasProcessingJobs(queueName))
            {
                return; // Job yoksa hemen çık
            }

            System.Threading.Thread.Sleep(checkInterval);
            elapsed += checkInterval;
        }

        // Timeout olursa log yazabilirsin
        // Console.WriteLine($"Timeout waiting for processing jobs to complete in queue: {queueName}");
    }

    private async Task WaitForProcessingJobsToCompleteAsync(string? queueName = null)
    {
        var maxWaitTime = System.TimeSpan.FromMinutes(5); // Maksimum 5 dakika bekle
        var checkInterval = System.TimeSpan.FromSeconds(1); // Her saniye kontrol et
        var elapsed = System.TimeSpan.Zero;

        while (elapsed < maxWaitTime)
        {
            // Belirtilen queue'da processing job var mı kontrol et
            if (!HasProcessingJobs(queueName))
            {
                return; // Job yoksa hemen çık
            }

            await Task.Delay(checkInterval);
            elapsed += checkInterval;
        }

        // Timeout olursa log yazabilirsin
        // Console.WriteLine($"Timeout waiting for processing jobs to complete in queue: {queueName}");
    }

    private bool HasProcessingJobs(string? queueName = null)
    {
        try
        {
            // Hangfire'ın processing job'larını kontrol et
            var processingJobs = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, int.MaxValue);
            
            if (queueName == null)
            {
                // Tüm processing job'ları kontrol et
                return processingJobs.Any();
            }
            else
            {
                // Sadece belirtilen queue'daki processing job'ları kontrol et
                return processingJobs.Any(job => 
                {
                    try
                    {
                        // Job'ın queue bilgisini al
                        var jobDetails = JobStorage.Current.GetMonitoringApi().JobDetails(job.Key);
                        if (jobDetails != null && jobDetails.Properties.ContainsKey("Queue"))
                        {
                            var jobQueue = jobDetails.Properties["Queue"];
                            return jobQueue == queueName;
                        }
                        return false;
                    }
                    catch
                    {
                        // Job detayları alınamazsa false döndür
                        return false;
                    }
                });
            }
        }
        catch
        {
            // Hata durumunda false döndür (güvenli varsayım)
            return false;
        }
    }
} 