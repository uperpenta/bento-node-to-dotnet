namespace bento_node_to_dotnet.Models;

public class MonitoredService
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastCheckedAt { get; set; }
}