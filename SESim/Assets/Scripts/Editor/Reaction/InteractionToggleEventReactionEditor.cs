using UnityEditor;

/// <summary>
/// Editor class for the interaction toggle event reaction class.
/// </summary>
[CustomEditor(typeof(InteractionToggleEventReaction))]
public class InteractionToggleEventReactionEditor : ReactionEditor
{
    /*
        Various serialized properties associated with the interaction toggle event reaction.
    */

    private SerializedProperty descriptionProperty, toggleEventIdentifierProperty, toggleProperty, interactableProperty;

    protected override void Init()
    {
        descriptionProperty = serializedObject.FindProperty("Description");
        toggleEventIdentifierProperty = serializedObject.FindProperty("ToggleEventIdentifier");
        toggleProperty = serializedObject.FindProperty("Toggle");
        interactableProperty = serializedObject.FindProperty("Interactable");
    }

    protected override void DrawReactionGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(descriptionProperty);
        EditorGUILayout.PropertyField(toggleEventIdentifierProperty);
        EditorGUILayout.PropertyField(toggleProperty);
        EditorGUILayout.PropertyField(interactableProperty);
        EditorGUILayout.EndVertical();
    }

    protected override string GetFoldoutLabel()
    {
        return "Interaction Toggle Event Reaction";
    }
}
