using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor class for interactable class/component.
/// </summary>
[CustomEditor(typeof(Interactable))]
public class InteractableEditor : Editor
{
    /*
        Properties associated with interactable component.
    */

    private SerializedProperty interactionLocationProperty, reactionsProperty;

    private void OnEnable()
    {
        // Reference various properties of interactable class
        interactionLocationProperty = serializedObject.FindProperty("InteractionLocation");
        reactionsProperty = serializedObject.FindProperty("Reactions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // For displaying the script (for quick edit if need)
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Interactable)target),
            typeof(Interactable), false);
        GUI.enabled = true;

        // Display remaining properties
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(reactionsProperty);
        EditorGUILayout.PropertyField(interactionLocationProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
