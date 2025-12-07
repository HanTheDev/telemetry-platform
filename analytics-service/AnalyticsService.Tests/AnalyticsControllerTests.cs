using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AnalyticsService.Controllers;
using AnalyticsService.Models;
using AnalyticsService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Tests;

public class AnalyticsControllerTests
{
    private readonly Mock<ILogger<AnalyticsController>> _loggerMock;
    private readonly Mock<ITelemetryClient> _telemetryClientMock;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTests()
    {
        _loggerMock = new Mock<ILogger<AnalyticsController>>();
        _telemetryClientMock = new Mock<ITelemetryClient>();
        _controller = new AnalyticsController(_loggerMock.Object, _telemetryClientMock.Object);
    }

    [Fact]
    public async Task GetAnalytics_ReturnsOk_WhenDataExists()
    {
        // Arrange
        var equipmentId = "CNC-001";
        var readings = new List<TelemetryReading>
        {
            new TelemetryReading { EquipmentId = equipmentId, Temperature = 70.0, Vibration = 0.1, Pressure = 100.0, Timestamp = DateTime.UtcNow },
            new TelemetryReading { EquipmentId = equipmentId, Temperature = 75.0, Vibration = 0.2, Pressure = 102.0, Timestamp = DateTime.UtcNow }
        };

        _telemetryClientMock
            .Setup(x => x.GetTelemetryAsync(equipmentId))
            .ReturnsAsync(readings);

        // Act
        var result = await _controller.GetAnalytics(equipmentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var analytics = Assert.IsType<AnalyticsResult>(okResult.Value);
        Assert.Equal(equipmentId, analytics.EquipmentId);
        Assert.Equal(2, analytics.ReadingCount);
        Assert.Equal(72.5, analytics.AverageTemperature);
    }

    [Fact]
    public async Task GetAnalytics_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        var equipmentId = "UNKNOWN";
        _telemetryClientMock
            .Setup(x => x.GetTelemetryAsync(equipmentId))
            .ReturnsAsync(new List<TelemetryReading>());

        // Act
        var result = await _controller.GetAnalytics(equipmentId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}