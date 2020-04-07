using UnityEngine;

/// <summary>
/// Utility, singleton, helper class. This was made as a component to expose the seed to the unity
/// editor and initialise the RNG, using the specified seed, at runtime.
/// </summary>
public class RNG : Singleton<RNG>
{
    /*
	 	This class was made since the Random class needed to be
		initialised at runtime using the specified seed.

	 	This class should probably be refactored into another game object,
		as the Random class is already static by default (without initialisation).
	*/

    [SerializeField] private int seed = 0;

    public override void Awake()
    {
        base.Awake();

        // Initialise the random class with the specified seed.
        Random.InitState(seed);
    }

    /// <summary>
    /// Utility method for acquiring a random number between the specified
    /// thresholds.
    /// </summary>
    /// <param name="min">Minimum threshold for random number (inclusive).</param>
    /// <param name="max">Maximum threshold for random number (inclusive).</param>
    /// <returns>A random number between the specified thresholds.</returns>
    public float Range(float min, float max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// Utility method for acquiring a random number between the specified
    /// thresholds.
    /// </summary>
    /// <param name="min">Minimum threshold for random number (inclusive).</param>
    /// <param name="max">Maximum threshold for random number (inclusive).</param>
    /// <returns>A random number between the specified thresholds.</returns>
    public int Range(int min, int max)
    {
        return Random.Range(min, max);
    }

}
