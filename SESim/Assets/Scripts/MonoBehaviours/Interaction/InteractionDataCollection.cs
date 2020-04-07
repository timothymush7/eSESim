using UnityEngine;

/// <summary>
/// Class/component which manages access to sequences of interaction-data.
/// These sequences of interaction-data represent a variation of performing
/// an activity. These sequences are used to define inhabitant behaviour.
/// </summary>
public class InteractionDataCollection : Singleton<InteractionDataCollection>
{
    [Tooltip("Asset containing interaction-data sequences.")] public InteractionDataCollectionAsset interactionDataSequencesCollection;

    /// <summary>
    /// Primary utility method for acquiring interaction-data sequences from the asset.
    /// This method acquires a random sequence from a listname key in the asset.
    /// </summary>
    /// <param name="listName">Key which is used to identify a list of interaction-data sequences.</param>
    /// <param name="sequence">The referenced sequence of interaction-data.</param>
    /// <returns></returns>
    public bool TrySelectRandomSequenceFromList(string listName, out InteractionData[] sequence)
    {
        // Get all sequences of the list name
        sequence = null;
        var sequencesOfListName = interactionDataSequencesCollection.
            GetSequencesUsingListName(listName);

        if (sequencesOfListName != null)
        {
            if (RNG.Instance)
            {
                // Reference a random sequence from the list
                int randomVariationIndex = Random.Range(0, sequencesOfListName.sequenceVariations.Length);
                sequence = sequencesOfListName.sequenceVariations[randomVariationIndex].sequence;
                return true;
            }

            Debug.LogError("RNG is not defined in the scene.");
            return false;
        }

        Debug.LogError("List name: '" + listName + "' was not found in the IDC asset.");
        return false;
    }
}
