/// <summary>
/// Class which assists with generating SQL queries for the sensor reading
/// database.
/// </summary>
public static class SensorReadingTableDetails
{
    /*
        String properties describing table names in the database.
    */

    public static string TABLE_NAME_ALL = "all_sr";
    public static string TABLE_NAME_COOKING = "cooking_sr", TABLE_NAME_WASH_DISHES = "wash_dishes_sr", TABLE_NAME_SLEEPING = "sleeping_sr", TABLE_NAME_DRESSING = "dressing_sr";

    /*
        String properties describing column names for tables in the database.
    */

    public const string COL_SENSOR_TYPE = "sensor_type";
    public const string COL_SENSOR_AREA = "sensor_area";
    public const string COL_SENSOR_NAME = "sensor_name";
    public const string COL_SENSOR_DATE_YEAR = "sensor_date_year";
    public const string COL_SENSOR_DATE_MONTH = "sensor_date_month";
    public const string COL_SENSOR_DATE_DAY = "sensor_date_day";
    public const string COL_SENSOR_TIME_HOURS = "sensor_time_hours";
    public const string COL_SENSOR_TIME_MINUTES = "sensor_time_minutes";
    public const string COL_SENSOR_TIME_SECONDS = "sensor_time_seconds";
    public const string COL_SENSOR_VALUE = "sensor_value";
    public const string COL_SENSOR_BOOKMARK_NAME = "sensor_bookmark_name";
    public const string COL_SESSION_ID = "session_id";

    /// <summary>
    /// Utility method which generates the SQL string for creating a table
    /// in the database.
    /// </summary>
    /// <param name="TABLE_NAME">Name of table to be created in the database.</param>
    /// <returns>SQL string for creating table in the database.</returns>
    public static string CreateTableSQLString(string TABLE_NAME)
    {
        return "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " (" +
                COL_SENSOR_TYPE + " TEXT NOT NULL, " +
                COL_SENSOR_AREA + " TEXT NOT NULL, " +
                COL_SENSOR_NAME + " TEXT NOT NULL, " +
                COL_SENSOR_DATE_YEAR + " INTEGER NOT NULL, " +
                COL_SENSOR_DATE_MONTH + " INTEGER NOT NULL, " +
                COL_SENSOR_DATE_DAY + " INTEGER NOT NULL, " +
                COL_SENSOR_TIME_HOURS + " INTEGER NOT NULL, " +
                COL_SENSOR_TIME_MINUTES + " INTEGER NOT NULL, " +
                COL_SENSOR_TIME_SECONDS + " INTEGER NOT NULL, " +
                COL_SENSOR_VALUE + " REAL NOT NULL, " +
                COL_SENSOR_BOOKMARK_NAME + " TEXT NOT NULL, " +
                COL_SESSION_ID + " INTEGER NOT NULL " + ")";
    }

    /// <summary>
    /// Utility method which generates the SQL string for deleting a table
    /// from the database.
    /// </summary>
    /// <param name="TABLE_NAME">Name of table to be deleted from the database.</param>
    /// <returns>SQL string for deleting a table from the database.</returns>
    public static string DropTableSQLString(string TABLE_NAME)
    {
        return "DROP TABLE IF EXISTS " + TABLE_NAME;
    }

    /// <summary>
    /// Utility method which generates the SQL string for inserting a sensor reading
    /// into a specified table in the database.
    /// </summary>
    /// <param name="aSensorReading">Sensor reading to be inserted.</param>
    /// <param name="bookmarkName">Bookmark name associated with the sensor reading.</param>
    /// <param name="sessionID">Session ID associated with the sensor reading.</param>
    /// <param name="tableName">Name of table which will be storing the sensor reading.</param>
    /// <returns>SQL string for inserting the specified sensor reading into a specified table in the database.</returns>
    public static string InsertSensorReadingSQLString(BaseSensorReading aSensorReading,
         string bookmarkName, int sessionID, string tableName)
    {
        return "INSERT OR REPLACE INTO " + tableName + " ("
            + COL_SENSOR_TYPE + "," + COL_SENSOR_AREA + "," + COL_SENSOR_NAME + ","
            + COL_SENSOR_DATE_YEAR + "," + COL_SENSOR_DATE_MONTH + "," + COL_SENSOR_DATE_DAY + ","
            + COL_SENSOR_TIME_HOURS + "," + COL_SENSOR_TIME_MINUTES + "," + COL_SENSOR_TIME_SECONDS + ","
            + COL_SENSOR_VALUE + "," + COL_SENSOR_BOOKMARK_NAME + "," + COL_SESSION_ID
            + ") VALUES (" + "'"
            + aSensorReading.sensorType + "','" + aSensorReading.areaName + "','" + aSensorReading.sensorName + "',"
            + aSensorReading.dateTime.Year + "," + aSensorReading.dateTime.Month + "," + aSensorReading.dateTime.Day + ","
            + aSensorReading.dateTime.Hour + "," + aSensorReading.dateTime.Minute + "," + aSensorReading.dateTime.Second + ","
            + aSensorReading.GetSensorValue() + ",'" + bookmarkName + "'," + sessionID + ")";
    }

    /// <summary>
    /// Utility method which generates the SQL string for transferring data records between tables in the database.
    /// </summary>
    /// <param name="fromTableName">Name of table whose data records will be transferred.</param>
    /// <param name="toTableName">Name of table who will be receiving the transferred data records.</param>
    /// <param name="sessionIDStart">Lower bound of session ID for sensor readings to be transferred.</param>
    /// <param name="sessionIDEnd">Upper bound of session ID for sensor readings to be transferred.</param>
    /// <returns>SQL string for transferring data records between tables in the database.</returns>
    public static string TransferSensorReadingsBetweenTablesSQLString(string fromTableName, string toTableName, int sessionIDStart, int sessionIDEnd)
    {
        return "INSERT INTO " + toTableName + " SELECT * FROM " + fromTableName + " WHERE session_id >= "
            + sessionIDStart + " AND session_id <= " + sessionIDEnd + ";";
    }

    /// <summary>
    /// Utility method which generates the SQL string for acquiring all sensor readings from a specific
    /// table in the database.
    /// </summary>
    /// <param name="TABLE_NAME">Name of table whose sensor readings will be retrieved.</param>
    /// <returns>SQL string for acquiring all sensor readings from a specified table.</returns>
    public static string GetAllSensorReadingsSQLString(string TABLE_NAME)
    {
        return "SELECT * FROM " + TABLE_NAME;
    }

    /// <summary>
    /// Utility method which generates the SQL string for counting the number of data records in a
    /// specific table in the database.
    /// </summary>
    /// <param name="TABLE_NAME">Name of table whose data records will be counted.</param>
    /// <returns>SQL string for counting the number of data records within the specified table of the database.</returns>
    public static string CountNumberOfRowsSQLString(string TABLE_NAME)
    {
        return "SELECT Count(*) FROM " + TABLE_NAME;
    }
}
