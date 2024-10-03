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

    [Header("Front Bumper Points")]
    [SerializeField] private Transform leftBumper;  // Empty object on left bumper part
    [SerializeField] private Transform rightBumper; // Empty object on right bumper part

    private void CalculateProximity()
    {
        RaycastHit hitLeft, hitRight;
        nearestCar = null;
        float nearestDistance = maxProximityDistance;

        Debug.DrawRay(leftBumper.position, leftBumper.forward * maxProximityDistance, Color.green);  
        Debug.DrawRay(rightBumper.position, rightBumper.forward * maxProximityDistance, Color.green); 

       
        bool hitFromLeft = Physics.Raycast(leftBumper.position, leftBumper.forward, out hitLeft, maxProximityDistance);

        
        bool hitFromRight = Physics.Raycast(rightBumper.position, rightBumper.forward, out hitRight, maxProximityDistance);


        if (hitFromLeft || hitFromRight)
        {
            foreach (var otherCar in otherVehicles)
            {
                if (hitFromLeft && hitLeft.collider != null && hitLeft.collider.transform == otherCar.transform)
                {
                    float distance = hitLeft.distance;
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestCar = otherCar;
                    }
                }

                if (hitFromRight && hitRight.collider != null && hitRight.collider.transform == otherCar.transform)
                {
                    float distance = hitRight.distance;
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestCar = otherCar;
                    }
                }
            }
        }

        //  proximity Update
        if (nearestCar != null)
        {
            proximity = nearestDistance;
        }
        else
        {
            proximity = maxProximityDistance + 1; // Якщо не знайдено жодного автомобіля попереду
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

        engineRPM = (vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000.0f); //engine RPM value
    }
    #endregion
}




