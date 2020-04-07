using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuantiseWindow : EditorWindow
{

    private List<BaseSensorReading> sensorReadings;
    private SensorReadingCollectionAsset assetInstance;

    private bool debugModeOn = true;
    private QuantiseMode quantiseMode = QuantiseMode.Selected;
    private int samplingRate = 0;
    private Sensor[] sensorObjects;

    public enum QuantiseMode
    {
        Selected,
        All
    }

    private string targetDirectory = FileHandler.DEFAULT_FILEPATH;
    private string targetFilename = "";
    private bool includeHeaders = true;
    private bool appendDataToFile = true;

    [MenuItem("Window/Quantise Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(QuantiseWindow));
    }

    private void OnGUI()
    {
        SensorReadingCollectionAssetGUI();
        WriteToFileGUI();
        QuantiseFieldsGUI();
    }

    private void SensorReadingCollectionAssetGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Sensor Reading Collection Asset Fields", EditorStyles.boldLabel);
        assetInstance = (SensorReadingCollectionAsset)
            EditorGUILayout.ObjectField(assetInstance, typeof(SensorReadingCollectionAsset), true);
        EditorGUILayout.EndVertical();
    }

    private void QuantiseFieldsGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Quantise Fields", EditorStyles.boldLabel);
        quantiseMode = (QuantiseMode)EditorGUILayout.EnumPopup("Quantise Mode", quantiseMode);
        samplingRate = EditorGUILayout.IntSlider("Sampling Rate (seconds)", samplingRate, 1, TimeController.SECONDS_IN_MINUTE - 1);
        debugModeOn = EditorGUILayout.Toggle("Debug Mode", debugModeOn);
        if (GUILayout.Button("Rebuild Sensor Objects Array"))
        {
            sensorObjects = FindObjectsOfType<Sensor>();
            //string test = "";
            //for (int i = 0; i < sensorObjects.Length; i++)
            //    test += sensorObjects[i].SensorName + " ";
            //Debug.Log(test);
        }

        EditorGUILayout.EndVertical();

        // Display appropriate quantise GUI only if asset defined
        if (assetInstance)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (assetInstance.HasCollectionBeenIndexed())
            {
                if (quantiseMode == QuantiseMode.Selected)
                    QuantiseSelectedGUI();
                else if (quantiseMode == QuantiseMode.All)
                    QuantiseAllGUI();
            }
            else
                EditorGUILayout.LabelField("Asset has not been indexed. Please rebuild the asset.");

            EditorGUILayout.EndVertical();
        }
    }

    private int sessionIDIndex = 0;
    private int bookmarkNameIndex = 0;

    private void QuantiseSelectedGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        bookmarkNameIndex = EditorGUILayout.Popup("Bookmark Name", bookmarkNameIndex, assetInstance.BookmarkNames);
        int[] sessionIDs = assetInstance.BookmarkNameToSessionIDs[assetInstance.BookmarkNames[bookmarkNameIndex]];
        sessionIDIndex = EditorGUILayout.IntSlider("Session ID Index", sessionIDIndex, 0, sessionIDs.Length - 1);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        List<BaseSensorReading> sensorReadings = assetInstance.BookmarkNameToSessionIDToSensorReadingsListMap[assetInstance.BookmarkNames[bookmarkNameIndex]][sessionIDs[sessionIDIndex]];
        EditorGUILayout.IntField("# Sensor Readings", sensorReadings.Count);
        EditorGUILayout.IntField("Session ID", sessionIDs[sessionIDIndex]);

        System.DateTime startTime = GetStartTimeInSensorReadingsList(sensorReadings);
        EditorGUILayout.LabelField("Start Time", startTime.ToLongTimeString());
        System.DateTime endTime = GetEndTimeInSensorReadingsList(sensorReadings);
        EditorGUILayout.LabelField("End Time", endTime.ToLongTimeString());
        EditorGUILayout.LabelField("Session Duration", (endTime - startTime).Duration().ToString());
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Quantise Selected Sensor Readings"))
        {
            FileHandler.WriteInputVectorsToFile(GetQuantisedSensorReadings(sensorReadings, startTime),
                    targetDirectory + targetFilename, includeHeaders, appendDataToFile);
        }

    }

    private System.DateTime GetStartTimeInSensorReadingsList(List<BaseSensorReading> sensorReadings)
    {
        // Impossible maximum, so any date would be set as earliest
        System.DateTime startDateTime = System.DateTime.MaxValue;

        if (sensorReadings != null)
        {
            // Search for earliest time amongst the sensor readings
            foreach (BaseSensorReading aSensorReading in sensorReadings)
                if (startDateTime.CompareTo(aSensorReading.dateTime) > 0)
                    startDateTime = aSensorReading.dateTime;
        }

        return startDateTime;
    }

    private System.DateTime GetEndTimeInSensorReadingsList(List<BaseSensorReading> sensorReadings)
    {
        // Set impossible minimum, so any date would be set as latest
        System.DateTime endDateTime = System.DateTime.MinValue;

        if (sensorReadings != null)
        {
            // Search for latest time amongst the sensor readings
            foreach (BaseSensorReading aSensorReading in sensorReadings)
                if (endDateTime.CompareTo(aSensorReading.dateTime) < 0)
                    endDateTime = aSensorReading.dateTime;
        }

        return endDateTime;
    }

    private InputVector[] GetQuantisedSensorReadings(List<BaseSensorReading> sensorReadings, System.DateTime startTime)
    {
        List<BaseSensorReading>[] dividedSensorReadings =
            QuantiseHelper.SampleSensorReadings(sensorReadings, startTime, 0, 0, samplingRate);

        if (dividedSensorReadings.Length > 0)
        {
            if (sensorObjects != null)
            {
                if (debugModeOn)
                {
                    Debug.Log("Quantising Process");
                    Debug.Log("There are a total of " + sensorReadings.Count + " sensor readings sent for quantising.");

                    Debug.Log("Sensor reading dataset divided. There are a total of " + dividedSensorReadings.Length + " divisions.");
                    for (int i = 0; i < dividedSensorReadings.Length; i++)
                    {
                        List<BaseSensorReading> sensorReadingDivision = dividedSensorReadings[i];
                        Debug.Log("Division " + i + " consists of " + sensorReadingDivision.Count + " sensor readings.");
                        Debug.Log("----------------------------------------------------------");
                        foreach (BaseSensorReading aSensorReading in sensorReadingDivision)
                            Debug.Log("Time: " + aSensorReading.dateTime.ToLongTimeString());
                    }
                }

                return QuantiseHelper.QuantiseSensorReadingsSamples(dividedSensorReadings, sensorObjects);
            }
            else
                Debug.LogError("Sensor objects array not defined. Please rebuild the sensor objects array.");
        }
        else
            Debug.LogError("Zero divisions were found for quantising. Please check your quantise period and function.");

        return null;
    }

    private void QuantiseAllGUI()
    {
        if (GUILayout.Button("Quantise All Sensor Readings"))
            FileHandler.WriteInputVectorsListToFile(GetAllQuantisedSensorReadings(),
                targetDirectory + targetFilename, includeHeaders, appendDataToFile);
    }

    private List<InputVector[]> GetAllQuantisedSensorReadings()
    {
        List<InputVector[]> inputVectorsList = new List<InputVector[]>();

        if (sensorObjects != null)
        {
            for (int i = 0; i < assetInstance.BookmarkNames.Length; i++)
            {
                int[] sessionIDsForBookmarkName = assetInstance.BookmarkNameToSessionIDs[assetInstance.BookmarkNames[i]];

                for (int j = 0; j < sessionIDsForBookmarkName.Length; j++)
                {
                    List<BaseSensorReading> sensorReadingsOfBookmarkAndSessionID =
                        assetInstance.BookmarkNameToSessionIDToSensorReadingsListMap[assetInstance.BookmarkNames[i]][sessionIDsForBookmarkName[j]];
                    System.DateTime startTime = GetStartTimeInSensorReadingsList(sensorReadingsOfBookmarkAndSessionID);

                    InputVector[] quantisedSensorReadings = GetQuantisedSensorReadings(sensorReadingsOfBookmarkAndSessionID, startTime);
                    if (quantisedSensorReadings != null)
                        inputVectorsList.Add(quantisedSensorReadings);
                }
            }
        }
        else
            Debug.LogError("Sensor objects array is null. Please rebuild the sensor objects array.");

        if (debugModeOn)
        {
            int inputVectorsCount = 0;
            foreach (InputVector[] inputVectors in inputVectorsList)
                inputVectorsCount += inputVectors.Length;
            Debug.Log("Quantisation process complete. There is a total of " + inputVectorsCount +
                " input vectors generated from quantising sensor readings.");
        }

        return inputVectorsList;
    }

    private void WriteToFileGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Write-to-File Fields", EditorStyles.boldLabel);
        targetDirectory = EditorGUILayout.TextField("Target Directory", targetDirectory);
        targetFilename = EditorGUILayout.TextField("Target Filename", targetFilename);
        includeHeaders = EditorGUILayout.Toggle("Include Headers?", includeHeaders);
        appendDataToFile = EditorGUILayout.Toggle("Append Data To File?", appendDataToFile);
        EditorGUILayout.EndVertical();
    }

    private List<BaseSensorReading> SortSensorReadingsList(List<BaseSensorReading> sensorReadings)
    {
        BaseSensorReading[] sensorReadingsArray = sensorReadings.ToArray();

        if (sensorReadingsArray.Length > 1)
        {
            int initPos = 1;
            while (initPos < sensorReadingsArray.Length)
            {
                int listPos = initPos;
                while ((listPos > 0) && (sensorReadingsArray[listPos - 1].
                    CompareToDateTime(sensorReadingsArray[listPos]) > 0))
                {
                    BaseSensorReading sensorReadingA = sensorReadingsArray[listPos];
                    sensorReadingsArray[listPos] = sensorReadingsArray[listPos - 1];
                    sensorReadingsArray[listPos - 1] = sensorReadingA;

                    listPos--;
                }
                initPos++;
            }
        }

        sensorReadings.Clear();
        for (int i = 0; i < sensorReadingsArray.Length; i++)
            sensorReadings.Add(sensorReadingsArray[i]);
        return sensorReadings;
    }
}
