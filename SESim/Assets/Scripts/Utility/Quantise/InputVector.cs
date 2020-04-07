/// <summary>
/// Class which defines the structure for an input vector. Input vectors
/// represent a sampling of sensor reading data at a specific time period.
/// </summary>
[System.Serializable]
public class InputVector
{
    public InputVectorTuple[] InputTuples;  // array of input tuples (tuples consisting of a sensor name, sensor type, sensor value)

    /*
        Output values associated with the input vector. SESim currently supports four different activities.
        The fifth activity was defined to represent an activity which may not be recognised.
    */

    public int OutputDressing = 0, OutputCooking = 0, OutputWashDishes = 0, OutputSleeping = 0, OutputOther = 0;

    /// <summary>
    /// Helper method for setting output based on a sensor bookmark name.
    /// </summary>
    /// <param name="bookmarkName">Name of bookmark associated with the input vector.</param>
    public void SetOutputUsingBookmarkName(string bookmarkName)
    {
        if (bookmarkName == null)
            return;

        switch (bookmarkName)
        {
            case "dressing":
                OutputDressing = 1;
                break;
            case "cooking":
                OutputCooking = 1;
                break;
            case "washdishes":
                OutputWashDishes = 1;
                break;
            case "sleeping":
                OutputSleeping = 1;
                break;
            default:
                OutputOther = 1;
                break;
        }
    }

    /// <summary>
    /// Helper method for finding a specific input vector tuple.
    /// </summary>
    /// <param name="sensorName">Sensor name which is used to identify a specific input vector.</param>
    /// <returns>InputVectorTuple which matches the specified sensor name.</returns>
    public InputVectorTuple FindInputVectorTuple(string sensorName)
    {
        for (int i = 0; i < InputTuples.Length; i++)
            if (InputTuples[i].sensorName.Equals(sensorName))
                return InputTuples[i];
        return null;
    }

    /// <summary>
    /// Utility method for acquiring the column headers from the input vector tuples.
    /// </summary>
    /// <returns>A single string consisting of all column headers from the input vector tuples.</returns>
    public string GetHeadersFromInputVectorTuples()
    {
        string headerString = "";

        for (int i = 0; i < InputTuples.Length; i++)
            headerString += InputTuples[i].sensorName + FileHandler.DELIMITER_COMMA;

        headerString += "output_dressing" + FileHandler.DELIMITER_COMMA;
        headerString += "output_cooking" + FileHandler.DELIMITER_COMMA;
        headerString += "output_wash_dishes" + FileHandler.DELIMITER_COMMA;
        headerString += "output_sleeping" + FileHandler.DELIMITER_COMMA;
        headerString += "output_other";

        return headerString;
    }

    /// <summary>
    /// Utility method for acquiring all the sensor values from the input vector tuples.
    /// </summary>
    /// <returns>A single string consisting of all sensor values from the input vector tuples.</returns>
    public string GetDataFromInputVectorTuples()
    {
        string dataString = "";

        for (int i = 0; i < InputTuples.Length; i++)
            dataString += InputTuples[i].sensorValue + FileHandler.DELIMITER_COMMA;

        dataString += OutputDressing + FileHandler.DELIMITER_COMMA;
        dataString += OutputCooking + FileHandler.DELIMITER_COMMA;
        dataString += OutputWashDishes + FileHandler.DELIMITER_COMMA;
        dataString += OutputSleeping + FileHandler.DELIMITER_COMMA;
        dataString += OutputOther;

        return dataString;
    }
}

/// <summary>
/// Class which defines the object structure for containing a sampling
/// of a sensor and its value at a specific time period.
/// </summary>
[System.Serializable]
public class InputVectorTuple
{
    public string sensorName;
    public Sensor.Type sensorType;
    public float sensorValue;

    public override bool Equals(object obj)
    {
        return sensorName.Equals(((InputVectorTuple)obj).sensorName);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
