package com.manufacturing.ingestion.service;

import com.manufacturing.ingestion.model.TelemetryReading;
import org.springframework.stereotype.Service;

import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.stream.Collectors;

@Service
public class TelemetryService {

    // Using in-memory storage for rapid prototyping - will be replaced with database in production
    private final Map<String, List<TelemetryReading>> telemetryStore = new ConcurrentHashMap<>();

    // Prevent memory overflow by implementing a simple circular buffer per equipment
    // Production: Consider time-based retention or database archiving instead
    private static final int MAX_READINGS_PER_EQUIPMENT = 1000;

    /**
     * Ingests telemetry data from IoT devices/equipment
     * Thread-safe design supports concurrent writes from multiple sensors
     */
    public void storeTelemetry(TelemetryReading reading){
        // Concurrent-safe creation/retrieval: ensures thread safety during equipment initialization
        /** 
         * check if equipment ID is exists on the map
         * if not create a synchronized array list
         * add the reading to the equipments' list
         */ 
        telemetryStore.computeIfAbsent(reading.getEquipmentId(), 
        k -> Collections.synchronizedList(new ArrayList<>()))
        .add(reading);

        // Limit storage size
        // Eviction policy: Maintain sliding window of recent data
        List<TelemetryReading> readings = telemetryStore.get(reading.getEquipmentId());

        if(readings.size() > MAX_READINGS_PER_EQUIPMENT){
            readings.remove(0);
        }
    }

     /**
     * Retrieves historical data for analysis/dashboarding
     * Sorted descending for UI convenience (most recent first)
     */
    public List<TelemetryReading> getTelemetryByEquipment(String equipmentId) {
        return telemetryStore.getOrDefault(equipmentId, Collections.emptyList())
        .stream()
        // Reverse chronological for most common use case (monitoring current state)
        .sorted(Comparator.comparing(TelemetryReading::getTimestamp).reversed())
        .collect(Collectors.toList());
    }


    /**
     * API for real-time monitoring systems needing only the latest reading
     */
    public Optional<TelemetryReading> getLatestTelemetry(String equipmentId) {
        return getTelemetryByEquipment(equipmentId).stream().findFirst();
    }

     /**
     * Supports dashboard widgets showing recent trend data
     * Used by UI components that display last N readings
     */
    public List<TelemetryReading> getRecentReadings(String equipmentId, int limit) {
        return getTelemetryByEquipment(equipmentId).stream()
        .limit(limit)
        .collect(Collectors.toList());
    }
}