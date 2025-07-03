using Hangfire.DynaScale.Models;

namespace Hangfire.DynaScale.Services;

public interface IHangfireServerManager
{
    List<ServerInfo> GetServerInfo();
    
    Task SetWorkerCountAsync(string serverName, string queueName, int workerCount);
    
    Task SetWorkerCountForAllServersAsync(string queueName, int workerCount);
} 