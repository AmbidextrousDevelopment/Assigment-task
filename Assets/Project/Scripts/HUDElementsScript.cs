using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDElementsScript : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rpmText;
    [SerializeField] private TextMeshProUGUI gearText;
    [SerializeField] private TextMeshProUGUI isEngineOnText;
    [SerializeField] private TextMeshProUGUI transmissionModeText;
    [SerializeField] private TextMeshProUGUI proximityText;
    public void UpdateHUD() //static Values from script "VehicleTelemetryManager"
    {
        speedText.text = $"Speed: {VehicleTelemetryManager.vehicleSpeed:F1} km/h"; //Speed in km/h text

        rpmText.text = $"RPM: {VehicleTelemetryManager.engineRPM:F1}"; //RPM speed text

        gearText.text = $"Gear: {VehicleTelemetryManager.activeGear}"; //Gear value text

        // Engine on/off text
        isEngineOnText.text = VehicleTelemetryManager.isEngineOn ? "Engine: On" : "Engine: Off";

        // Transmission mode text
        transmissionModeText.text = VehicleTelemetryManager.activeGear == 0 ? "Mode: N" :
                                    VehicleTelemetryManager.activeGear > 0 ? "Mode: D" : "Mode: R";

        // Proximity text
        if (VehicleTelemetryManager.proximity <= VehicleTelemetryManager.maxProximityDistance)
            proximityText.text = $"Proximity: {VehicleTelemetryManager.proximity:F1} m";
        else
            proximityText.text = "No car within proximity";
    }
    private void Update()
    {
        UpdateHUD();
    }
}

