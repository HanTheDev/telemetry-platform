using AnalyticsService.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HttpClient for telemetry service
builder.Services.AddHttpClient<ITelemetryClient, TelemetryClient>(client =>
{
    var baseUrl = builder.Configuration["TelemetryService:BaseUrl"] ?? "http://localhost:8082";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add health checks
builder.Services.AddHealthChecks();

// Add Razor Pages for UI
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHealthChecks("/health");

app.Run();