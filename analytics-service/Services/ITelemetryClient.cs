namespace AnalyticsService.Services;

public interface ITelemetryClient
{
    Task<List<TelemetryReading>> GetTelemetryAsync(string equipmentId);
}
