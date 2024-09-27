using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CSVLogger : MonoBehaviour //Class to save data in .csv
{
    private List<string> logData = new List<string>();
    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "TelemetryData.csv");
        WriteHeader();
    }

    private void WriteHeader()
    {
        string header = "Time,Speed,RPM,EngineOn,Gear,Proximity";
        logData.Add(header);
    }

    public void LogData(float time, float speed, float rpm, bool isEngineOn, int gear, float proximity)
    {
        string dataLine = $"{time:F2},{speed:F1},{rpm:F1},{isEngineOn},{gear},{proximity:F1}";
        logData.Add(dataLine);
    }

    public void ExportData() //exports Data, is called in "VehicleTelemetryManager"
    {
        File.WriteAllLines(filePath, logData.ToArray(), Encoding.UTF8);
        Debug.Log("Data exported to: " + filePath); //Gives Debug.Log to place, where data is exported to
    }
}
