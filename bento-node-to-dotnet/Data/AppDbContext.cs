using bento_node_to_dotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace bento_node_to_dotnet.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<MonitoredService> MonitoredServices => Set<MonitoredService>();
    public DbSet<HealthCheckResults> HealthCheckResults => Set<HealthCheckResults>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HealthCheckResults>()
            .HasOne(h => h.MonitoredService)
            .WithMany()
            .HasForeignKey(h => h.MonitoredServiceId);
    }
}