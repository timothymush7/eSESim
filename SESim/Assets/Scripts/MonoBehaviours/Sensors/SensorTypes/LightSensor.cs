using UnityEngine;

/// <summary>
/// This class/component contains the functional logic for the
/// light sensor model. This model summates light intensities
/// in the environment. 
/// </summary>
public class LightSensor : PollSensor
{
    [Header("Light Sensor Attributes")]
    [Tooltip("The current summation of light intensities of nearby light sources")] [ReadOnly] public float SensorValue;
    [Tooltip("Layer mask which is used to identify obstacles between the sensor and light sources.")] public LayerMask ObstacleMask;

    protected override void Poll()
    {
        UpdateSensorValue();
        GenerateFloatSensorReading(SensorValue);
    }

    /// <summary>
    /// Helper method which computes the summation of light intensities
    /// from different light sources within view of the sensor.
    /// </summary>
    private void UpdateSensorValue()
    {
        SensorValue = 0f;

        // Iterate through all point lights in the scene
        foreach (Light aLight in AllHomeObjects.Instance.GetPointLights())
        {
            // Is the light "on"?
            if (aLight.enabled)
            {
                // Compute distance between sensor and light source - no need to calculate light intensity if sensor out of range
                float distanceBetweenSensorAndLight = Vector3.Distance(transform.position, aLight.transform.position);
                if (distanceBetweenSensorAndLight <= aLight.range)
                {
                    /*
                        This calculation assumes that if an obstacle between light source and sensor,
                        then no light intensity is recorded. It is important to note
                        that this is not true in all cases.
                    */

                    // Check that an obstacle is not between light and sensor
                    if (!Physics.Linecast(transform.position, aLight.transform.position, ObstacleMask))
                        SensorValue += GetIntensityValue(aLight.intensity, aLight.range, distanceBetweenSensorAndLight);
                }
            }
        }

        SensorValue += DayNightController.Instance.GetAmbientLightIntensity();
    }

    /// <summary>
    /// Helper method for calculating the light intensity value at a 
    /// specific position within radius of the light source. This calculation is
    /// based on the inverse square law for lights.
    /// </summary>
    /// <param name="intensityAtSource">Light intensity of the light source.</param>
    /// <param name="maxIntensityRange">The maximum range of the light source.</param>
    /// <param name="currentDistanceFromSource">The distance between light source and sensor.</param>
    /// <returns></returns>
    private float GetIntensityValue(float intensityAtSource, float maxIntensityRange, float currentDistanceFromSource)
    {
        // Find ratio of current distance to the intensity source range         
        float distanceRangeScaling = Mathf.Min(maxIntensityRange, currentDistanceFromSource);
        distanceRangeScaling = distanceRangeScaling / maxIntensityRange;

        /*
            This calculation is based on inverse square law for lights, where the light intensity
            at sensor position is inversely proportional to the squared distance from sensor
            position to the light source.
        */

        float distanceSquared = distanceRangeScaling * distanceRangeScaling;
        float intensityValueAtCurrentDistance = intensityAtSource - (distanceSquared * intensityAtSource);
        return intensityValueAtCurrentDistance;
    }
}
