namespace Hangfire.DynaScale.Models;

public sealed record ServerInfo
{
    public string ServerName { get; init; } = string.Empty;
    
    public bool IsActive { get; init; }
    
    public DateTime LastHeartbeat { get; init; }
    
    public List<QueueInfo> Queues { get; init; } = new();
}

public sealed record QueueInfo
{
    public string ServerName { get; init; } = string.Empty;

    public string QueueName { get; init; } = string.Empty;
    
    public int CurrentWorkerCount { get; init; }
    
    public int MaxWorkerCount { get; init; }
}