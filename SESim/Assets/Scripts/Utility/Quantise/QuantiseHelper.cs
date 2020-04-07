using System.Collections.Generic;

/// <summary>
/// Class which provides various helper methods for the sampling and
/// quantisation process.
/// </summary>
public static class QuantiseHelper
{
    /// <summary>
    /// Utility method for sampling the sensor readings according to a specified time samples.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be sampled</param>
    /// <param name="startTime">The initial/start time which serves as base for calculating sample periods.</param>
    /// <param name="samplingHours">The hours component of the sample period duration.</param>
    /// <param name="samplingMinutes">The minutes component of the sample period duration.</param>
    /// <param name="samplingSeconds">The seconds component of the sample period duration.</param>
    /// <returns></returns>
    public static List<BaseSensorReading>[] SampleSensorReadings(List<BaseSensorReading> sensorReadings, System.DateTime startTime,
        int samplingHours, int samplingMinutes, int samplingSeconds)
    {
        List<List<BaseSensorReading>> sensorReadingsSamples = new List<List<BaseSensorReading>>();

        // Calculate next end of sampling period
        System.DateTime nextSamplePeriod = startTime;
        nextSamplePeriod = nextSamplePeriod.Date + new System.TimeSpan(samplingHours + nextSamplePeriod.Hour,
            samplingMinutes + nextSamplePeriod.Minute, samplingSeconds + nextSamplePeriod.Second);

        // Do nothing if quantised period was not set
        if (!nextSamplePeriod.Equals(startTime))
        {
            List<BaseSensorReading> sensorReadingSample = new List<BaseSensorReading>();

            foreach (BaseSensorReading aSensorReading in sensorReadings)
            {
                // If within sampling period, add it
                if (aSensorReading.dateTime <= nextSamplePeriod)
                {
                    sensorReadingSample.Add(aSensorReading);
                }
                else
                {
                    // Add current sample if there are sensor readings in it
                    if (sensorReadingSample.Count != 0)
                        sensorReadingsSamples.Add(sensorReadingSample);

                    // Clear sample to represent next sample period + add current sensor reading
                    sensorReadingSample.Clear();
                    sensorReadingSample.Add(aSensorReading);

                    // Calculate next end of sampling period
                    nextSamplePeriod = nextSamplePeriod.Date + new System.TimeSpan(samplingHours + nextSamplePeriod.Hour,
                        samplingMinutes + nextSamplePeriod.Minute, samplingSeconds + nextSamplePeriod.Second);
                }
            }

            // If current sample not empty + reached end of sensor readings -> add as last sample
            if (sensorReadingSample.Count != 0)
                sensorReadingsSamples.Add(sensorReadingSample);
        }

        return sensorReadingsSamples.ToArray();
    }

    /// <summary>
    /// Utility method for quantising a samples of sensor readings into input vectors.
    /// </summary>
    /// <param name="sensorReadingsSamples">A list of sensor reading samples to be quantised.</param>
    /// <param name="sensorObjects">Array of sensor objects associated with the sensor readings.</param>
    /// <returns>An array of input vectors which were created from quantising the provided samples of sensor readings.</returns>
    public static InputVector[] QuantiseSensorReadingsSamples(List<BaseSensorReading>[] sensorReadingsSamples, Sensor[] sensorObjects)
    {
        InputVector[] inputVectors = new InputVector[sensorReadingsSamples.Length];

        for (int i = 0; i < sensorReadingsSamples.Length; i++)
        {
            InputVector anInputVector;

            /*
                This part probably could be coded better. The previous input vector may influence the value
                of the next input vector. For example, presence sensor values during quantisation.
            */

            if (i > 0)
                anInputVector = QuantiseSensorReadingsSample(sensorReadingsSamples[i], sensorObjects, inputVectors[i - 1]);
            else
                anInputVector = QuantiseSensorReadingsSample(sensorReadingsSamples[i], sensorObjects, null);

            inputVectors[i] = anInputVector;
        }

        return inputVectors;
    }

    /// <summary>
    /// Utility method for quantising a single sample of sensor readings into a input vector.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings to be quantised.</param>
    /// <param name="sensorObjects">Array of sensor objects associated with the sensor readings.</param>
    /// <param name="previousInputVector">Input vector from the previous sample period.</param>
    /// <returns></returns>
    private static InputVector QuantiseSensorReadingsSample(List<BaseSensorReading> sensorReadings, Sensor[] sensorObjects, InputVector previousInputVector)
    {
        // Create and setup input vector, which will contain all the quantised data tuples
        InputVector anInputVector = CreateInputVector(sensorObjects);

        // Setup data structure for sensor name to sensor reading list lookups - also byproducts: input vector output and sensor names found in sensor readings
        List<string> sensorNamesFoundInSensorReadings;
        var sensorNameToSensorReadings = CreateSensorReadingsDictionary(sensorReadings, out sensorNamesFoundInSensorReadings, ref anInputVector);

        // Reference quantised values for each sensor name
        foreach (string sensorName in sensorNamesFoundInSensorReadings)
        {
            List<BaseSensorReading> sensorReadingsOfSensorName = sensorNameToSensorReadings[sensorName];

            // Reference the previous tuple value
            float previousTupleValue = 0f;
            if (previousInputVector != null)
            {
                InputVectorTuple previousInputVectorTuple = previousInputVector.FindInputVectorTuple(sensorName);
                if (previousInputVectorTuple != null)
                    previousTupleValue = previousInputVectorTuple.sensorValue;
            }

            // Construct new tuple with quantised value
            InputVectorTuple newInputVectorTuple = anInputVector.FindInputVectorTuple(sensorName);
            newInputVectorTuple.sensorValue = QuantiseSensorReadings(sensorReadingsOfSensorName, newInputVectorTuple.sensorType, previousTupleValue);
        }

        return anInputVector;
    }

    /// <summary>
    /// Helper method creating an input vector which represents a sample of sensor readings.
    /// </summary>
    /// <param name="sensorObjects">Array of sensor objects associated with sensor readings.</param>
    /// <returns></returns>
    private static InputVector CreateInputVector(Sensor[] sensorObjects)
    {
        InputVector anInputVector = new InputVector()
        {
            InputTuples = new InputVectorTuple[sensorObjects.Length]
        };

        for (int i = 0; i < sensorObjects.Length; i++)
        {
            anInputVector.InputTuples[i] = new InputVectorTuple()
            {
                sensorName = sensorObjects[i].SensorName,
                sensorType = sensorObjects[i].GetSensorType(),
                sensorValue = 0f
            };
        }

        return anInputVector;
    }

    /// <summary>
    /// Helper method for creating a dictionary for sensor readings. The dictionary is used to separate
    /// the sensor readings with the respective sensors.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings used for creating the dictionary.</param>
    /// <param name="sensorNamesList">Referenced list of sensor names which are identified from the sensor readings.</param>
    /// <param name="anInputVector">Referenced input vector whose output is updated from the sensor readings.</param>
    /// <returns>Dictionary mapping of sensor names to sensor readings.</returns>
    private static Dictionary<string, List<BaseSensorReading>> CreateSensorReadingsDictionary(List<BaseSensorReading> sensorReadings,
       out List<string> sensorNamesList, ref InputVector anInputVector)
    {
        var sensorNameToSensorReadings = new Dictionary<string, List<BaseSensorReading>>();
        sensorNamesList = new List<string>();

        foreach (BaseSensorReading aSensorReading in sensorReadings)
        {
            if (!sensorNameToSensorReadings.ContainsKey(aSensorReading.sensorName))
            {
                sensorNameToSensorReadings.Add(aSensorReading.sensorName, new List<BaseSensorReading>());
                sensorNamesList.Add(aSensorReading.sensorName);
            }
            sensorNameToSensorReadings[aSensorReading.sensorName].Add(aSensorReading);

            anInputVector.SetOutputUsingBookmarkName(aSensorReading.sensorBookmarkName);
        }

        return sensorNameToSensorReadings;
    }

    /// <summary>
    /// Helper method for quantising a sensor readings, within a sample, from a specific sensor.
    /// </summary>
    /// <param name="sensorReadings">Sensor readings, from a specific sensor, to be quantised.</param>
    /// <param name="sensorType">Sensor type associated with the specified sensor readings.</param>
    /// <param name="previousInputVectorValue">Float which describes the previous input vector value of the specified sensor.</param>
    /// <returns></returns>
    private static float QuantiseSensorReadings(List<BaseSensorReading> sensorReadings, Sensor.Type sensorType, float previousInputVectorValue)
    {
        float sensorValue = 0f;

        switch (sensorType)
        {
            case Sensor.Type.Temperature:
                sensorValue = QuantiseMethods.Temperature(sensorReadings);
                break;
            case Sensor.Type.Light:
                sensorValue = QuantiseMethods.Light(sensorReadings);
                break;
            case Sensor.Type.Presence:
                sensorValue = QuantiseMethods.Presence(sensorReadings, previousInputVectorValue);
                break;
            case Sensor.Type.Interaction:
                sensorValue = QuantiseMethods.Interaction(sensorReadings);
                break;
            case Sensor.Type.Toggle:
                sensorValue = QuantiseMethods.Toggle(sensorReadings, previousInputVectorValue);
                break;
            default:
                break;
        }

        return sensorValue;
    }
}

