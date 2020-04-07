using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Child implementation of Timer class. This object mimics
/// a stopwatch timer, and consists of a cooldown poll event.
/// Once the timer has elapsed for a specified duration, the
/// event will be called.
/// </summary>
public class StopwatchTimer : Timer
{
    public UnityAction OnDelayElapsed;      // Event which is processed when the specified delay has elapsed.
    protected float StopwatchDelay = 5f;    // Delay which determines when the stopwatch event is processed.
    protected float InitialTimeStamp;       // Initial time stamp for timer, which is used for checking if the specified delay has elapsed.
    private bool HasElapsed = false;        // Boolean which indicates whether the timer's specified delay has elapsed.

    public StopwatchTimer(float stopwatchDelay)
    {
        StopwatchDelay = stopwatchDelay;
    }

    public void Update()
    {
        if (!HasElapsed)
            UpdateTimer();
    }

    /// <summary>
    /// Helper method which checks if the stopwatch delay event
    /// should be processed based on elapsed time.
    /// </summary>
    private void UpdateTimer()
    {
        /*
            I have explicitly calculated and referenced the difference
            between current time and last time stamp, as this is useful
            debugging information.
        */

        // Has specified duration passed?
        float differenceSinceLastTimeStamp = Time.time - InitialTimeStamp;
        if (differenceSinceLastTimeStamp >= StopwatchDelay)
        {
            HasElapsed = true;

            if (OnDelayElapsed != null)
                OnDelayElapsed();
        }
    }

    /// <summary>
    /// Helper method which sets the delay for the stopwatch timer.
    /// </summary>
    /// <param name="stopwatchDelay">Delay before stopwatch processes event.</param>
    /// <param name="reset">Boolean which indicates whether stopwatch should be reset.</param>
    public void SetDuration(float stopwatchDelay, bool reset)
    {
        StopwatchDelay = stopwatchDelay;

        if (reset)
            Reset();
    }

    /// <summary>
    /// Helper method which resets the stopwatch timer.
    /// </summary>
    public void Reset()
    {
        InitialTimeStamp = Time.time;
        HasElapsed = false;
    }

    /// <summary>
    /// Utility method which returns whether the stopwatch is enabled.
    /// </summary>
    /// <returns>True if stopwatch is active. False if otherwise.</returns>
    public bool HasTimerElapsed()
    {
        return HasElapsed;
    }
}
