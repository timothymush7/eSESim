using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Editor class for the reaction collection class.
/// </summary>
[CustomEditor(typeof(ReactionCollection))]
public class ReactionCollectionEditor : EditorWithSubEditors<ReactionEditor, Reaction>
{
    private ReactionCollection ReactionCollection;          // Current instance of the reaction collection for editor.
    private SerializedProperty ReactionsProperty;           // Serialized property of reactions array.
    private Type[] ReactionTypes;                           // Array containing types of all possible reactions defined.
    private string[] ReactionTypeNames;                     // Array containing names of all possible reaction types defined.
    private int SelectedIndex;                              // Property which is maintained to identify the currently selected index in the editor

    private void OnEnable()
    {
        // Reference current instance, properties of the serialized object (should match name in reaction collection)
        ReactionCollection = (ReactionCollection)target;
        ReactionsProperty = serializedObject.FindProperty("Reactions");
        RecreateSubEditors(ReactionCollection.Reactions);
        SetReactionNamesArray();
    }

    private void OnDisable()
    {
        DestroySubEditors();
    }

    /// <summary>
    /// Helper method which does the setup for a specified subeditor.
    /// </summary>
    /// <param name="editor">Subeditor to be setup.</param>
    protected override void SubEditorSetup(ReactionEditor editor)
    {
        if (editor)
            editor.reactionsProperty = ReactionsProperty;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Recreate subeditors if needed, draw subeditors.
        RecreateSubEditors(ReactionCollection.Reactions);
        for (int i = 0; i < subEditors.Length; i++)
            if (subEditors[i] != null)
                subEditors[i].OnInspectorGUI();
        if (ReactionCollection.Reactions.Length > 0)
            EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        // Pop-up to select appropriate reaction for adding purposes
        SelectedIndex = EditorGUILayout.Popup(SelectedIndex, ReactionTypeNames);

        // Button to add selected reaction
        if (GUILayout.Button("Add Selected Reaction"))
        {
            Type reactionType = ReactionTypes[SelectedIndex];
            Reaction newReaction = ReactionEditor.CreateReaction(reactionType);
            ReactionsProperty.AddToObjectArray(newReaction);
        }

        // Button to remove undefined/null reactions
        if (GUILayout.Button("Remove Null"))
        {
            // Iterate through collection until all reactions removed
            while (AreThereNullReactions())
            {
                for (int j = 0; j < ReactionCollection.Reactions.Length; j++)
                {
                    if (ReactionCollection.Reactions[j] == null)
                    {
                        ReactionsProperty.RemoveFromObjectArrayAt(j);
                        break;
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Helper method for checking if there are undefined reactions in the collection.
    /// </summary>
    /// <returns>True if undefined reactions exist. False if otherwise.</returns>
    private bool AreThereNullReactions()
    {
        for (int i = 0; i < ReactionCollection.Reactions.Length; i++)
            if (ReactionCollection.Reactions[i] == null)
                return true;
        return false;
    }

    /// <summary>
    /// Helper method which references all the names of reactions.
    /// </summary>
    private void SetReactionNamesArray()
    {
        // Use reflection to get the list of all reaction types
        Type reactionType = typeof(Reaction);
        Type[] allTypes = reactionType.Assembly.GetTypes();
        List<Type> reactionSubTypeList = new List<Type>();

        // Filter out non-reaction implementations or abstract reactions
        for (int i = 0; i < allTypes.Length; i++)
            if (allTypes[i].IsSubclassOf(reactionType) && !allTypes[i].IsAbstract)
                reactionSubTypeList.Add(allTypes[i]);

        // Initialise string list to contain the reaction sub type names
        List<string> reactionTypeNameList = new List<string>();

        // Iterate through reaction sub-types, add names to list
        ReactionTypes = reactionSubTypeList.ToArray();
        for (int i = 0; i < ReactionTypes.Length; i++)
            reactionTypeNameList.Add(ReactionTypes[i].Name);

        // Reference reaction type names as array
        ReactionTypeNames = reactionTypeNameList.ToArray();
    }
}
