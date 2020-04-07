using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor class for text reaction class/objects.
/// </summary>
[CustomEditor(typeof(TextReaction))]
public class TextReactionEditor : ReactionEditor
{
    /*
        Various properties associated with the text reaction.
    */

    private SerializedProperty MessageProperty, TextColourProperty, DelayProperty;
    private const float MessageGUILines = 3f;
    private const float AreaWidthOffset = 19f;

    protected override void Init()
    {
        MessageProperty = serializedObject.FindProperty("Message");
        TextColourProperty = serializedObject.FindProperty("textColor");
        DelayProperty = serializedObject.FindProperty("delay");
    }

    protected override void DrawReactionGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Message", GUILayout.Width(EditorGUIUtility.labelWidth - AreaWidthOffset));
        MessageProperty.stringValue = EditorGUILayout.TextArea(MessageProperty.stringValue, GUILayout.Height(EditorGUIUtility.singleLineHeight * MessageGUILines));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(TextColourProperty);
        EditorGUILayout.PropertyField(DelayProperty);
    }

    protected override string GetFoldoutLabel()
    {
        return "Text Reaction";
    }
}
