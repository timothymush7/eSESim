using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor class for the interaction data collection asset. This
/// class describes the editor GUI for the interaction data collection asset
/// and includes the functional logic for supporting the interaction data
/// collection asset.
/// </summary>
[CustomEditor(typeof(InteractionDataCollectionAsset))]
public class InteractionDataCollectionAssetEditor : Editor
{
    private InteractionDataCollectionAsset instance;        // Instance of the interaction data collection asset
    private int ListNameIndex = 0;                          // Index for selecting a list name in the dictionary GUI
    private int InteractionDataSequenceIndex = 0;           // Index for selecting a interaction sequence in the dictionary GUI

    private void OnEnable()
    {
        instance = (InteractionDataCollectionAsset)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw base GUI and custom GUI...
        base.OnInspectorGUI();
        DrawDictionaryGUI();
        DrawButtonGUI();

        if (GUI.changed)
            EditorUtility.SetDirty(instance);
    }

    /// <summary>
    /// Helper method for drawing the dictionary GUI for interaction data sequences.
    /// This allows the user to select a specific list name and thus the interaction
    /// data sequences associated with the list name would be displayed.
    /// </summary>
    private void DrawDictionaryGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        if (instance.DoesAssetDataExist())
        {
            if (instance.DoesAssetContainAnyData())
            {
                ListNameIndex = EditorGUILayout.Popup("List Name", ListNameIndex, instance.GetListNames());

                InteractionDataSequence[] interactionDataSequences = instance.GetInteractionDataSequences(ListNameIndex);

                if (interactionDataSequences != null)
                {
                    if (interactionDataSequences.Length > 0)
                    {
                        InteractionDataSequenceIndex = EditorGUILayout.IntSlider("Queue #",
                            InteractionDataSequenceIndex, 0, interactionDataSequences.Length - 1);

                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        InteractionData[] interactionDataArray = interactionDataSequences[InteractionDataSequenceIndex].sequence;

                        for (int i = 0; i < interactionDataArray.Length; i++)
                            EditorGUILayout.TextField(interactionDataArray[i].Description);
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("No data found in the asset.");
                EditorGUILayout.Space();
            }
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Asset data not defined.");
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Helper method for generating a button in the editor GUI.
    /// This button specifically processes interaction data from the specified text assets.
    /// </summary>
    private void DrawButtonGUI()
    {
        if (GUILayout.Button("Process Interaction Data From Text Assets"))
        {
            ProcessInteractionDataFromTextAssets();
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// Helper method which process the interaction data from the specified text assets
    /// </summary>
    private void ProcessInteractionDataFromTextAssets()
    {
        TextAsset[] textAssets = instance.TextAssets;
        if (textAssets != null)
        {
            InteractionDataSequenceJSON[] rawInteractionDataSequences = new InteractionDataSequenceJSON[textAssets.Length];
            string[] listNames = new string[textAssets.Length];

            // Parse text from JSON files
            for (int i = 0; i < textAssets.Length; i++)
            {
                InteractionDataSequenceJSON data = JsonUtility.FromJson<InteractionDataSequenceJSON>(textAssets[i].text);
                rawInteractionDataSequences[i] = data;
                listNames[i] = data.ListName;
            }

            // Generate collection of variations of interaction sequences
            instance.SetInteractionDataSequencesCollection(GenerateInteractionDataCollection(rawInteractionDataSequences));
            instance.SetListNames(listNames);
        }
        else
            Debug.LogError("No text assets are attached.");
    }

    /// <summary>
    /// Helper method which generates the collection of variations of interaction sequences
    /// from the raw interaction data sequences from the text files.
    /// </summary>
    /// <param name="interactionDataSequencesFromJSONFiles">Interaction data sequences from JSON files.</param>
    /// <returns>A newly generated collection of variations of interaction sequences.</returns>
    private InteractionDataSequenceVariations[] GenerateInteractionDataCollection(InteractionDataSequenceJSON[] interactionDataSequencesFromJSONFiles)
    {
        var sequencesCollection = new InteractionDataSequenceVariations[interactionDataSequencesFromJSONFiles.Length];

        for (int i = 0; i < interactionDataSequencesFromJSONFiles.Length; i++)
        {
            // Reference array + properties from interaction sequence in JSON files
            var anInteractionDataArray = interactionDataSequencesFromJSONFiles[i].InteractionDataArray;
            bool generateSequenceVariations = interactionDataSequencesFromJSONFiles[i].GenerateSequenceVariations;

            // Generate the variations of interaction sequences
            List<InteractionData[]> generatedSequenceVariations =
                InteractionDataMethods.GetInteractionDataSequenceVariations(anInteractionDataArray,
                instance.GenerateVariationAttempts, instance.MaxConflictThreshold, generateSequenceVariations);

            // Define collection of sequences...
            sequencesCollection[i] = new InteractionDataSequenceVariations
            {
                listName = interactionDataSequencesFromJSONFiles[i].ListName,
                sequenceVariations = new InteractionDataSequence[generatedSequenceVariations.Count]
            };

            // ... and fill it with the generated variations of interaction sequences
            for (int j = 0; j < generatedSequenceVariations.Count; j++)
            {
                sequencesCollection[i].sequenceVariations[j] = new InteractionDataSequence
                {
                    sequence = generatedSequenceVariations[j]
                };
            }
        }

        return sequencesCollection;
    }

    /// <summary>
    /// Helper method for creating an interaction data collection asset.
    /// This menu option is accessible in the Unity editor.
    /// </summary>
    [MenuItem("Assets/Create/Interaction Data Collection Asset")]
    private static void CreateInteractionDataCollectionAsset()
    {
        // Only create asset if it doesn't exist already in resources folder
        if (Resources.Load<InteractionDataCollectionAsset>(InteractionDataCollectionAsset.AssetLoadPath) == null)
        {
            InteractionDataCollectionAsset newAsset = CreateInstance<InteractionDataCollectionAsset>();
            AssetDatabase.CreateAsset(newAsset, InteractionDataCollectionAsset.AssetCreationPath);
            AssetDatabase.SaveAssets();
        }
    }
}
