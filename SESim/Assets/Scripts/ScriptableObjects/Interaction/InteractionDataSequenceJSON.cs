/// <summary>
/// This class is used to define the object structure for containing
/// interaction data from a JSON text file.
/// </summary>
[System.Serializable]
public class InteractionDataSequenceJSON
{
    /*
        These objects specifically contain the raw interaction data
        from a JSON file. The interaction data is not yet sorted
        into a sequence, nor has the variations for the interaction
        data been generated.

        This data, however, is later used by the Interaction Data Collection Asset
        editor to sort as a sequence, and generate the various variations.
    */

    public string ListName;                             // Unique identifier associated with the array of interaction data.
    public bool GenerateSequenceVariations;             // Boolean indicating if variations of the interaction data should be generated
    public InteractionData[] InteractionDataArray;      // Simply an array of interaction data (not sorted in a sequence)
}
