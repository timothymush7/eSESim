using UnityEngine;

/// <summary>
/// Child extension of sensor class which includes a polling mechanism.
/// </summary>
public abstract class PollSensor : Sensor
{
    [Header("Poll Sensor Attributes")]
    [Tooltip("Rate (in seconds) at which the sensor object polls.")] public float SensorPollRate = 1f;
    [Tooltip("Boolean which indicates whether the polls start as soon as the scene is started.")] public bool PollOnStart = false;
    private PollTimer PollTimer;   // Timer object which manages the polling of the sensor object

    protected virtual void Start()
    {
        PollTimer = new PollTimer(SensorPollRate);
        PollTimer.OnTimerPoll += Poll;
        TogglePolling(PollOnStart, true);
    }

    /// <summary>
    /// Parent method that handles value calculations for sensor data and the
    /// publishing of sensor data to sensor stream.
    /// </summary>
    protected virtual void Poll()
    {
        // may or may not be implemented by deriving class
    }

    /// <summary>
    /// Helper method which toggles the timer on or off.
    /// </summary>
    /// <param name="startPoll">Boolean which indicates whether the timer should be activated.</param>
    /// <param name="resetTime">Boolean which indicates whether the timer should be reset.</param>
    public virtual void TogglePolling(bool startPoll, bool resetTime)
    {
        if (startPoll)
            PollTimer.Play();
        else
            PollTimer.Pause();

        if (resetTime)
            PollTimer.Reset();
    }

    protected virtual void Update()
    {
        PollTimer.Update();
    }
}
