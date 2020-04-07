/// <summary>
/// Parent interface for timer classes.
/// </summary>
public interface Timer
{
    /// <summary>
    /// Method for checking if a time-related event should be processed.
    /// </summary>
    void Update();

    /// <summary>
    /// Method for resetting the mechanism for a time-related event.
    /// </summary>
    void Reset();
}
