/// <summary>
/// A class that contains the metadata for describing an interaction
/// between an inhabitant and a home game object.
/// </summary>
[System.Serializable]
public class InteractionData
{
    public string Description;                      // Unique identifier for the interaction
    public string TargetGameObjectName;             // Name of game object, which is associated with the interaction
    public string ReactionCollectionDescription;    // Unique identifier for reactions associated with the interaction
    public float InitialInteractionDuration;        // Default/initial duration of the interaction when performed
    public string[] DependencyDescriptions;         // Array of interaction descriptions which are required to be processed prior to this interaction

    public override bool Equals(object obj)
    {
        return Description.Equals(((InteractionData)obj).Description);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
