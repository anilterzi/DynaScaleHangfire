namespace Hangfire.DynaScale.Models;

public sealed record SetWorkersRequest
{
    public int WorkerCount { get; init; }
    public bool ApplyToAllServers { get; init; }
} 