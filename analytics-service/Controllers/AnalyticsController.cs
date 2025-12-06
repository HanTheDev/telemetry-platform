using Microsoft.AspNetCore.Mvc;
using AnalyticsService.Models;
using AnalyticsService.Services;

namespace AnalyticsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly ILogger<AnalyticsController> _logger;
    private readonly ITelemetryClient _telemetryClient;

    public AnalyticsController(ILogger<AnalyticsController> logger, 
                              ITelemetryClient telemetryClient)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    [HttpGet("{equipmentId}")]
    public async Task<ActionResult<AnalyticsResult>> GetAnalytics(string equipmentId)
    {
        _logger.LogInformation("Fetching analytics for equipment: {EquipmentId}", equipmentId);

        try
        {
            var readings = await _telemetryClient.GetTelemetryAsync(equipmentId);

            if (!readings.Any())
            {
                _logger.LogWarning("No telemetry data found for equipment: {EquipmentId}", equipmentId);
                return NotFound($"No data found for equipment {equipmentId}");
            }

            var analytics = new AnalyticsResult
            {
                EquipmentId = equipmentId,
                ReadingCount = readings.Count,
                AverageTemperature = readings.Average(r => r.Temperature),
                MinTemperature = readings.Min(r => r.Temperature),
                MaxTemperature = readings.Max(r => r.Temperature),
                AverageVibration = readings.Average(r => r.Vibration),
                MaxVibration = readings.Max(r => r.Vibration),
                AveragePressure = readings.Average(r => r.Pressure),
                LastReading = readings.OrderByDescending(r => r.Timestamp).First()
            };

            return Ok(analytics);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching telemetry data from ingestion service");
            return StatusCode(503, "Ingestion service unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating analytics");
            return StatusCode(500, "Internal server error");
        }
    }
}