using System.Collections.Generic;
using UnityEngine;

public class SensorReadingCollectionAsset : ScriptableObject
{

    private List<BaseSensorReading> SensorReadings;
    public string databaseName = "sensor_readings";
    public string tableNameInDatabase;

    public Dictionary<Sensor.Type, List<string>> SensorTypeToSensorNameListMap;
    public Dictionary<string, List<BaseSensorReading>> SensorNameToSensorReadingsMap;
    public Dictionary<string, int[]> BookmarkNameToSessionIDs;
    public Dictionary<string, Dictionary<int, List<BaseSensorReading>>> BookmarkNameToSessionIDToSensorReadingsListMap;
    public string[] BookmarkNames;

    public static string AssetLoadPath = "SensorReadingCollection";
    public static string AssetCreationPath = "Assets/Resources/SensorReadingCollection.asset";

    public void GetSensorReadingsFromTableInDB()
    {
        DBInstance dbInstance = new DBInstance(databaseName);
        dbInstance.InitialiseSQLiteConnection();
        SensorReadings = dbInstance.GetSensorReadingsFromTable(tableNameInDatabase);
        dbInstance.CloseSQLiteConnection();
    }

    public int GetSensorReadingsCount()
    {
        if (SensorReadings != null)
            return SensorReadings.Count;
        return -1;
    }

    public int GetSensorTypeCount(Sensor.Type sensorType)
    {
        if (SensorReadings != null)
            if (SensorTypeToSensorNameListMap.ContainsKey(sensorType))
                return SensorTypeToSensorNameListMap[sensorType].Count;
        return 0;
    }

    public int GetSensorReadingsForSensorTypeCount(Sensor.Type sensorType)
    {
        if (SensorReadings != null)
        {
            if (SensorTypeToSensorNameListMap.ContainsKey(sensorType))
            {
                int sensorReadingsCount = 0;
                foreach (string sensorName in SensorTypeToSensorNameListMap[sensorType])
                    sensorReadingsCount += SensorNameToSensorReadingsMap[sensorName].Count;
                return sensorReadingsCount;
            }
        }
        return 0;
    }

    public bool HasCollectionBeenIndexed()
    {
        if (SensorTypeToSensorNameListMap == null)
            return false;

        if (SensorNameToSensorReadingsMap == null)
            return false;

        if (BookmarkNameToSessionIDs == null)
            return false;

        if (BookmarkNameToSessionIDToSensorReadingsListMap == null)
            return false;

        if (BookmarkNames == null)
            return false;

        return true;
    }

    public void IndexSensorReadings()
    {
        InitialiseIndexDataStructures();

        List<string> bookmarkNames = new List<string>();
        Dictionary<string, List<int>> bookmarkNameToSessionIDMap = new Dictionary<string, List<int>>();

        if (SensorReadings != null)
        {
            foreach (BaseSensorReading aSensorReading in SensorReadings)
            {
                RegisterBookmarkName(bookmarkNames, bookmarkNameToSessionIDMap, aSensorReading);
                RegisterSessionID(bookmarkNameToSessionIDMap, aSensorReading);
                AddSensorReadingToSessionIDMap(aSensorReading);

                RegisterSensorType(aSensorReading);
                RegisterSensorNameUnderSensorType(aSensorReading);
                AddSensorReadingToSensorNameMap(aSensorReading);
            }
        }

        foreach (string bookmarkName in bookmarkNames)
            BookmarkNameToSessionIDs.Add(bookmarkName, bookmarkNameToSessionIDMap[bookmarkName].ToArray());
        BookmarkNames = bookmarkNames.ToArray();
    }

    private void InitialiseIndexDataStructures()
    {
        if (BookmarkNameToSessionIDToSensorReadingsListMap == null)
            BookmarkNameToSessionIDToSensorReadingsListMap = new Dictionary<string, Dictionary<int, List<BaseSensorReading>>>();
        BookmarkNameToSessionIDToSensorReadingsListMap.Clear();

        if (BookmarkNameToSessionIDs == null)
            BookmarkNameToSessionIDs = new Dictionary<string, int[]>();
        BookmarkNameToSessionIDs.Clear();

        if (SensorTypeToSensorNameListMap == null)
            SensorTypeToSensorNameListMap = new Dictionary<Sensor.Type, List<string>>();
        SensorTypeToSensorNameListMap.Clear();

        if (SensorNameToSensorReadingsMap == null)
            SensorNameToSensorReadingsMap = new Dictionary<string, List<BaseSensorReading>>();
        SensorNameToSensorReadingsMap.Clear();
    }

    private void RegisterBookmarkName(List<string> bookmarkNames, Dictionary<string, List<int>> bookmarkNameToSessionIDMap,
        BaseSensorReading aSensorReading)
    {
        // Add new sensor bookmark names to relevant data structures
        if (!bookmarkNames.Contains(aSensorReading.sensorBookmarkName))
        {
            bookmarkNames.Add(aSensorReading.sensorBookmarkName);
            BookmarkNameToSessionIDToSensorReadingsListMap.
                Add(aSensorReading.sensorBookmarkName, new Dictionary<int, List<BaseSensorReading>>());
            bookmarkNameToSessionIDMap.Add(aSensorReading.sensorBookmarkName, new List<int>());
        }
    }

    private void RegisterSessionID(Dictionary<string, List<int>> bookmarkNameToSessionIDMap, BaseSensorReading aSensorReading)
    {
        // Add session ID to relevant data structures
        if (!bookmarkNameToSessionIDMap[aSensorReading.sensorBookmarkName].Contains(aSensorReading.sessionID))
        {
            bookmarkNameToSessionIDMap[aSensorReading.sensorBookmarkName].
                Add(aSensorReading.sessionID);
            BookmarkNameToSessionIDToSensorReadingsListMap[aSensorReading.sensorBookmarkName].
                Add(aSensorReading.sessionID, new List<BaseSensorReading>());
        }
    }

    private void AddSensorReadingToSessionIDMap(BaseSensorReading aSensorReading)
    {
        BookmarkNameToSessionIDToSensorReadingsListMap[aSensorReading.sensorBookmarkName][aSensorReading.sessionID].Add(aSensorReading);
    }

    private void RegisterSensorType(BaseSensorReading aSensorReading)
    {
        // Add sensor name to list under appropriate sensor type
        if (!SensorTypeToSensorNameListMap.ContainsKey(aSensorReading.sensorType))
            SensorTypeToSensorNameListMap.Add(aSensorReading.sensorType, new List<string>());
    }

    private void RegisterSensorNameUnderSensorType(BaseSensorReading aSensorReading)
    {
        if (!SensorTypeToSensorNameListMap[aSensorReading.sensorType].Contains(aSensorReading.sensorName))
        {
            SensorTypeToSensorNameListMap[aSensorReading.sensorType].Add(aSensorReading.sensorName);
        }
    }

    private void AddSensorReadingToSensorNameMap(BaseSensorReading aSensorReading)
    {
        // Add sensor reading to list under appropriate sensor name
        if (!SensorNameToSensorReadingsMap.ContainsKey(aSensorReading.sensorName))
            SensorNameToSensorReadingsMap.Add(aSensorReading.sensorName, new List<BaseSensorReading>());
        SensorNameToSensorReadingsMap[aSensorReading.sensorName].Add(aSensorReading);
    }
}
