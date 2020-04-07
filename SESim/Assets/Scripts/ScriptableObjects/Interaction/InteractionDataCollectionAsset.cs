using UnityEngine;

/// <summary>
/// Scriptable object which contains a collection of interaction data sequences and their variations.
/// </summary>
public class InteractionDataCollectionAsset : ScriptableObject
{
    [Header("Interaction Data Text Assets")]
    [Tooltip("Array of JSON text files where interaction data is loaded from.")] public TextAsset[] TextAssets;

    [Header("Interaction Data Asset Setup Attributes")]
    [Tooltip("Number of attempts for generating variations of interaction sequences.")] public int GenerateVariationAttempts = 100;
    [Tooltip("Maximum threshold of encountering duplicate variations before stopping.")] public int MaxConflictThreshold = 100;

    /*
        Used array at the time, but a dictionary would be even better.
        Consider implementing this change.
    */

    private InteractionDataSequenceVariations[] SequencesCollection;                                // Collection of variations for interaction sequences
    private string[] ListNamesForSequencesCollection;                                               // list names found in the collection of variations for interaction sequences
    public static string AssetLoadPath = "InteractionDataCollection";                               // Unique string which indicates the load path for the interaction data collection asset
    public static string AssetCreationPath = "Assets/Resources/InteractionDataCollection.asset";    // Unique string which indicates the creation path for the interaction data collection asset

    /// <summary>
    /// Primary utility method which returns the variations of an interaction data sequence
    /// associated with a specified list name.
    /// </summary>
    /// <param name="listName">Unique identifier for variations of an interaction data sequence.</param>
    /// <returns>Variations of interaction data sequence with specified list name.</returns>
    public InteractionDataSequenceVariations GetSequencesUsingListName(string listName)
    {
        for (int i = 0; i < SequencesCollection.Length; i++)
            if (SequencesCollection[i].listName.Equals(listName))
                return SequencesCollection[i];
        return null;
    }

    /// <summary>
    /// Helper method for acquiring the variations of a interaction sequence using
    /// a specific list name index. This is primarily used for drawing the GUI
    /// of the IDCA editor.
    /// </summary>
    /// <param name="listNameIndex">Index of list name for the collection of variations for interaction sequences.</param>
    /// <returns>Variations of an interaction sequence.</returns>
    public InteractionDataSequence[] GetInteractionDataSequences(int listNameIndex)
    {
        string listName = ListNamesForSequencesCollection[listNameIndex];

        if (SequencesCollection == null)
            return null;

        for (int i = 0; i < SequencesCollection.Length; i++)
            if (SequencesCollection[i] != null)
                if (SequencesCollection[i].listName.Equals(listName))
                    return SequencesCollection[i].sequenceVariations;

        return null;
    }

    /// <summary>
    /// Helper method for setting/updating the collection of variations for interaction sequences.
    /// </summary>
    /// <param name="sequenceCollection">A new collection of variations for interaction sequences.</param>
    public void SetInteractionDataSequencesCollection(InteractionDataSequenceVariations[] sequencesCollection)
    {
        SequencesCollection = sequencesCollection;
    }

    /// <summary>
    /// Helper method for setting/updating the list names associated with the collection of variations
    /// for interaction sequences.
    /// </summary>
    /// <param name="listNames">List names associated with a collection of variations for interaction sequences.</param>
    public void SetListNames(string[] listNames)
    {
        ListNamesForSequencesCollection = listNames;
    }

    /// <summary>
    /// Helper method which returns the list names associated with the collection of variations
    /// for interaction sequences.
    /// </summary>
    /// <returns>List names associated with the collection of variations for interaction sequences.</returns>
    public string[] GetListNames()
    {
        return ListNamesForSequencesCollection;
    }

    /// <summary>
    /// Helper method returns whether the asset is defined.
    /// </summary>
    /// <returns>True if collection is defined. False if otherwise.</returns>
    public bool DoesAssetDataExist()
    {
        return SequencesCollection != null;
    }

    /// <summary>
    /// Helper method which returns whether the asset contains data.
    /// </summary>
    /// <returns>True if at least one variation of interaction sequences exists. False if otherwise.</returns>
    public bool DoesAssetContainAnyData()
    {
        return SequencesCollection.Length > 0;
    }
}
