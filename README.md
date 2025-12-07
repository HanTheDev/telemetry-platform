# Manufacturing Telemetry Platform

## Overview
A dual-service microservices platform for ingesting and analyzing equipment telemetry data in manufacturing environments. Built with Spring Boot (Java) and .NET Core (C#).

## Architecture
- **Ingestion Service** (Spring Boot): Receives and stores telemetry data via REST API
- **Analytics Service** (.NET Core): Fetches data, calculates metrics, provides dashboard UI

## Prerequisites
- Docker and Docker Compose installed
- (Optional) Java 17+ and .NET 8 SDK for local development

## Quick Start
```bash
# Clone repository
git clone https://github.com/yourusername/telemetry-platform.git
cd telemetry-platform

# Start services
docker-compose up --build

# Access services
# Ingestion API: http://localhost:8080
# Analytics API: http://localhost:5000
```

## API Contracts

### POST /api/telemetry (Ingestion Service)
```json
{
  "equipmentId": "CNC-001",
  "timestamp": "2024-12-06T10:30:00Z",
  "temperature": 72.5,
  "vibration": 0.15,
  "pressure": 100.2
}
```
Response: 201 Created

### GET /api/analytics/{equipmentId} (Analytics Service)
Response:
```json
{
  "equipmentId": "CNC-001",
  "readingCount": 150,
  "averageTemperature": 72.1,
  "maxVibration": 0.20,
  "lastReading": {...}
}
```

## Testing
```bash
# Run Spring Boot tests
cd ingestion-service && mvn test

# Run .NET tests
cd analytics-service && dotnet test
```

## Demo Commands
```bash
# Start services
docker-compose up -d

# POST sample data
curl -X POST http://localhost:8081/api/telemetry \
  -H "Content-Type: application/json" \
  -d '{"equipmentId":"CNC-001","timestamp":"2024-12-06T14:30:00Z","temperature":75.5,"vibration":0.18,"pressure":102.3}'

# Get analytics
curl http://localhost:5272/api/analytics/CNC-001

# Check health
curl http://localhost:8081/actuator/health
curl http://localhost:5272/health
```

## Future Enhancements
- Add persistent storage (PostgreSQL)
- Implement message queue (Kafka)
- Add Grafana dashboards
- Implement JWT authentication