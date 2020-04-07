using UnityEngine;

/// <summary>
/// Singleton class/component which handles the notion of date
/// during the simulation. The date is typically specified via
/// BT activity scripts, which may model activity performances
/// for different days.
/// </summary>
public class DateController : Singleton<DateController>
{
    [Tooltip("The current year which is simulated.")] public int Year;
    [Tooltip("The current month which is simulated.")] public int Month;
    [Tooltip("The current day which is simulated.")] public int Day;

    public const int NUMBER_OF_MONTHS_IN_YEAR = 12;         // Constant describing the number of months in a year
    public const int NUMBER_OF_DAYS_IN_NORMAL_YEAR = 365;   // Constant describing the number of days in a normal year
    public const int NUMBER_OF_DAYS_IN_LEAP_YEAR = 366;     // Constant describing the number of days in a leap year

    // Arrays representing the number of days in the various months (both normal + leap years)
    public static int[] DaysInMonthNormalYear = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    public static int[] DaysInMonthLeapYear = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    public void Start()
    {
        // Listen for day pass event from Time Controller
        if (TimeController.Instance)
            TimeController.Instance.OnDayPassed += OnDayPassed;
        else
            Debug.LogError("Time controller is not defined in the scene.");
    }

    /// <summary>
    /// Primary callback which updates the current date based on time passing
    /// from the time controller.
    /// </summary>
    public void OnDayPassed()
    {
        if (System.DateTime.IsLeapYear(Year))
            ProcessDayPassed(DaysInMonthLeapYear);
        else
            ProcessDayPassed(DaysInMonthNormalYear);
    }

    /// <summary>
    /// Helper method which updates the date based on a single day passing.
    /// </summary>
    /// <param name="daysOfMonth">Array describing the number of days in a month.</param>
    private void ProcessDayPassed(int[] daysOfMonth)
    {
        // If we haven't reached maximum days for the month - increment days
        if (Day != daysOfMonth[Month - 1])
        {
            Day++;
        }
        else
        {
            // Reset day for the new month
            Day = 1;

            // If we haven't reached maximum months for the year - increment month
            if (Month != NUMBER_OF_MONTHS_IN_YEAR)
            {
                Month++;
            }
            else
            {
                // ... Otherwise reset month and increment years
                Year++;
                Month = 1;
            }
        }
    }

    /// <summary>
    /// Helper method which returns the current date as a string in the
    /// format: "DD/MM/YYYY".
    /// </summary>
    /// <returns>String describing the current date.</returns>
    public string GetCurrentDateString()
    {
        return Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString();
    }

    /// <summary>
    /// Helper method which returns the date using a DateTime object.
    /// </summary>
    /// <param name="resultDateTime">DateTime object which is updated with the current date.</param>
    /// <returns>DateTime object with the current date.</returns>
    public System.DateTime GetCurrentDate(System.DateTime resultDateTime)
    {
        return new System.DateTime(Year, Month, Day, resultDateTime.Hour, resultDateTime.Minute, resultDateTime.Second);
    }
}
