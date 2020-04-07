using UnityEngine;

/// <summary>
/// Class which handles the scaling of sensor values in preparation for
/// machine learning models.
/// </summary>
public static class SensorValueScaling
{
    /*
        Never used, but may be useful for future work.
    */

    public static float ScaleTemperature(float temperature, float minRange, float maxRange)
    {
        if (minRange > maxRange)
        {
            Debug.LogError("Min range is greater than max range. Returning original value.");
            return temperature;
        }

        /*
         * The scaling to [0, 1] range involves the calculation of a translation value and divisor.
         * The translation moves the bounds to be zero based, then the divisor is used to calculate
         * the proportion.
         * 
         * Example: [-50, -30]
         * -> [0, 20] with translation value = 50
         * -> [0, 1] with divisor = 20
         */

        float translationValue = -minRange;
        float divisor = maxRange + translationValue;
        return (temperature + translationValue) / divisor;
    }

    public static float ScaleLight(float intensity)
    {
        /*
         * Light intensity values in Unity range from [0, infinity)
         * 
         * Using a squashing function, the values can be squashed to a smaller range.
         * f(x) = 1 / (1 + e^-x), where 0 would yield 1/2, negative x values would
         * yield values towards 0, and positive x values would yield towards 1.
         * 
         * Thus we have values ranging from [1/2, 1). To scale the values to [0, 1) range,
         * the values can be doubled and subtracted by 1.
         */

        float squashedValue = 1 / (1 + Mathf.Exp(-intensity));
        float scaledValue = (squashedValue * 2) - 1;

        return scaledValue;
    }
}


