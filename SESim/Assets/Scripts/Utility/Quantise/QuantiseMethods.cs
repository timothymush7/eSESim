using System.Collections.Generic;

/// <summary>
/// Class which provides helper methods specifically for quantising
/// sensor readings, within a sample, of a specific sensor type.
/// </summary>
public static class QuantiseMethods
{
    /// <summary>
    /// Utility method for quantising sensor readings of the temperature sensor type.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>Quantised value of sensor readings from the sample.</returns>
    public static float Temperature(List<BaseSensorReading> sensorReadings)
    {
        return MaxValue(sensorReadings);
        //return MinValue(sensorReadings);
        //return AvgValue(sensorReadings);
    }

    /// <summary>
    /// Utility method for quantising sensor readings of the light sensor type.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>Quantised value of sensor readings from the sample.</returns>
    public static float Light(List<BaseSensorReading> sensorReadings)
    {
        return MaxValue(sensorReadings);
        //return MinValue(sensorReadings);
        //return AvgValue(sensorReadings);
    }

    /// <summary>
    /// Utility method for quantising sensor readings of the toggle sensor type.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <param name="previousValue">Quantised value from previous sample.</param>
    /// <returns>Quantised value of sensor readings from the sample.</returns>
    public static float Toggle(List<BaseSensorReading> sensorReadings, float previousValue)
    {
        return FirstValue(sensorReadings, previousValue);
        //return LastValue(sensorReadings, previousValue);
        //return OnActivationValue(sensorReadings);
        //return ActivationCountValue(sensorReadings);
    }

    /// <summary>
    /// Utility method for quantising sensor readings of the presence sensor type.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <param name="previousValue">Quantised value from previous sample.</param>
    /// <returns>Quantised value of sensor readings from the sample.</returns>
    public static float Presence(List<BaseSensorReading> sensorReadings, float previousValue)
    {
        return FirstValue(sensorReadings, previousValue);
        //return LastValue(sensorReadings, previousValue);
        //return OnActivationValue(sensorReadings);
        //return ActivationCountValue(sensorReadings);
    }

    /// <summary>
    /// Utility method for quantising sensor readings of the interaction sensor type.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>Quantised value of sensor readings from the sample.</returns>
    public static float Interaction(List<BaseSensorReading> sensorReadings)
    {
        return AtLeastOneSensorReadingValue(sensorReadings);
        //return SensorReadingsCountValue(sensorReadings);
    }

    /// <summary>
    /// Helper quantise method which returns the maximum sensor value amongst the specified sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>The maximum sensor value amongst the specified sensor readings.</returns>
    private static float MaxValue(List<BaseSensorReading> sensorReadings)
    {
        float tupleOutput = float.MinValue;

        foreach (BaseSensorReading aSensorReading in sensorReadings)
            if (tupleOutput < aSensorReading.GetSensorValue())
                tupleOutput = aSensorReading.GetSensorValue();

        return tupleOutput;
    }

    /// <summary>
    /// Helper quantise method which returns the minimum sensor value amongst the specified sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>The minimum sensor value amongst the specified sensor readings.</returns>
    private static float MinValue(List<BaseSensorReading> sensorReadings)
    {
        float tupleOutput = float.MaxValue;

        foreach (BaseSensorReading aSensorReading in sensorReadings)
            if (tupleOutput > aSensorReading.GetSensorValue())
                tupleOutput = aSensorReading.GetSensorValue();

        return tupleOutput;
    }

    /// <summary>
    /// Helper quantise method which returns the average sensor value amongst the specified sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>The average sensor value amongst the specified sensor readings.</returns>
    private static float AvgValue(List<BaseSensorReading> sensorReadings)
    {
        float tupleOutput = 0f;

        foreach (BaseSensorReading aSensorReading in sensorReadings)
            tupleOutput += aSensorReading.GetSensorValue();

        return tupleOutput / sensorReadings.Count;
    }

    /// <summary>
    /// Helper quantise method which returns the first value amongst the specified sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <param name="previousValue">Quantised value from previous sample.</param>
    /// <returns>First value amongst the specified sensor readings.</returns>
    private static float FirstValue(List<BaseSensorReading> sensorReadings, float previousValue)
    {
        float tupleOutput = previousValue;

        if (sensorReadings.Count > 0)
            tupleOutput = sensorReadings[0].GetSensorValue();

        return tupleOutput;
    }

    /// <summary>
    /// Helper quantise method which returns the last value amongst the specified sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <param name="previousValue">Quantised value from previous sample.</param>
    /// <returns>Last value amongst the specified sensor readings.</returns>
    private static float LastValue(List<BaseSensorReading> sensorReadings, float previousValue)
    {
        float tupleOutput = previousValue;

        if (sensorReadings.Count > 0)
            tupleOutput = sensorReadings[sensorReadings.Count - 1].GetSensorValue();

        return tupleOutput;
    }

    /// <summary>
    /// Helper quantise method which returns true (1f) if at least one activation exists amongst
    /// the specified sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>True (1f) if one activation exists amongst the specified sensor readings. False (0f) if otherwise.</returns>
    private static float OnActivationValue(List<BaseSensorReading> sensorReadings)
    {
        float tupleOutput = 0f;

        foreach (BaseSensorReading aSensorReading in sensorReadings)
            if (aSensorReading.GetSensorValue() == 1)
                return 1f;

        return tupleOutput;
    }

    /// <summary>
    /// Helper quantise method which counts the number of activations amongst the specified
    /// sensor readings.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>The number of activations amongst the specified sensor readings.</returns>
    private static float ActivationCountValue(List<BaseSensorReading> sensorReadings)
    {
        float tupleOutput = 0f;

        foreach (BaseSensorReading aSensorReading in sensorReadings)
            if (aSensorReading.GetSensorValue() == 1)
                tupleOutput += 1;

        return tupleOutput;
    }

    /// <summary>
    /// Helper quantise method which counts the number of sensor readings within this sample.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>The number of sensor readings within this sample.</returns>
    private static float SensorReadingsCountValue(List<BaseSensorReading> sensorReadings)
    {
        return sensorReadings.Count;
    }

    /// <summary>
    /// Helper quantise method which returns true (1f) if at least one sensor reading exists
    /// within this sample.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <returns>True (1f) if at least one sensor reading exists in this sample. False (0f) if otherwise.</returns>
    private static float AtLeastOneSensorReadingValue(List<BaseSensorReading> sensorReadings)
    {
        if (sensorReadings.Count > 0)
            return 1f;
        return 0f;
    }
}

