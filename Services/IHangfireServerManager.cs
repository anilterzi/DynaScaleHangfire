namespace Hangfire.DynaScale.Services;

public interface IHangfireServerManager
{
    void RestartServer(string? queueName = null);
    
    /// <summary>
    /// Restarts the server asynchronously, waiting for current jobs to complete
    /// </summary>
    /// <param name="queueName">Specific queue name to restart, or null for all queues</param>
    /// <returns>Task that completes when the server restart is finished</returns>
    Task RestartServerAsync(string? queueName = null);
} 