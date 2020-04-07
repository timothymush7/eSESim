using UnityEngine;

/// <summary>
/// Class representing a collection of reactions that occur when all conditions of
/// a collection are satisfied.
/// </summary>
public class ReactionCollection : MonoBehaviour
{
    [Tooltip("Reactions associated with this collection.")] public Reaction[] Reactions = new Reaction[0];

    private void Start()
    {
        // Initialise all reactions
        for (int i = 0; i < Reactions.Length; i++)
            if (Reactions[i] != null)
                Reactions[i].Init();
    }

    /// <summary>
    /// Primary utility method for processing a reaction with a specific description.
    /// </summary>
    /// <param name="reactionDescription">Unique identifier for reactions.</param>
    public void ReactSpecific(string reactionDescription)
    {
        for (int i = 0; i < Reactions.Length; i++)
        {
            if (Reactions[i].Description.Equals(reactionDescription))
            {
                Reactions[i].React();
                return;
            }
        }
    }

    /// <summary>
    /// Primary utility method which processes all reactions in the collection.
    /// </summary>
    public void ReactAll()
    {
        for (int i = 0; i < Reactions.Length; i++)
            Reactions[i].React();
    }
}
