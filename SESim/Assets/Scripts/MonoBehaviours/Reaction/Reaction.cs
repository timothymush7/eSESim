using UnityEngine;

/// <summary>
/// Base class representing a reaction. Implementing functions utilise
/// different initialisation functions and approaches of executing reactions.
/// </summary>
public abstract class Reaction : ScriptableObject
{
    public string Description;      // Description which describes the reaction

    /// <summary>
    /// Initialisation method for reactions.
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// Utility method for processing the reaction.
    /// </summary>
    public virtual void React()
    {

    }
}
