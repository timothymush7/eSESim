using System.Collections.Generic;

/// <summary>
/// Helper class which acts as an interface to the DBInstance class.
/// This class is specifically designed to handle sensor reading data
/// with databases.
/// </summary>
public static class DBInterfaceForSensorReadings
{
    private static DBInstance dbInstance;           // Database connection instance

    /*
        Consider defining an interface/parent class for these methods,
        as it is shared with the DBInterfaceForInputVectors class.
    */

    /// <summary>
    /// Utility method for initialising a connection to a specified database.
    /// </summary>
    /// <param name="databaseName">Name of database to be connected to.</param>
    public static void InitialiseDatabaseConnection(string databaseName)
    {
        dbInstance = new DBInstance(databaseName);
        dbInstance.InitialiseSQLiteConnection();
    }

    /// <summary>
    /// Utility method for closing an existing connection to a database.
    /// </summary>
    public static void CloseDatabaseConnection()
    {
        if (dbInstance != null)
            dbInstance.CloseSQLiteConnection();
    }

    /// <summary>
    /// Utility method which executes a SQL string to create a table in the database
    /// for sensor readings.
    /// </summary>
    /// <param name="tableName">Name of table to be created in the database.</param>
    public static void CreateTableInDatabaseForSensorReadings(string tableName)
    {
        dbInstance.ExecuteNonQuery(SensorReadingTableDetails.CreateTableSQLString(tableName));
    }

    /// <summary>
    /// Utility method acquires sensor readings from a specified table from the connected database
    /// by executing the appropriate SQL string.
    /// </summary>
    /// <param name="tableName">Name of table whose sensor readings will be acquired.</param>
    /// <returns>List of sensor readings from a specified table in the connected database.</returns>
    public static List<BaseSensorReading> GetSensorReadingsFromDatabaseTable(string tableName)
    {
        return dbInstance.GetSensorReadingsFromTable(tableName);
    }

    /// <summary>
    /// Utility method for inserting sensor readings into a table in the connected database.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be inserted.</param>
    /// <param name="bookmarkName">Name of bookmark associated with the sensor readings.</param>
    /// <param name="sessionID">Session ID associated with the sensor readings.</param>
    /// <param name="tableName">Name of table who will be storing the sensor readings.</param>
    public static void InsertSensorReadingsInDatabaseTable(List<BaseSensorReading> sensorReadings, string bookmarkName, int sessionID, string tableName)
    {
        if (sensorReadings != null)
            if (sensorReadings.Count != 0)
                foreach (BaseSensorReading aSensorReading in sensorReadings)
                    InsertSensorReadingInDatabaseTable(aSensorReading, bookmarkName, sessionID, tableName);
    }

    /// <summary>
    /// Utility, helper method for inserting a sensor reading into a table in the connected database.
    /// </summary>
    /// <param name="sensorReading">Sensor reading to be inserted.</param>
    /// <param name="bookmarkName">Name of bookmark associated with the sensor reading.</param>
    /// <param name="sessionID">Session ID associated with the sensor reading.</param>
    /// <param name="tableName">Name of table who will be storing the sensor reading.</param>
    public static void InsertSensorReadingInDatabaseTable(BaseSensorReading sensorReading, string bookmarkName, int sessionID, string tableName)
    {
        dbInstance.ExecuteNonQuery(SensorReadingTableDetails.InsertSensorReadingSQLString(sensorReading, bookmarkName, sessionID, tableName));
    }

    /// <summary>
    /// Utility method for transferring sensor readings, between a specified session IDs, between two tables
    /// in the connected database.
    /// </summary>
    /// <param name="fromTableName">Name of table who will be transferring sensor readings.</param>
    /// <param name="toTableName">Name of table who will be receiving the transferred sensor readings.</param>
    /// <param name="sessionIDStart">Lower bound of session IDs of sensor readings that will be transferred.</param>
    /// <param name="sessionIDEnd">Upper bound of session IDs of sensor readings that will be transferred.</param>
    public static void TransferSensorReadings(string fromTableName, string toTableName, int sessionIDStart, int sessionIDEnd)
    {
        dbInstance.ExecuteNonQuery(SensorReadingTableDetails.TransferSensorReadingsBetweenTablesSQLString(fromTableName, toTableName, sessionIDStart, sessionIDEnd));
    }

    /// <summary>
    /// Utility method which acquires the earliest/lowest bound of session IDs amongst sensor readings
    /// from the specified table in the connected database.
    /// </summary>
    /// <param name="tableName">Name of table which will be searched for the earliest session ID.</param>
    /// <returns>The earliest session ID from specified table in the connected database.</returns>
    public static int GetEarliestSessionIDInDatabaseTable(string tableName)
    {
        return dbInstance.GetEarliestSessionIDInTable(tableName);
    }

    /// <summary>
    /// Utility method which acquires the latest/highest bound of session IDs amongst sensor readings
    /// from the specified table in the connected database.
    /// </summary>
    /// <param name="tableName">Name of table which will be searched for the latest session ID.</param>
    /// <returns>The latest session ID from specified table in the connected database.</returns>
    public static int GetLatestSessionIDInDatabaseTable(string tableName)
    {
        return dbInstance.GetLatestSessionIDInTable(tableName);
    }
}
