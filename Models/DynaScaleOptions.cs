namespace Hangfire.DynaScale.Models;

public sealed record DynaScaleOptions
{
    public int MaxWorkerCountPerQueue { get; init; } = 100;
} 