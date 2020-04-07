/// <summary>
/// Child extension of the base sensor reading class. This sensor reading
/// extension explicitly manages a float value as it's sensor value.
/// </summary>
[System.Serializable]
public class FloatSensorReading : BaseSensorReading
{
    public float sensorValue;

    public override string GetSensorReadingString()
    {
        return base.GetSensorReadingString() + ";" + sensorValue;
    }

    public override float GetSensorValue()
    {
        return sensorValue;
    }
}