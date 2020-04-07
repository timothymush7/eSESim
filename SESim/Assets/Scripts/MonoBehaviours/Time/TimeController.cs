using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton class/component which contains the functional logic for managing
/// the notion of time during runtime.
/// </summary>
public class TimeController : Singleton<TimeController>
{
    /*
        CurrentTimeOfDay is a value indicating the time of day.

        In particular, there are specific points in the range that indicate
        different parts of the day:
             0       - Midnight
             0.25    - Sunrise (Assumed as 6 AM)
             0.5     - Noon
             0.75    - Sunset (Assumed as 6 PM)
    */

    [Tooltip("The current time during simulations.")] [Range(0f, 1f)] public float CurrentTimeOfDay = 0f;
    [Tooltip("Initial time set for simulations")] [Range(0, 1)] public float InitialTimeOfDay;
    [Tooltip("Initial time (hours) set for simulations.")] public int InitialTimeHours = 0;
    [Tooltip("Initial time (minutes) set for simulations.")] public int InitialTimeMinutes = 0;
    [Tooltip("Initial time (seconds) set for simulations.")] public int InitialTimeSeconds = 0;
    [Tooltip("The total amount of time that has passed.")] public float TotalElapsedTime = 0f;
    [Tooltip("The number of seconds per day in simulation.")] public float TotalSecondsInFullDay = 120f;

    /*
        Read here to understand time scaling: https://docs.unity3d.com/Manual/TimeFrameManagement.html
    */

    [Tooltip("The scale of time updates.")] public float TimeScaleFactor = 1f;

    public const int HOURS_IN_DAY = 24;         // Constant which describes the number of hours in one day
    public const int MINUTES_IN_HOUR = 60;      // Constant which describes the number of minutes in one hour
    public const int SECONDS_IN_MINUTE = 60;    // Constant which describes the number of seconds in one minute
    public UnityAction OnDayPassed;         // Event which represents the passing of a day

    void Start()
    {
        CurrentTimeOfDay = InitialTimeOfDay;
    }

    void Update()
    {
        UpdateTime();
        Time.timeScale = TimeScaleFactor;
    }

    /// <summary>
    /// Helper method which updates the notion of time.
    /// </summary>
    private void UpdateTime()
    {
        /*
            Advance current time of day based on a proportion of delta time with
            regards to specified duration representing a single day.

            Delta time = time since last frame completed in simulation
        */

        float changeInTime = (Time.deltaTime / TotalSecondsInFullDay);

        CurrentTimeOfDay += changeInTime;
        TotalElapsedTime += changeInTime;

        // Has a day been processed?
        if (CurrentTimeOfDay >= 1)
        {
            // Reset time of day to indicate start of new day, send event of day passing
            CurrentTimeOfDay = 0f;
            if (OnDayPassed != null)
                OnDayPassed();
        }
    }

    /// <summary>
    /// Helper method which converts an hours value, minutes value, and seconds value
    /// into simulation time (on the scale from 0 to 1).
    /// </summary>
    /// <param name="hoursValue">Hours value for simulation time.</param>
    /// <param name="minutesValue">Minutes value for simulation time.</param>
    /// <param name="secondsValue">Seconds value for simulation time.</param>
    /// <returns>Simulation time from the specified values.</returns>
    public float ConvertTimeValuesToSimulationTime(int hoursValue, int minutesValue, int secondsValue)
    {
        /*
            Just quick note on the calculations:

            CurrentTimeOfDay (simulation time) ranges on a scale from 0 to 1, 
            need to scale the hours, minutes and seconds into this range.

            After scaling each value, adding the values together will yield
            time value expressed in the range of [0, 1].
        */

        // # of hours / 24 = proportion of hours that has passed in the day
        float hoursProportion = hoursValue / (HOURS_IN_DAY * 1f);

        // # of minutes / 24 * 60 = proportion of minutes during particular hour
        float minutesProportion = (minutesValue / (MINUTES_IN_HOUR * HOURS_IN_DAY * 1f));

        // # of seconds / 24 * 60 * 60 = proportion of seconds during particular hour and minute
        float secondsProportion = (secondsValue / (SECONDS_IN_MINUTE * MINUTES_IN_HOUR * HOURS_IN_DAY * 1f));

        return hoursProportion + minutesProportion + secondsProportion;
    }

    /// <summary>
    /// Helper method which converts a string time text to simulation time.
    /// </summary>
    /// <param name="timeText">Text describing time in the format of "HH:MM:SS".</param>
    /// <returns>Simulation time from the specified text.</returns>
    public float ConvertStringTimeTextToSimulationTime(string timeText)
    {
        string[] timeValueArray = timeText.Split(':');

        // Make sure the length is correct
        if (timeValueArray.Length == 3)
        {
            // Parse values then use conversion helper method
            float hoursValue = float.Parse(timeValueArray[0]);
            float minutesValue = float.Parse(timeValueArray[1]);
            float secondsValue = float.Parse(timeValueArray[2]);
            return ConvertTimeValuesToSimulationTime((int)hoursValue, (int)minutesValue, (int)secondsValue);
        }

        return 0f;
    }

    /// <summary>
    /// Helper method which gets the current simulation time in string format.
    /// </summary>
    /// <returns>Simulation time in string format.</returns>
    public string GetCurrentTimeString()
    {
        // Calculate the time of day
        float hoursValue = HOURS_IN_DAY * CurrentTimeOfDay;
        float minutesValue = MINUTES_IN_HOUR * (hoursValue - Mathf.Floor(hoursValue));
        float secondsValue = SECONDS_IN_MINUTE * (minutesValue - Mathf.Floor(minutesValue));

        // Return the values as string
        return hoursValue.ToString("f0")
            + ":" + minutesValue.ToString("f0")
            + ":" + secondsValue.ToString("f0");
    }

    /// <summary>
    /// Helper method which scales time (in seconds) to simulation time.
    /// This is particularly used when time scale manipulation is used.
    /// </summary>
    /// <param name="timeInSeconds">Time expressed in seconds.</param>
    /// <returns>The specified time scaled according to simulation time.</returns>
    public float ScaleSecondsToSimulationTime(float timeInSeconds)
    {
        // Convert to simulation time
        float secondsInSimulationTime = (timeInSeconds / (SECONDS_IN_MINUTE * MINUTES_IN_HOUR * HOURS_IN_DAY));

        // Return scaled time value
        return secondsInSimulationTime * TotalSecondsInFullDay;
    }

    /// <summary>
    /// Helper method which is used to update the current simulation time of day to a specified time value.
    /// </summary>
    /// <param name="hoursValue">Hours value for updating current simulation time of day.</param>
    /// <param name="minutesValue">Minutes value for updating current simulation time of day.</param>
    /// <param name="secondsValue">Seconds value for updating current simulation time of day.</param>
    /// <param name="resetTime">Boolean which indicates whether the total time counter should be reset.</param>
    public void SetTimeOfDay(int hoursValue, int minutesValue, int secondsValue, bool resetTime)
    {
        CurrentTimeOfDay = ConvertTimeValuesToSimulationTime(hoursValue, minutesValue, secondsValue);
        if (resetTime)
            TotalElapsedTime = 0;
    }

    /// <summary>
    /// Helper method which is used to update the current simulation time of day to a specified time value.
    /// </summary>
    /// <param name="newTime">DateTime object for updating the current simulation time of day.</param>
    /// <param name="resetTime">Boolean which indicates whether the total time counter should be reset.</param>
    public void SetTimeOfDay(System.DateTime newTime, bool resetTime)
    {
        SetTimeOfDay(newTime.Hour, newTime.Minute, newTime.Second, resetTime);
    }

    /// <summary>
    /// Helper method which acquires the current simulation time of day.
    /// </summary>
    /// <param name="hours">Hours value of current simulation time of day.</param>
    /// <param name="minutes">Minutes value of current simulation time of day.</param>
    /// <param name="seconds">Seconds value of current simulation time of day.</param>
    public void GetCurrentTime(out int hours, out int minutes, out int seconds)
    {
        float hoursValue = HOURS_IN_DAY * CurrentTimeOfDay;
        float minutesValue = MINUTES_IN_HOUR * (hoursValue - Mathf.Floor(hoursValue));
        float secondsValue = SECONDS_IN_MINUTE * (minutesValue - Mathf.Floor(minutesValue));

        hours = (int)hoursValue;
        minutes = (int)minutesValue;
        seconds = (int)secondsValue;
    }

    /// <summary>
    /// Helper method which acquires the current simulation time of day.
    /// </summary>
    /// <param name="resultDateTime">DateTime object which will be updated with the current simulation time of day.</param>
    /// <returns>DateTime object which describes the current simulation time of day.</returns>
    public System.DateTime GetCurrentTime(System.DateTime resultDateTime)
    {
        int hours, minutes, seconds;
        GetCurrentTime(out hours, out minutes, out seconds);
        return new System.DateTime(resultDateTime.Year, resultDateTime.Month,
            resultDateTime.Day, hours, minutes, seconds);
    }

    /// <summary>
    /// Static helper method which is used to acquire the string format of a specified
    /// simulation time value.
    /// </summary>
    /// <param name="simulationTime">Simulation time value to be converted to string format.</param>
    /// <returns>String format of specified simulation time value.</returns>
    public static string SimulationTimeToRealtimeString(float simulationTime)
    {
        float rawHoursValue = simulationTime * (HOURS_IN_DAY);
        int hoursValue = Mathf.FloorToInt(rawHoursValue);

        float rawMinutesValue = (rawHoursValue - hoursValue) * (MINUTES_IN_HOUR * 1f);
        int minutesValue = Mathf.FloorToInt(rawMinutesValue);

        float rawSecondsValue = (rawMinutesValue - minutesValue) * (SECONDS_IN_MINUTE * 1f);
        int secondsValue = Mathf.RoundToInt(rawSecondsValue);

        return hoursValue + ":" + minutesValue + ":" + secondsValue;
    }
}
