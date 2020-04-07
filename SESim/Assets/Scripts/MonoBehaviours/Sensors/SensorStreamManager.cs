using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class/component which handles communication between sensor game objects
/// and a sensor stream. Sensor game objects generate sensor readings, and the
/// sensor stream manager listens for these sensor readings to store in the
/// current sensor stream.
/// </summary>
public class SensorStreamManager : MonoBehaviour
{
    /*
        Communication to the sensor stream manager is purely
        event based.
    */

    [Header("Sensor Stream Manager Attributes")]
    [Tooltip("Unique identifier for the sensor stream.")] public int SensorStreamSessionID = 0;
    [Tooltip("Reference to existing database manager for writing sensor readings to a database.")] public DatabaseManager databaseManager;

    private SensorStream CurrentSensorStream;                       // Current sensor stream which temporarily arranges/stores sensor readings
    [HideInInspector] public bool IsSensorStreamActive = false;     // Boolean which indicates whether a sensor stream is active/defined

    private void Start()
    {
        SubscribeToSensorDataEvents();
        SubscribeToSensorStreamEvents();

        if (!databaseManager)
            Debug.LogError("DB Interface for sensor readings not referenced. Cannot write sensor readings to DB.");
    }

    /// <summary>
    /// Helper method which queries for all sensor components and subscribes to
    /// the methods for generating sensor readings.
    /// </summary>
    private void SubscribeToSensorDataEvents()
    {
        if (AllHomeObjects.Instance)
        {
            // Find all sensor game objects and subscribe to the publish sensor data events
            GameObject[] sensorGameObjects;
            if (AllHomeObjects.Instance.TryFindGameObjectArray(AllHomeObjects.TAG_SENSOR, out sensorGameObjects))
            {
                for (int i = 0; i < sensorGameObjects.Length; i++)
                {
                    GameObject sensorGameObject = sensorGameObjects[i];
                    Sensor sensorComponent = sensorGameObject.GetComponent<Sensor>();
                    sensorComponent.GenerateSensorReadingEvent += AddSensorDataToSensorStream;
                }
            }
        }
        else
            Debug.LogError("AllHomeObjects is not defined in the scene.");
    }

    /// <summary>
    /// Helper method which subscribes to the sensor stream-related events.
    /// </summary>
    private void SubscribeToSensorStreamEvents()
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            // Sensor bookmark events
            SimulationLayerEventsObserver.Instance.AddListenerToAddSensorBookmarkEvent(AddSensorBookmarkToSensorStream);
            SimulationLayerEventsObserver.Instance.AddListenerToAnnotateEndSensorBookmarkEvent(AnnotateEndOfSensorBookmarkFromSensorStream);

            // Sensor stream events
            SimulationLayerEventsObserver.Instance.AddListenerToNoArgumentEvent(SimulationLayerEventsObserver.KEY_START_NEW_SENSOR_STREAM,
                StartNewSensorStream);
            SimulationLayerEventsObserver.Instance.AddListenerToNoArgumentEvent(SimulationLayerEventsObserver.KEY_END_CURRENT_SENSOR_STREAM,
                EndCurrentSensorStream);
            SimulationLayerEventsObserver.Instance.AddListenerToNoArgumentEvent(SimulationLayerEventsObserver.KEY_WRITE_SENSOR_READINGS_TO_DB,
                WriteSensorReadingsToDatabase);
        }
        else
            Debug.LogError("Simulation Layer Events Observer is not defined in the scene.");
    }

    /// <summary>
    /// Utility method/callback which initialises a new sensor stream.
    /// </summary>
    private void StartNewSensorStream()
    {
        if (CurrentSensorStream == null)
            CurrentSensorStream = new SensorStream(SensorStreamSessionID);
        else
        {
            CurrentSensorStream.SessionID = SensorStreamSessionID;
            CurrentSensorStream.Clear();
        }

        IsSensorStreamActive = true;
        SensorStreamSessionID++;
    }

    /// <summary>
    /// Utility method/callback which ends the current sensor stream.
    /// </summary>
    private void EndCurrentSensorStream()
    {
        IsSensorStreamActive = false;
    }

    /// <summary>
    /// Utility method/callback which writes all sensor readings from the sensor stream
    /// into a database using the referenced database manager.
    /// </summary>
    private void WriteSensorReadingsToDatabase()
    {
        if (databaseManager)
        {
            /*
                This code writes sensor readings for EACH sensor bookmark defined in the sensor stream.
                Therefore it is possible for sensor readings to be inserted multiple times under different
                sensor bookmarks.
            */

            foreach (SensorStreamBookmark aBookmark in CurrentSensorStream.GetSensorStreamBookmarks())
            {
                List<BaseSensorReading> sensorReadingsWithinBookmark =
                    CurrentSensorStream.GetSensorReadingsWithinSensorBookmark(aBookmark);

                databaseManager.InsertSensorReadingsListInDatabaseTable(
                    sensorReadingsWithinBookmark,
                    aBookmark.BookmarkName,
                    CurrentSensorStream.SessionID);
            }
        }
        else
            Debug.LogError("Database manager reference is null. Please attach one to the sensor stream manager object");
    }

    /// <summary>
    /// Primary utility method/callback which adds a newly generated sensor reading
    /// into the current sensor stream.
    /// </summary>
    /// <param name="newSensorReading">New sensor reading to be added.</param>
    private void AddSensorDataToSensorStream(BaseSensorReading newSensorReading)
    {
        if (IsSensorStreamActive)
            CurrentSensorStream.AddSensorData(newSensorReading);
    }

    /// <summary>
    /// Utility method/callback which adds a new bookmark to the current sensor stream.
    /// </summary>
    /// <param name="bookmarkName">Name of new sensor bookmark.</param>
    private void AddSensorBookmarkToSensorStream(string bookmarkName)
    {
        if (IsSensorStreamActive)
        {
            System.DateTime startDateTime = System.DateTime.MaxValue;

            if (DateController.Instance)
                startDateTime = DateController.Instance.GetCurrentDate(startDateTime);
            else
                Debug.LogError("Date controller does not exist in the scene.");

            if (TimeController.Instance)
                startDateTime = TimeController.Instance.GetCurrentTime(startDateTime);
            else
                Debug.LogError("Time controller does not exist in the scene.");

            CurrentSensorStream.AddSensorBookmark(bookmarkName, startDateTime);
        }
    }

    /// <summary>
    /// Helper method/callback which annotates/updates the end time of a specified
    /// sensor bookmark in the current sensor stream.
    /// </summary>
    /// <param name="bookmarkName">Name of sensor bookmark to be annotated/updated.</param>
    private void AnnotateEndOfSensorBookmarkFromSensorStream(string bookmarkName)
    {
        if (IsSensorStreamActive)
        {
            System.DateTime endDateTime = System.DateTime.MaxValue;

            if (DateController.Instance)
                endDateTime = DateController.Instance.GetCurrentDate(endDateTime);
            else
                Debug.LogError("Date controller instance does not exist.");

            if (TimeController.Instance)
                endDateTime = TimeController.Instance.GetCurrentTime(endDateTime);
            else
                Debug.LogError("Time controller instance does not exist.");

            CurrentSensorStream.AnnotateEndOfSensorBookmark(bookmarkName, endDateTime);
        }
    }
}
