/// <summary>
/// Class which defines the structure of object for containing the various
/// variations of an interaction sequence (an activity).
/// </summary>
[System.Serializable]
public class InteractionDataSequenceVariations
{
    public string listName;                                 // Unique identifier for this collection of variations for an interaction sequence
    public InteractionDataSequence[] sequenceVariations;    // Array of interaction sequences, which represent different variations of performing an interaction sequence
}
