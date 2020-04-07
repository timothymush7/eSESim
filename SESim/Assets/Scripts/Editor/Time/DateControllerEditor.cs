using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor class for the date controller class/component.
/// </summary>
[CustomEditor(typeof(DateController))]
public class DateControllerEditor : Editor
{
    private DateController Controller;      // Current instance associated with the editor.
    private GUIStyle EditorLabelStyle;      // Style defined for the overall date GUI label.

    private void OnEnable()
    {
        // Reference instance
        Controller = (DateController)target;

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
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((DateController)target),
            typeof(DateController), false);
        GUI.enabled = true;

        // Integer sliders for controlling date figures
        Controller.Year = EditorGUILayout.IntField("Year", Controller.Year);
        Controller.Month = EditorGUILayout.IntSlider("Month", Controller.Month, 1,
            DateController.NUMBER_OF_MONTHS_IN_YEAR);

        // Cater for leap years...
        if (Controller.Year % 4 == 0)
            Controller.Day = EditorGUILayout.IntSlider("Day", Controller.Day, 1,
                DateController.DaysInMonthLeapYear[Controller.Month - 1]);
        else
            Controller.Day = EditorGUILayout.IntSlider("Day", Controller.Day, 1,
                DateController.DaysInMonthNormalYear[Controller.Month - 1]);

        // Example text to show what end result looks like
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        EditorGUILayout.LabelField(Controller.Day.ToString() + "/"
            + Controller.Month.ToString() + "/"
            + Controller.Year.ToString(), EditorLabelStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }
}
