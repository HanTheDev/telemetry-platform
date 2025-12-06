package com.manufacturing.ingestion.controller;

import com.manufacturing.ingestion.model.TelemetryReading;
import com.manufacturing.ingestion.service.TelemetryService;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import java.time.Instant;
import java.util.Arrays;
import java.util.Collections;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

@WebMvcTest(TelemetryController.class)
public class TelemetryControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private ObjectMapper objectMapper;

    @MockBean
    private TelemetryService telemetryService;

    @Test
    public void testIngestTelemetry_Success() throws Exception {
        TelemetryReading reading = new TelemetryReading(
            "CNC-001", 
            Instant.now(), 
            75.5, 
            0.18, 
            102.3
        );

        doNothing().when(telemetryService).storeTelemetry(any(TelemetryReading.class));

        mockMvc.perform(post("/api/telemetry")
                .contentType(MediaType.APPLICATION_JSON)
                .content(objectMapper.writeValueAsString(reading)))
                .andExpect(status().isCreated());
    }

    @Test
    public void testGetTelemetryByEquipment_Found() throws Exception {
        TelemetryReading reading = new TelemetryReading(
            "CNC-001", 
            Instant.now(), 
            75.5, 
            0.18, 
            102.3
        );

        when(telemetryService.getTelemetryByEquipment("CNC-001"))
            .thenReturn(Arrays.asList(reading));

        mockMvc.perform(get("/api/telemetry/CNC-001"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].equipmentId").value("CNC-001"));
    }

    @Test
    public void testGetTelemetryByEquipment_NotFound() throws Exception {
        when(telemetryService.getTelemetryByEquipment("UNKNOWN"))
            .thenReturn(Collections.emptyList());

        mockMvc.perform(get("/api/telemetry/UNKNOWN"))
                .andExpect(status().isNotFound());
    }
}