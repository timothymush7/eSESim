using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System;

/// <summary>
/// Utility class that handles creation/deletion of SQLite databases and the
/// communication between Unity and SQLite databases.
/// 
/// The included code is a modification of the example script from the SQLiter asset by OuijaPaw Games LLC.
/// </summary>
public class DBInstance
{
    private static string SQL_DB_LOCATION = "";     // String which describes the file URL of the database
    private IDbConnection dbConnection = null;      // Object which handles the connection to database
    private IDbCommand dbCommand = null;            // Object which is used to execute SQL queries
    private IDataReader dbReader = null;            // Object which is used to retrieve output from executing SQL queries

    /// <summary>
    /// Constructor which simply defines the SQL database location.
    /// </summary>
    /// <param name="databaseName">Database name which will be connected to.</param>
    public DBInstance(string databaseName)
    {
        SQL_DB_LOCATION = "URI=file:" + Application.dataPath + "/Databases/" + databaseName + ".db";
        Debug.Log(SQL_DB_LOCATION);
    }

    /// <summary>
    /// Helper method which initialises the database connection based on the defined
    /// SQL database location.
    /// </summary>
    public void InitialiseSQLiteConnection()
    {
        Debug.Log("SQLiter - Opening SQLite Connection at " + SQL_DB_LOCATION);
        dbConnection = new SqliteConnection(SQL_DB_LOCATION);
        dbCommand = dbConnection.CreateCommand();

        dbConnection.Open();
        OptimiseDBPerformance();
        dbConnection.Close();
    }

    /// <summary>
    /// Helper method which executes two SQL queries which optimise the various aspects
    /// of database queries.
    /// </summary>
    private void OptimiseDBPerformance()
    {
        // WAL = write ahead logging, basically a speed increase (see https://sqlite.org/wal.html)
        dbCommand.CommandText = "PRAGMA journal_mode = WAL;";
        dbCommand.ExecuteNonQuery();

        // Speed increase in commits (https://sqlite.org/pragma.html#pragma_synchronous)
        dbCommand.CommandText = "PRAGMA synchronous = OFF";
        dbCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// Utility method for checking whether a database connection has been established.
    /// </summary>
    /// <returns>True if connection established. False otherwise.</returns>
    public bool IsDbConnectionEstablished()
    {
        return dbConnection != null;
    }

    /// <summary>
    /// Helper method which closes the database connection and other
    /// database-related objects.
    /// </summary>
    public void CloseSQLiteConnection()
    {
        if (dbReader != null && !dbReader.IsClosed)
            dbReader.Close();
        dbReader = null;

        if (dbCommand != null)
            dbCommand.Dispose();
        dbCommand = null;

        if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
            dbConnection.Close();
        dbConnection = null;
    }

    /// <summary>
    /// Primary utility method which executes specified SQL database queries
    /// using the currently defined database connection.
    /// </summary>
    /// <param name="SQLquery">SQL query to be executed using the current database connection.</param>
    public void ExecuteNonQuery(string SQLquery)
    {
        dbConnection.Open();
        dbCommand.CommandText = SQLquery;
        dbCommand.ExecuteNonQuery();
        dbConnection.Close();
    }

    /// <summary>
    /// Utility method which checks whether a specific table in the connected database exists.
    /// </summary>
    /// <param name="tableName">Table name in the database to be checked.</param>
    /// <returns>True if a table with the specified name exists in the connected database. False if otherwise.</returns>
    public bool DoesTableExistInDB(string tableName)
    {
        Debug.Log("SQLiter - Checking if table exists: " + tableName);
        bool tableExists = true;
        dbConnection.Open();
        dbCommand.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
        dbReader = dbCommand.ExecuteReader();
        if (!dbReader.Read())
            tableExists = false;
        dbReader.Close();
        dbConnection.Close();
        return tableExists;
    }

    /// <summary>
    /// Utility method which removes a table from the connected database.
    /// </summary>
    /// <param name="tableName">Name of table to be removed from the connected database.</param>
    public void RemoveTableInDB(string tableName)
    {
        Debug.Log("SQLiter - Dropping old SQLite table if exists: " + tableName);
        dbConnection.Open();
        dbCommand.CommandText = "DROP TABLE IF EXISTS " + tableName;
        dbCommand.ExecuteNonQuery();
        dbConnection.Close();
    }

    /// <summary>
    /// Utility method which counts the number of rows in a table of a connected database.
    /// </summary>
    /// <param name="tableName">Name of table in connected database which will be evaluated.</param>
    /// <returns>Number of data rows in specified table. Zero by default.</returns>
    public int GetNumberOfRowsInTable(string tableName)
    {
        Debug.Log("SQLiter - Finding total number of rows in SQLite table if exists: " + tableName);
        int numberOfRows = 0;
        dbConnection.Open();
        dbCommand.CommandText = "SELECT Count(*) FROM " + tableName;
        dbReader = dbCommand.ExecuteReader();
        while (dbReader.Read())
            numberOfRows = dbReader.GetInt32(0);
        dbReader.Close();
        dbConnection.Close();

        return numberOfRows;
    }

    /// <summary>
    /// Helper method which acquires the earliest session ID amongst all data rows in a specified
    /// table of the connected database.
    /// </summary>
    /// <param name="tableName">Name of table to evaluate in the connected database.</param>
    /// <returns>Earliest session ID. Zero by default.</returns>
    public int GetEarliestSessionIDInTable(string tableName)
    {
        int earliestSessionID = 0;
        dbConnection.Open();
        dbCommand.CommandText = "SELECT MIN(session_id) FROM " + tableName;
        dbReader = dbCommand.ExecuteReader();
        while (dbReader.Read())
            earliestSessionID = dbReader.GetInt32(0);
        dbReader.Close();
        dbConnection.Close();

        return earliestSessionID;
    }

    /// <summary>
    /// Helper method which acquires the latest session ID amongst all data rows in a specified
    /// table of the connected database.
    /// </summary>
    /// <param name="tableName">Name of table to evaluate in the connected database.</param>
    /// <returns>Latest session ID. Zero by default.</returns>
    public int GetLatestSessionIDInTable(string tableName)
    {
        int latestSessionID = 0;
        dbConnection.Open();
        dbCommand.CommandText = "SELECT MAX(session_id) FROM " + tableName;
        dbReader = dbCommand.ExecuteReader();
        while (dbReader.Read())
            latestSessionID = dbReader.GetInt32(0);
        dbReader.Close();
        dbConnection.Close();

        return latestSessionID;
    }

    /// <summary>
    /// Helper method which acquires all sensor readings from a specified table in the connected
    /// database.
    /// </summary>
    /// <param name="tableName">Name of table to evaluate in the connected database.</param>
    /// <returns>List of sensor readings from the specified table in the connected database.</returns>
    public List<BaseSensorReading> GetSensorReadingsFromTable(string tableName)
    {
        List<BaseSensorReading> sensorReadings = new List<BaseSensorReading>();
        dbConnection.Open();
        dbCommand.CommandText = "SELECT * FROM " + tableName;
        dbReader = dbCommand.ExecuteReader();

        // Iterate through all data records
        while (dbReader.Read())
        {
            // Extract appropriate data from records
            string sensorType = dbReader.GetString(0);
            string sensorArea = dbReader.GetString(1);
            string sensorName = dbReader.GetString(2);
            int year = dbReader.GetInt16(3);
            int month = dbReader.GetInt16(4);
            int day = dbReader.GetInt16(5);
            int hours = dbReader.GetInt16(6);
            int minutes = dbReader.GetInt16(7);
            int seconds = dbReader.GetInt16(8);
            float sensorValue = dbReader.GetFloat(9);
            string sensorBookmarkName = dbReader.GetString(10);
            int sessionID = dbReader.GetInt16(11);

            // Create sensor reading using extracted data and add it to list
            BaseSensorReading newSensorReading = ConstructSensorReading(sensorArea, sensorName, year, month,
                day, hours, minutes, seconds, sensorValue, Sensor.StringToSensorType(sensorType));
            newSensorReading.sensorBookmarkName = sensorBookmarkName;
            newSensorReading.sessionID = sessionID;
            sensorReadings.Add(newSensorReading);
        }

        dbConnection.Close();
        return sensorReadings;
    }

    /// <summary>
    /// Helper method which creates a base sensor reading object using the specified sensor
    /// reading data. Used to construct sensor reading objects from database data.
    /// </summary>
    /// <param name="sensorArea">Area of where sensor reading was generated.</param>
    /// <param name="sensorName">Unique identifier of sensor which generated the sensor reading.</param>
    /// <param name="year">Year (Date) of when sensor reading was generated.</param>
    /// <param name="month">Month (Date) of when sensor reading was generated.</param>
    /// <param name="day">Day (Date) of when sensor reading was generated.</param>
    /// <param name="hours">Hours (Time) of when sensor reading was generated.</param>
    /// <param name="minutes">Minutes (Time) of when sensor reading was generated.</param>
    /// <param name="seconds">Seconds (Time) of when sensor reading was generated.</param>
    /// <param name="sensorValue">Value associated with the sensor reading.</param>
    /// <param name="sensorType">Sensor type which generated the sensor reading.</param>
    /// <returns></returns>
    private BaseSensorReading ConstructSensorReading(string sensorArea, string sensorName,
        int year, int month, int day, int hours, int minutes, int seconds, float sensorValue,
        Sensor.Type sensorType)
    {
        BaseSensorReading newSensorReading;
        switch (sensorType)
        {
            // Float sensor reading format for light, temperature.
            case Sensor.Type.Light:
            case Sensor.Type.Temperature:
                newSensorReading = new FloatSensorReading
                {
                    sensorValue = sensorValue,
                };
                break;

            // Boolean sensor reading format for presence, toggle
            case Sensor.Type.Presence:
            case Sensor.Type.Toggle:
                newSensorReading = new BoolSensorReading
                {
                    sensorValue = Convert.ToBoolean(sensorValue)
                };
                break;

            // Base sensor reading format for interaction
            default:
                newSensorReading = new BaseSensorReading();
                break;
        }

        // Set the base attributes for the sensor reading
        newSensorReading.areaName = sensorArea;
        newSensorReading.sensorName = sensorName;
        newSensorReading.sensorType = sensorType;
        newSensorReading.SetDateTime(year, month, day, hours, minutes, seconds);

        return newSensorReading;
    }
}

