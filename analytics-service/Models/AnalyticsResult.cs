namespace AnalyticsService.Models;

public class AnalyticsResult
{
    public string EquipmentId { get; set; } = string.Empty;
    public int ReadingCount { get; set; }
    public double AverageTemperature { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
    public double AverageVibration { get; set; }
    public double MaxVibration { get; set; }
    public double AveragePressure { get; set; }
    public TelemetryReading? LastReading { get; set; }
}