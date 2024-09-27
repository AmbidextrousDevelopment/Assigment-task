using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class VehicleTelemetryManager : MonoBehaviour
{
    #region Main Variables
    [Header("Main Vehicle")]
    [SerializeField] private VehicleBase vehicle; //Main Vehicle object

    //static values to make it Global Accessed (It could be with other method, but I think that one is best here)
    [Header("Main Values")] 
    public static float vehicleSpeed;
    public static float engineRPM;
    public static bool isEngineOn;
    public static int activeGear;
    public static int transmissionMode;
    public static float proximity;
    #endregion


    #region Proximity
    [Header("Proximity Settings")]
    [SerializeField] private GameObject[] otherVehicles;  // List of other vehicles 
    public static float maxProximityDistance = 20f; //min distance, where Proximity starts calculating
    private GameObject nearestCar; //nearest car to Player for Proximity

    private void CalculateProximity() //method to calculate Proximity 
    {
        float nearestDistance = maxProximityDistance;
        nearestCar = null;

        foreach (var otherCar in otherVehicles)
        {
            if (otherCar != null && otherCar != vehicle)
            {
                float distance = Vector3.Distance(vehicle.transform.position, otherCar.transform.position);

                // Check if the car is within the player's field of view (front half of the vehicle)
                Vector3 directionToCar = (otherCar.transform.position - vehicle.transform.position).normalized;
                float angle = Vector3.Angle(vehicle.transform.forward, directionToCar);

                if (distance < nearestDistance && angle < 90)  // 90 degrees is the front field of view
                {
                    nearestDistance = distance;
                    nearestCar = otherCar;
                }
            }
        }

        // Update proximity only if a car was found
        if (nearestCar != null)
        {
            proximity = nearestDistance;
        }
        else
        {
            proximity = maxProximityDistance + 1; // Set to higher than max to indicate no car in proximity
        }
    }
    #endregion
    [Header("CSV Logger")]
    [SerializeField] private CSVLogger csvLogger;
    public KeyCode key = KeyCode.F11; //Key to save Data to .cvs file
    #region CVS Telemetry Data
    private void LogTelemetryData()
    {
        csvLogger.LogData(Time.time, vehicleSpeed, engineRPM, isEngineOn, activeGear, proximity);
    }

    public void ExportTelemetry()
    {
        csvLogger.ExportData();
    }
    #endregion

    #region Unity Functions
    void Update()
    {
        if (vehicle == null || !vehicle.isActiveAndEnabled) return;

        isEngineOn = vehicle.data.Get(Channel.Vehicle, VehicleData.EngineWorking) > 0; // engine on off

        activeGear = vehicle.data.Get(Channel.Vehicle, VehicleData.GearboxGear);  // active gear value

        transmissionMode = vehicle.data.Get(Channel.Vehicle, VehicleData.GearboxMode); //transmission, (not sure that one is correct)


        CalculateProximity(); //calculates proximity

        LogTelemetryData(); //logs Telemetry Data


        if (Input.GetKeyDown(key)) //if key is pressed, data is saved as .cvs
        {
            ExportTelemetry();
        }
    }
    void FixedUpdate() //values that should be taken in FixedUpdate
    {
        if (vehicle == null || !vehicle.isActiveAndEnabled) return;

        vehicleSpeed = vehicle.speed * 3.6f; //Could be also line below, but as I tested, they are similar
        //vehicleSpeed = (vehicle.data.Get(Channel.Vehicle, VehicleData.Speed) / 1000.0f) *3.6f;

        engineRPM = (vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 100000.0f); //engine RPM value
    }
    #endregion
}




