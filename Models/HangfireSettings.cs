namespace Hangfire.DynaScale.Models;

public sealed record HangfireSettings
{
    public string RedisConnectionString { get; init; } = string.Empty;
    
    public int RedisDbId { get; init; }
    
    public string RedisPrefix { get; init; } = string.Empty;
    
    public int AttemptCount { get; init; }
    
    public int[] DelayInSeconds { get; init; } = Array.Empty<int>();
    
    public int SuccessJobExpirationSeconds { get; init; }
    
    public int FailedJobExpirationSeconds { get; init; }

    public bool ImmediateRetry { get; init; }
    
    public List<QueueInformation> Queues { get; init; } = new();
    
    public bool Enabled { get; init; }
}

public sealed record QueueInformation
{
    public List<string> QueueNames { get; init; } = new();
    
    public int WorkerCount { get; set; }
} 