using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MySQLHelperWindow : EditorWindow {

    private string databaseName = "sensor_readings";
    private string fromTableNameInDatabase, toTableNameInDatabase;
    private int sessionIDStart = 0, sessionIDEnd = 0;
    private int sessionIDMin = 0, sessionIDMax = 0;
    private bool informationLoaded = false;

    private List<BaseSensorReading> sensorReadings;

    [MenuItem("Window/MySQL Helper Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MySQLHelperWindow));
    }

    private void OnGUI()
    {
        FromDatabaseGUI();
        TransferToDatabaseGUI();
    }

    private void FromDatabaseGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("From-Database Fields", EditorStyles.boldLabel);
        databaseName = EditorGUILayout.TextField("Database Name", databaseName);
        fromTableNameInDatabase = EditorGUILayout.TextField("Table Name in Database", fromTableNameInDatabase);

        if (GUILayout.Button("Load Session ID Information"))
        {
            DBInterfaceForSensorReadings.InitialiseDatabaseConnection(databaseName);
            sessionIDMin = DBInterfaceForSensorReadings.GetEarliestSessionIDInDatabaseTable(fromTableNameInDatabase);
            sessionIDMax = DBInterfaceForSensorReadings.GetLatestSessionIDInDatabaseTable(fromTableNameInDatabase);
            DBInterfaceForSensorReadings.CloseDatabaseConnection();

            informationLoaded = true;
        }

        EditorGUILayout.EndVertical();
    }

    private void TransferToDatabaseGUI()
    {
        if (informationLoaded)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Transfer-To-Database Fields", EditorStyles.boldLabel);
            toTableNameInDatabase = EditorGUILayout.TextField("Table Name in Database", toTableNameInDatabase);
            sessionIDStart = EditorGUILayout.IntSlider("Session ID Start", sessionIDStart, sessionIDMin, sessionIDMax);
            sessionIDEnd = EditorGUILayout.IntSlider("Session ID End", sessionIDEnd, sessionIDMin, sessionIDMax);

            if (GUILayout.Button("Transfer Sensor Readings"))
            {
                DBInterfaceForSensorReadings.InitialiseDatabaseConnection(databaseName);
                DBInterfaceForSensorReadings.TransferSensorReadings(fromTableNameInDatabase, toTableNameInDatabase, sessionIDStart, sessionIDEnd);
                DBInterfaceForSensorReadings.CloseDatabaseConnection();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
