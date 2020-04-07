using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class/component for game objects which facilitates communication
/// with a database interface.
/// </summary>
public class DatabaseManager : MonoBehaviour
{
    /*
        This component was created to initialise the DB interface class
        at runtime. The code for this component can be moved if necessary
        to reduce unnecessary classes/code.
    */

    [Tooltip("Database name which will be communicated with.")] public string SQL_DB_NAME = "sensor_readings";
    [Tooltip("Table name which will be communicated with in a database.")] public string TABLE_NAME = "";

    public void Awake()
    {
        DBInterfaceForSensorReadings.InitialiseDatabaseConnection(SQL_DB_NAME);
    }

    public void OnDestroy()
    {
        DBInterfaceForSensorReadings.CloseDatabaseConnection();
    }

    /// <summary>
    /// Helper method for inserting sensor readings into a specific table in a connected database.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be inserted.</param>
    /// <param name="bookmarkName">Sensor bookmark name associated with the sensor readings.</param>
    /// <param name="sessionID">Unique identifier associated with the sensor readings.</param>
    public void InsertSensorReadingsListInDatabaseTable(List<BaseSensorReading> sensorReadings, string bookmarkName, int sessionID)
    {
        DBInterfaceForSensorReadings.InsertSensorReadingsInDatabaseTable(sensorReadings, bookmarkName, sessionID, TABLE_NAME);
    }
}
