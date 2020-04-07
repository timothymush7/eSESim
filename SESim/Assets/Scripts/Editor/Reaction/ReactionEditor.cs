using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Base reaction editor class, defining the general structure for all
/// implementing reaction editor classes.
/// </summary>
public abstract class ReactionEditor : Editor
{
    private bool ShowReaction;                       // Boolean that indicates whether the editor is expanded or not
    private const float buttonWidth = 30f;          // Constant for describing the width of the buttons used in the editor
    public SerializedProperty reactionsProperty;    // Reference to parent array containing this reaction (reference done in reaction collection editor)
    private Reaction reaction;                      // Reference to the current reaction instance of editor

    private void OnEnable()
    {
        // Reference and initialisation
        reaction = (Reaction)target;
        Init();
    }

    /// <summary>
    /// Initialisation method for reaction editor.
    /// </summary>
    protected virtual void Init() { }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();

        // Add arrow to indicate whether subeditor is expanded
        ShowReaction = EditorGUILayout.Foldout(ShowReaction, GetFoldoutLabel());

        // Button for removing the reaction - remove reaction from array
        if (GUILayout.Button("-", GUILayout.Width(buttonWidth)))
            reactionsProperty.RemoveFromObjectArray(reaction);
        EditorGUILayout.EndHorizontal();

        // Should the reaction be displayed?
        if (ShowReaction)
            DrawReactionGUI();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Utility method for creating a new reaction of specified type.
    /// </summary>
    /// <param name="reactionType">Type of reaction to be created.</param>
    /// <returns>New reaction of specified type.</returns>
    public static Reaction CreateReaction(Type reactionType)
    {
        return (Reaction)CreateInstance(reactionType);
    }

    /// <summary>
    /// Method for drawing the primary GUI for the reaction.
    /// </summary>
    protected virtual void DrawReactionGUI()
    {
        DrawDefaultInspector();
    }

    /// <summary>
    /// Method for acquiring unique foldout label for the reaction.
    /// </summary>
    /// <returns>Foldout label associated with reaction.</returns>
    protected abstract string GetFoldoutLabel();
}
