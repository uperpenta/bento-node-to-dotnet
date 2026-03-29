namespace bento_node_to_dotnet.Models;

public class HealthCheckResults
{
    public Guid Id { get; set; }
    public Guid MonitoredServiceId { get; set; }
    public MonitoredService MonitoredService { get; set; } = null!;
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public bool IsHealthy { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}