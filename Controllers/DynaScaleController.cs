using Microsoft.AspNetCore.Mvc;
using Hangfire.DynaScale.Models;
using Hangfire.DynaScale.Services;

namespace Hangfire.DynaScale.Controllers;

[ApiController]
[Route("dynamic-scaling")]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class DynaScaleController : ControllerBase
{
    private readonly IHangfireServerManager _serverManager;

    public DynaScaleController(IHangfireServerManager serverManager)
    {
        _serverManager = serverManager;
    }

    [HttpGet("servers")]
    public IActionResult GetServers()
    {
        var servers = _serverManager.GetServerInfo();
        return Ok(servers);
    }

    [HttpPost("servers/{serverName}/queues/{queueName}/set-workers")]
    public async Task<IActionResult> SetWorkers(string serverName, string queueName, [FromBody] SetWorkersRequest request)
    {
        if (request.ApplyToAllServers)
        {
            await _serverManager.SetWorkerCountForAllServersAsync(queueName, request.WorkerCount);
        }
        else
        {
            await _serverManager.SetWorkerCountAsync(serverName, queueName, request.WorkerCount);
        }

        return Ok();
    }
}