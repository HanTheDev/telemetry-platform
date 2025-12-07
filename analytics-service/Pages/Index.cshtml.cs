using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnalyticsService.Models;
using AnalyticsService.Services;

namespace AnalyticsService.Pages;

public class IndexModel : PageModel
{
    private readonly ITelemetryClient _telemetryClient;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITelemetryClient telemetryClient, ILogger<IndexModel> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public AnalyticsResult? Analytics { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task OnPostAsync(string equipmentId)
    {
        try
        {
            var readings = await _telemetryClient.GetTelemetryAsync(equipmentId);

            if (!readings.Any())
            {
                ErrorMessage = $"No data found for equipment {equipmentId}";
                return;
            }

            Analytics = new AnalyticsResult
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching analytics");
            ErrorMessage = "Error fetching data from ingestion service. Make sure it's running.";
        }
    }
}