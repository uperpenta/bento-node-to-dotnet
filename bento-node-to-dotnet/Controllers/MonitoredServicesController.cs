using bento_node_to_dotnet.Data;
using bento_node_to_dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bento_node_to_dotnet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonitoredServicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<MonitoredServicesController> _logger;

    public MonitoredServicesController(AppDbContext db, ILogger<MonitoredServicesController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var services = await _db.MonitoredServices.ToListAsync();

        if (services.Count == 0) return NotFound("No monitored services found");

        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MonitoredService service)
    {
        service.Id = Guid.NewGuid();
        _db.MonitoredServices.Add(service);
        await _db.SaveChangesAsync();
        _logger.LogInformation("New monitored service created: {Name} | URL: {Url} | Status: {Status} | ID: {Id}",
            service.Name, service.Url, service.Status, service.Id);
        return CreatedAtAction(nameof(GetAll), new { id = service.Id }, service);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var service = await _db.MonitoredServices.FindAsync(id);
        if (service is null) return NotFound();
        _db.MonitoredServices.Remove(service);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}