using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit test class which describes various methods for testing communication with SQLite databases.
/// </summary>
public class DatabaseTests
{
    [Test]
    public void InitialiseAndCloseDBConnection()
    {
        DBInstance dbInstance = new DBInstance("sensor_readings");
        dbInstance.InitialiseSQLiteConnection();
        bool dbInitialised = dbInstance.IsDbConnectionEstablished();
        dbInstance.CloseSQLiteConnection();
        bool dbClosed = dbInstance.IsDbConnectionEstablished();

        if ((dbInitialised) && (!dbClosed))
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void CreatingAndRemovingTableInDB()
    {
        DBInstance dbInstance = new DBInstance("sensor_readings");
        dbInstance.InitialiseSQLiteConnection();

        string newTableName = "test";

        dbInstance.ExecuteNonQuery(
            SensorReadingTableDetails.CreateTableSQLString(newTableName));

        bool doesTableExistAfterCreation = dbInstance.DoesTableExistInDB(newTableName);
        Debug.Log("Table '" + newTableName + "' should now exist: " + doesTableExistAfterCreation);

        dbInstance.ExecuteNonQuery(
            SensorReadingTableDetails.DropTableSQLString(newTableName));

        bool doesTableExistAfterRemoved = dbInstance.DoesTableExistInDB(newTableName);

        Debug.Log("Table '" + newTableName + "' should now not exist: " + doesTableExistAfterRemoved);

        dbInstance.CloseSQLiteConnection();

        if ((doesTableExistAfterCreation) && (!doesTableExistAfterRemoved))
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void InsertingSensorReading()
    {
        DBInstance dbInstance = new DBInstance("sensor_readings");
        dbInstance.InitialiseSQLiteConnection();

        string newTableName = "testTable";

        dbInstance.ExecuteNonQuery(SensorReadingTableDetails.CreateTableSQLString(newTableName));

        int numberOfRowsBeforeAddition = dbInstance.GetNumberOfRowsInTable(newTableName);
        Debug.Log("Number of rows in table before addition: " + numberOfRowsBeforeAddition);

        BaseSensorReading newSensorReading = new BaseSensorReading
        {
            sensorName = " ",
            sensorType = Sensor.Type.Interaction,
            areaName = " "
        };
        newSensorReading.SetDateTime(1, 1, 1, 1, 1, 1);

        string insertSensorReadingString = SensorReadingTableDetails.InsertSensorReadingSQLString(newSensorReading, "testBookmark", 0, newTableName);
        Debug.Log(insertSensorReadingString);
        dbInstance.ExecuteNonQuery(insertSensorReadingString);

        int numberOfRowsAfterAddition = dbInstance.GetNumberOfRowsInTable(newTableName);
        Debug.Log("Number of rows in table after addition: " + numberOfRowsAfterAddition);

        dbInstance.ExecuteNonQuery(SensorReadingTableDetails.DropTableSQLString(newTableName));
        dbInstance.CloseSQLiteConnection();

        if (numberOfRowsBeforeAddition == numberOfRowsAfterAddition - 1)
            Assert.Pass();
        else
            Assert.Fail();
    }
}

