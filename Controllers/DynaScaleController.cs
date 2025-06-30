using Microsoft.AspNetCore.Mvc;
using Hangfire.DynaScale.Models;
using Hangfire.DynaScale.Services;

namespace Hangfire.DynaScale.Controllers;

[ApiController]
[Route("dynamic-scaling")]
public sealed class DynaScaleController : ControllerBase
{
    private readonly HangfireSettings _hangfireSettings;
    private readonly IHangfireServerManager _serverManager;

    public DynaScaleController(
        HangfireSettings hangfireSettings,
        IHangfireServerManager serverManager)
    {
        _hangfireSettings = hangfireSettings;
        _serverManager = serverManager;
    }

    [HttpGet("queues")]
    public IActionResult GetQueues()
    {
        var queues = _hangfireSettings.Queues
            .SelectMany(q => q.QueueNames.Select(name => new { name, workerCount = q.WorkerCount }))
            .ToList();

        return Ok(queues);
    }

    [HttpPost("queues/{queueName}/set-workers")]
    public async Task<IActionResult> SetWorkers(string queueName, [FromBody] SetWorkersRequest request)
    {
        var queue = _hangfireSettings.Queues.FirstOrDefault(x => x.QueueNames.Contains(queueName));
        if (queue == null)
            return NotFound();

        queue.WorkerCount = request.WorkerCount;
        
        // Server'ı güvenli şekilde yeniden başlat (job'ları beklet)
        await _serverManager.RestartServerAsync(queueName);

        return Ok();
    }
}

public sealed record SetWorkersRequest
{
    public int WorkerCount { get; init; }
} 