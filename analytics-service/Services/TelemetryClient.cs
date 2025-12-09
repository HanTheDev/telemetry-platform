using System.Text.Json;
using AnalyticsService.Models;

namespace AnalyticsService.Services;

public class TelemetryClient : ITelemetryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelemetryClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public TelemetryClient(HttpClient httpClient, ILogger<TelemetryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<TelemetryReading>> GetTelemetryAsync(string equipmentId)
    {
        _logger.LogInformation("Fetching telemetry from ingestion service for: {EquipmentId}", equipmentId);

        try
        {
            var response = await _httpClient.GetAsync($"/api/telemetry/{equipmentId}");
            
            // If 404, return empty list (not an error)
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("No data found for equipment: {EquipmentId}", equipmentId);
                return new List<TelemetryReading>();
            }
            
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var readings = JsonSerializer.Deserialize<List<TelemetryReading>>(content, _jsonOptions);

            return readings ?? new List<TelemetryReading>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error connecting to ingestion service");
            throw; // This will trigger 503 in controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching telemetry data");
            throw;
        }
    }
}