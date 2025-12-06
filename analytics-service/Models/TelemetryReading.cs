namespace AnalyticsService.Models;

public class TelemetryReading
{
    public string EquipmentId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double Temperature { get; set; }
    public double Vibration { get; set; }
    public double Pressure { get; set; }
}