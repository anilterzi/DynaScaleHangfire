using System.Diagnostics;
using Hangfire.DynaScale.Models;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;

namespace Hangfire.DynaScale.Services;

public sealed class HangfireServerManager : IHangfireServerManager
{
    private readonly JobStorage _jobStorage;
    private readonly DynaScaleOptions _options;

    public HangfireServerManager(DynaScaleOptions options)
    {
        _jobStorage = JobStorage.Current;
        _options = options;
    }

    public List<ServerInfo> GetServerInfo()
    {
        var serverInfos = new List<ServerInfo>();
        var servers = _jobStorage.GetMonitoringApi().Servers();
        
        var machineGroups = servers
            .GroupBy(server => ExtractMachineName(server.Name))
            .ToList();
        
        foreach (var machineGroup in machineGroups)
        {
            var machineName = machineGroup.Key;
            var machineServers = machineGroup.ToList();
            
            var activeServers = machineServers.Where(s => s.Heartbeat > DateTime.UtcNow.AddMinutes(-1)).ToList();
            
            if (activeServers.Any())
            {
                var serverInfo = new ServerInfo
                {
                    ServerName = machineName,
                    IsActive = true,
                    LastHeartbeat = activeServers.Max(s => s.Heartbeat ?? DateTime.MinValue),
                    Queues = GetMachineQueues(activeServers)
                };
                
                serverInfos.Add(serverInfo);
            }
        }
        
        return serverInfos;
    }

    public async Task SetWorkerCountAsync(string serverName, string queueName, int workerCount)
    {
        PauseServerJobProcessing(serverName);
        await WaitForProcessingJobsToComplete(serverName);
        var servers = _jobStorage.GetMonitoringApi().Servers();
        var targetServer = servers.FirstOrDefault(s => s.Name == serverName);
        var currentQueues = targetServer?.Queues?.ToList() ?? new List<string>{queueName};
        RestartServerWithNewWorkerCount(serverName, workerCount, currentQueues);
    }

    public async Task SetWorkerCountForAllServersAsync(string queueName, int workerCount)
    {
        var servers = _jobStorage.GetMonitoringApi().Servers();
        var targetServers = servers.Where(s => s.Queues?.Contains(queueName) == true);
        var allTasks = targetServers.Select(server => SetWorkerCountAsync(server.Name, queueName, workerCount));
        await Task.WhenAll(allTasks);
    }

    private async Task WaitForProcessingJobsToComplete(string serverName)
    {
        var maxWaitTime = TimeSpan.FromMinutes(5);
        var checkInterval = TimeSpan.FromSeconds(1);
        var elapsed = TimeSpan.Zero;
        while (elapsed < maxWaitTime)
        {
            if (!HasProcessingJobs(serverName))
                return;
            await Task.Delay(checkInterval);
            elapsed += checkInterval;
        }
    }

    private bool HasProcessingJobs(string serverName)
    {
        var processingJobs = _jobStorage.GetMonitoringApi().ProcessingJobs(0, int.MaxValue);
        return processingJobs
            .Where(j => j.Value.ServerId == serverName)
            .Any();
    }

    private string ExtractMachineName(string serverName)
    {
        var parts = serverName.Split(':');
        return parts.Length > 0 ? parts[0] : serverName;
    }

    private List<QueueInfo> GetMachineQueues(List<ServerDto> machineServers)
    {
        var queueInfos = new List<QueueInfo>();
        var allQueues = new HashSet<string>();
        
        foreach (var server in machineServers)
        {
            var queues = server.Queues ?? Array.Empty<string>();
            foreach (var queue in queues)
            {
                allQueues.Add(queue);
            }
        }
        
        foreach (var queueName in allQueues)
        {
            var serversForQueue = machineServers.Where(s => s.Queues?.Contains(queueName) == true).ToList();
            
            foreach (var server in serversForQueue)
            {
                var queueInfo = new QueueInfo
                {
                    ServerName = server.Name,
                    QueueName = queueName,
                    CurrentWorkerCount = server.WorkersCount,
                    MaxWorkerCount = _options.MaxWorkerCountPerQueue
                };
                queueInfos.Add(queueInfo);
            }
        }
        
        return queueInfos;
    }

    private void RestartServerWithNewWorkerCount(string serverName, int workerCount, List<string> currentQueues)
    {
        var connection = _jobStorage.GetConnection();
        connection.RemoveServer(serverName);

        // Queue listesi boşsa default queue kullan
        var queues = currentQueues.Any() ? currentQueues.ToArray() : new[] { "default" };
        
        var options = new BackgroundJobServerOptions
        {
            Queues = queues,
            WorkerCount = workerCount
        };

        new BackgroundJobServer(options, _jobStorage);
    }

    private void PauseServerJobProcessing(string serverName)
    {
        var connection = _jobStorage.GetConnection();
        var servers = _jobStorage.GetMonitoringApi().Servers();
        var targetServer = servers.FirstOrDefault(s => s.Name == serverName);
        
        if (targetServer?.Queues != null)
        {
            foreach (var queue in targetServer.Queues)
            {
                connection.SetRangeInHash($"hangfire:queue:{queue}:paused", new Dictionary<string, string>
                {
                    ["paused"] = "true",
                    ["pausedAt"] = DateTime.UtcNow.ToString("O")
                });
            }
        }
    }
}