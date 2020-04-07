/// <summary>
/// Child extension of the base sensor reading class. This sensor reading
/// extension explicitly manages a boolean value as it's sensor value.
/// </summary>
[System.Serializable]
public class BoolSensorReading : BaseSensorReading
{
    public bool sensorValue;

    public override string GetSensorReadingString()
    {
        return base.GetSensorReadingString() + ";" + sensorValue;
    }

    public override float GetSensorValue()
    {
        // Have manually managed boolean values as 1 (true) or 0 (false) to keep values as floats
        if (sensorValue)
            return 1f;
        return 0f;
    }
}
