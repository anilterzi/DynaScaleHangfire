namespace Hangfire.DynaScale.Services;

public interface IHangfireServerManager
{
    void RestartServer(string? queueName = null);
} 