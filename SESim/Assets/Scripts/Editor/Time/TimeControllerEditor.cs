using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor class for the time controller class/component.
/// </summary>
[CustomEditor(typeof(TimeController))]
public class TimeControllerEditor : Editor
{
    private TimeController Controller;      // Current instance associated with this editor.
    private GUIStyle EditorLabelStyle;      // Style defined for the overall time GUI label.

    private void OnEnable()
    {
        // Reference instance
        Controller = (TimeController)target;

        // Setup editor label style to be used
        EditorLabelStyle = new GUIStyle
        {
            fontSize = 18,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter
        };
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        // For displaying the script (for quick edit if need)
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TimeController)target),
            typeof(TimeController), false);
        GUI.enabled = true;

        InitialTimeGUI();
        TotalCurrentTimeGUI();
        TimeScaleGUI();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Helper method for drawing the GUI for specifying the initial time.
    /// </summary>
    private void InitialTimeGUI()
    {
        Controller.InitialTimeHours =
            EditorGUILayout.IntSlider("Initial Time (Hours)",
            Controller.InitialTimeHours, 0, TimeController.HOURS_IN_DAY - 1);
        Controller.InitialTimeMinutes =
            EditorGUILayout.IntSlider("Initial Time (Minutes)",
            Controller.InitialTimeMinutes, 0, TimeController.MINUTES_IN_HOUR - 1);
        Controller.InitialTimeSeconds =
            EditorGUILayout.IntSlider("Initial Time (Seconds)",
            Controller.InitialTimeSeconds, 0, TimeController.SECONDS_IN_MINUTE - 1);

        Controller.InitialTimeOfDay = Controller.ConvertTimeValuesToSimulationTime(Controller.InitialTimeHours,
            Controller.InitialTimeMinutes, Controller.InitialTimeSeconds);
        EditorGUILayout.Slider("Initial Time Of Day", Controller.InitialTimeOfDay, 0f, 1f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Initial Time Of Day In 24-Hour Format:");
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField(Controller.InitialTimeHours
            + ":" + Controller.InitialTimeMinutes
            + ":" + Controller.InitialTimeSeconds, EditorLabelStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Helper method for drawing the GUI for displaying the specified, overall time.
    /// </summary>
    private void TotalCurrentTimeGUI()
    {
        Controller.TotalElapsedTime =
            EditorGUILayout.FloatField("Total Elapsed Time", Controller.TotalElapsedTime);
        Controller.TotalSecondsInFullDay =
            EditorGUILayout.FloatField("Total Seconds In Full Day", Controller.TotalSecondsInFullDay);
        Controller.CurrentTimeOfDay =
            EditorGUILayout.FloatField("Current Time Of Day", Controller.CurrentTimeOfDay);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Time Of Day In 24-Hour Format:");
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField(TimeController.SimulationTimeToRealtimeString(Controller.CurrentTimeOfDay),
            EditorLabelStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Helper method for drawing the GUI for the time scaling property.
    /// </summary>
    private void TimeScaleGUI()
    {
        Controller.TimeScaleFactor = EditorGUILayout.Slider("Time Scale Factor", Controller.TimeScaleFactor, 1f, 100f);
    }
}
