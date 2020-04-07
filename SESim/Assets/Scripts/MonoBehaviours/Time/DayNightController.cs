using UnityEngine;

/// <summary>
/// Class/component which controls the position of a directional light, mimicking
/// the behavior of a sun to allow a day-night cycle.
/// 
/// Credit: TwiiK.net (http://twiik.net/articles/simplest-possible-day-night-cycle-in-unity-5)
/// </summary>
public class DayNightController : Singleton<DayNightController>
{
    /*
        This component is currently a singleton primarily to enable light sensors
        to query for the light intensity of the sun (or ambient light intensity).

        If this functionality is not necessary or an alternative is designed, then
        the singleton implementation can be removed.
    */

    [Tooltip("Reference to the directional light game object acting as the sun.")] public Light SunLightComponent;
    [Tooltip("Reference value for indicating the initial light intensity of the sun")] private float SunInitialIntensity;

    /*
        Current time of day is a value ranging between 0-1, where 0 represents
        the start of the day and 1 represents the end of the day respectively.
        
        At default: sunrise is at 0.25, noon is at 0.5, sunset is at 0.75, and midnight at 1
    */
    private float CurrentTimeOfDay = 0f;                                            // Current time of day (see above comment)

    [Range(0, 1)] public float TimeAtSunrise, TimeAtNoon, TimeAtSunset;             // Thresholds for defining sunrise, noon, and sunset.
    [Range(0, 1)] public float SunIntensityBeforeSunrise, SunIntensityAfterSunset;  // Light intensity of sun during time of day.   

    void Start()
    {
        if (SunLightComponent)
            SunInitialIntensity = SunLightComponent.intensity;
        else
            Debug.LogError("Light component for sun is not specified.");
    }

    void Update()
    {
        // Update time and thus sun position
        UpdateCurrentTimeOfDay();
        UpdateSunPositionAndIntensity();
    }

    /// <summary>
    /// Helper method which references the current time of day from the time controller.
    /// </summary>
    private void UpdateCurrentTimeOfDay()
    {
        if (TimeController.Instance)
            CurrentTimeOfDay = TimeController.Instance.CurrentTimeOfDay;
    }

    private void UpdateSunPositionAndIntensity()
    {
        /*
            Update the rotation of the sun, based on current time
            
            1.) Rotation on x-axis according to current time of day.
                Subtracted 90 degrees to make sun rise at 0.25 
                instead of 0 (ease of working in degrees)
                
            2.) Y-axis defines where on the horizon will the sun rise
                and set. Can be changed to suit the scene
                
            3.) Z-axis does nothing, therefore 0.
        */

        SunLightComponent.transform.localRotation =
            Quaternion.Euler((CurrentTimeOfDay * 360f) - 90, 170, 0);

        /*
            So basically this section deals with changing the intensity
            of the sun based on the current time of day, which ranges from 0 to 1.

            - Before sunrise = 0
            - Between sunrise and noon = gradually increases from 0 to 1
            - Between noon and sunset = gradually decreases from 1 to 0
            - After sunset = 0
        */

        float intensityMultiplier = 1;

        // Set intensity for before sunrise
        if (CurrentTimeOfDay <= TimeAtSunrise)
            intensityMultiplier = SunIntensityBeforeSunrise;

        // Set intensity for after sunset
        else if (CurrentTimeOfDay >= TimeAtSunset)
            intensityMultiplier = SunIntensityAfterSunset;

        // Calculate intensity between sunrise and noon
        else if ((CurrentTimeOfDay >= TimeAtSunrise) && (CurrentTimeOfDay <= TimeAtNoon))
            intensityMultiplier = Mathf.Clamp01(((0.25f - (TimeAtNoon - CurrentTimeOfDay)) / 0.25f)) * SunInitialIntensity;

        // Calculate intensity between noon and sunset
        else if ((CurrentTimeOfDay >= TimeAtNoon) && (CurrentTimeOfDay <= TimeAtSunset))
            intensityMultiplier = Mathf.Clamp01((((TimeAtSunset - CurrentTimeOfDay)) / 0.25f)) * SunInitialIntensity;

        // Update sun intensity using calculated intensity multiplier
        SunLightComponent.intensity = SunInitialIntensity * intensityMultiplier;
    }

    /// <summary>
    /// Helper method which acquires the ambient light in the scene (the sun).
    /// This is used by light sensors which require a measurement of ambient light
    /// for measuring light intensity in a specific position.
    /// </summary>
    /// <returns>Ambient light from the sun game object.</returns>
    public float GetAmbientLightIntensity()
    {
        /*
            The ambient light from the render settings seemed very small or insignificant.
            Thus I decided to just use the light intensity of the light component in the sun
            game object.
        */

        return SunLightComponent.intensity;
        // return RenderSettings.ambientIntensity;
    }
}
