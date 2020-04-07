using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor class for the sensor reading collection asset class/object.
/// </summary>
[CustomEditor(typeof(SensorReadingCollectionAsset))]
public class SensorReadingCollectionAssetEditor : Editor
{
    private SensorReadingCollectionAsset instance;              // Instance associated with this editor.

    private void OnEnable()
    {
        instance = (SensorReadingCollectionAsset)target;
    }

    public override void OnInspectorGUI()
    {
        DefaultLoaderGUI();
        SensorReadingStatisticsGUI();
    }

    /// <summary>
    /// Helper method which draws the GUI for loading sensor reading data into the asset.
    /// </summary>
    private void DefaultLoaderGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.IntField("# Sensor Readings", instance.GetSensorReadingsCount());
        instance.databaseName = EditorGUILayout.TextField("Database Name", instance.databaseName);
        instance.tableNameInDatabase = EditorGUILayout.TextField("Table Name in Database", instance.tableNameInDatabase);
        if (GUILayout.Button("Get Sensor Readings From Database and Index Sensor Readings"))
        {
            instance.GetSensorReadingsFromTableInDB();
            instance.IndexSensorReadings();
            UpdateSensorReadingStatistics();
        }
        EditorGUILayout.EndVertical();
    }

    /*
        Various properties which were cached for display purposes.
    */

    private int TemperatureCount, PresenceCount, LightCount, InteractionCount, ToggleCount;
    private int sr_temperatureCount, sr_presenceCount, sr_lightCount, sr_interactionCount, sr_toggleCount;

    /// <summary>
    /// Helper method which draws the sensor reading statistics GUI.
    /// </summary>
    private void SensorReadingStatisticsGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.IntField("# Temperature Sensors", TemperatureCount);
        EditorGUILayout.IntField("# Presence Sensors", PresenceCount);
        EditorGUILayout.IntField("# Light Sensors", LightCount);
        EditorGUILayout.IntField("# Interaction Sensors", InteractionCount);
        EditorGUILayout.IntField("# Toggle Sensors", ToggleCount);
        EditorGUILayout.Space();
        EditorGUILayout.IntField("# Temperature Sensor Readings", sr_temperatureCount);
        EditorGUILayout.IntField("# Presence Sensor Readings", sr_presenceCount);
        EditorGUILayout.IntField("# Light Sensor Readings", sr_lightCount);
        EditorGUILayout.IntField("# Interaction Sensor Readings", sr_interactionCount);
        EditorGUILayout.IntField("# Toggle Sensor Readings", sr_toggleCount);
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Helper method for updating the sensor reading statistics.
    /// </summary>
    private void UpdateSensorReadingStatistics()
    {
        TemperatureCount = instance.GetSensorTypeCount(Sensor.Type.Temperature);
        PresenceCount = instance.GetSensorTypeCount(Sensor.Type.Presence);
        LightCount = instance.GetSensorTypeCount(Sensor.Type.Light);
        InteractionCount = instance.GetSensorTypeCount(Sensor.Type.Interaction);
        ToggleCount = instance.GetSensorTypeCount(Sensor.Type.Toggle);

        sr_temperatureCount = instance.GetSensorReadingsForSensorTypeCount(Sensor.Type.Temperature);
        sr_presenceCount = instance.GetSensorReadingsForSensorTypeCount(Sensor.Type.Presence);
        sr_lightCount = instance.GetSensorReadingsForSensorTypeCount(Sensor.Type.Light);
        sr_interactionCount = instance.GetSensorReadingsForSensorTypeCount(Sensor.Type.Interaction);
        sr_toggleCount = instance.GetSensorReadingsForSensorTypeCount(Sensor.Type.Toggle);
    }

    /// <summary>
    /// Helper method for creating the sensor reading collection asset. This method is called
    /// using the appropriate Unity menu.
    /// </summary>
    [MenuItem("Assets/Create/SensorReadingCollection")]
    private static void CreateSensorReadingCollectionAsset()
    {
        // Only create asset if it doesn't exist already in resources folder
        if (Resources.Load<SensorReadingCollectionAsset>(SensorReadingCollectionAsset.AssetLoadPath) == null)
        {
            SensorReadingCollectionAsset newSensorReadingCollectionAsset = CreateInstance<SensorReadingCollectionAsset>();
            AssetDatabase.CreateAsset(newSensorReadingCollectionAsset, SensorReadingCollectionAsset.AssetCreationPath);
        }
    }
}
