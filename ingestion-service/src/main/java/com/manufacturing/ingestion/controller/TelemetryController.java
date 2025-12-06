package com.manufacturing.ingestion.controller;

import com.manufacturing.ingestion.model.TelemetryReading;
import com.manufacturing.ingestion.service.TelemetryService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import jakarta.validation.Valid;
import java.util.List;

/**
 * REST API for IoT telemetry ingestion and retrieval
 * Serves as the HTTP interface between sensors/UI and the telemetry processing system
 */
@RestController
@RequestMapping("/api/telemetry")
@CrossOrigin(origins = "*")
public class TelemetryController {
    
    // Structured logging for production monitoring and debugging
    private static final Logger logger = LoggerFactory.getLogger(TelemetryController.class);

    // Constructor injection over field injection: Enables immutability and easier testing
    private final TelemetryService telemetryService;
    
    public TelemetryController(TelemetryService telemetryService) {
        this.telemetryService = telemetryService;
    }

    /**
     * Primary ingestion endpoint for IoT devices/sensors
     * Uses HTTP POST for idempotent data submission
     */
    @PostMapping
    public ResponseEntity<String> ingestTelemetry(@Valid @RequestBody TelemetryReading reading) {
        // Logging equipment ID for audit trail and debugging in production
        logger.info("Received telemetry data for equipment: {}", reading.getEquipmentId());
        
        try {
            telemetryService.storeTelemetry(reading);
            return ResponseEntity.status(HttpStatus.CREATED)
                .body("Telemetry data received successfully");
        } catch (Exception e) {
            logger.error("Error storing telemetry data", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body("Error processing telemetry data");
        }
    }

    /**
     * Retrieves full historical data for analysis and visualization
     * Used by dashboards requiring complete time-series data
     */
    @GetMapping("/{equipmentId}")
    public ResponseEntity<List<TelemetryReading>> getTelemetryByEquipment(
            @PathVariable String equipmentId) {
        
        // Logging for usage tracking and performance monitoring
        logger.info("Fetching telemetry for equipment: {}", equipmentId);
        
        List<TelemetryReading> readings = telemetryService.getTelemetryByEquipment(equipmentId);
        
        if (readings.isEmpty()) {
            return ResponseEntity.notFound().build();
        }
        
        return ResponseEntity.ok(readings);
    }

    /**
     * Lightweight endpoint for real-time monitoring systems
     * Optimized for frequent polling without heavy data transfer
     */
    @GetMapping("/{equipmentId}/latest")
    public ResponseEntity<TelemetryReading> getLatestTelemetry(@PathVariable String equipmentId) {
        return telemetryService.getLatestTelemetry(equipmentId)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }
}