using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Timer class implementation which uses a polling mechanism.
/// The poll event is continuously processed after a specified duration
/// has elapsed.
/// </summary>
public class PollTimer : Timer
{
    public UnityAction OnTimerPoll;     // Event which is processed after a specified duration has elapsed.
    protected float PollDelay = 5f;     // Delay before polling event takes place (in seconds)
    protected float PollTimeStamp;      // Initial time stamp which is used for checking if a specified delay has elapsed
    protected bool IsRunning = false;   // Boolean which indicates whether the poll timer is active.

    public PollTimer(float pollDelay)
    {
        PollDelay = pollDelay;
        Reset();
    }

    /// <summary>
    /// Primary utility method which checks if a poll event should be processed.
    /// </summary>
    public void Update()
    {
        if (IsRunning)
            UpdateTimer();
    }

    /// <summary>
    /// Helper method which checks whether the timer poll event
    /// should be processed based on elapsed time.
    /// </summary>
    private void UpdateTimer()
    {
        // Has specified duration elapsed?
        if (Time.time - PollTimeStamp >= PollDelay)
        {
            Reset();

            // Notify listeners
            if (OnTimerPoll != null)
                OnTimerPoll();
        }
    }

    /// <summary>
    /// Utility method which enables the timer.
    /// </summary>
    public void Play()
    {
        IsRunning = true;
    }

    /// <summary>
    /// Utility method which disables the timer.
    /// </summary>
    public void Pause()
    {
        IsRunning = false;
    }

    /// <summary>
    /// Utility method which resets the timer mechanism for polls.
    /// </summary>
    public void Reset()
    {
        PollTimeStamp = Time.time;
    }

    /// <summary>
    /// Helper method which returns whether the timer is enabled.
    /// </summary>
    /// <returns>True if timer is enabled. False if otherwise.</returns>
    public bool IsTimerRunning()
    {
        return IsRunning;
    }
}
