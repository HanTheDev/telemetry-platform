package com.manufacturing.ingestion.model;

import java.time.Instant;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

public class TelemetryReading {
    
    @NotBlank(message = "Equipment ID is required")
    private String equipmentId;

    @NotNull(message = "Timestamp is required")
    private Instant timestamp;

    @NotNull(message = "Temperature is required")
    private Double temperature;

    @NotNull(message = "Vibration is required")
    private Double vibration;

    @NotNull(message = "Pressure is required")
    private Double pressure;

    public TelemetryReading() {}

    public TelemetryReading(String equipmentId, Instant timestamp, 
        Double temperature, Double vibration, Double pressure) {
            this.equipmentId = equipmentId;
            this.timestamp = timestamp;
            this.temperature = temperature;
            this.vibration = vibration;
            this.pressure = pressure;
        }

    // Getter and Setter
    public String getEquipmentId() { return equipmentId; }
    public void setEquipmentId(String equipmentId){
        this.equipmentId = equipmentId;
    }

    public Instant getTimestamp() { return timestamp; }
    public void setTimestamp(Instant timestamp){
        this.timestamp = timestamp;
    }

    public Double getTemperature() { return temperature; }
    public void setTemperature(Double temperature){
        this.temperature = temperature;
    }

    public Double getVibration() { return vibration; }
    public void setVibration(Double vibration) {
        this.vibration = vibration;
    }

    public Double getPressure() { return pressure; }
    public void setPressure(Double pressure) {
        this.pressure = pressure;
    }
}
