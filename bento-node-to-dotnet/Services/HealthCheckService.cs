using System.Diagnostics;
using bento_node_to_dotnet.Data;
using bento_node_to_dotnet.Models;
using Microsoft.EntityFrameworkCore;
namespace bento_node_to_dotnet.Services;

public class HealthCheckService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthCheckService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public HealthCheckService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<HealthCheckService> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAllServices();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task CheckAllServices()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        var services = await db.MonitoredServices.ToListAsync();

        var tasks = services.Select(service => CheckService(service, client, db));
        await Task.WhenAll(tasks);

        await db.SaveChangesAsync();
    }

    private async Task CheckService(MonitoredService service, HttpClient client, AppDbContext db)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await client.GetAsync(service.Url);
            sw.Stop();

            service.Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
            service.LastCheckedAt = DateTime.UtcNow;

            db.HealthCheckResults.Add(new HealthCheckResults
            {
                Id = Guid.NewGuid(),
                MonitoredServiceId = service.Id,
                StatusCode = (int)response.StatusCode,
                ResponseTimeMs = sw.ElapsedMilliseconds,
                IsHealthy = response.IsSuccessStatusCode
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            service.Status = "Unhealthy";
            service.LastCheckedAt = DateTime.UtcNow;
            _logger.LogWarning("Health check failed for {Name}: {Error}", service.Name, ex.Message);
        }
    }
}