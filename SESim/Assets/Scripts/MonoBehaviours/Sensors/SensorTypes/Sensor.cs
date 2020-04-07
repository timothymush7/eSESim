using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Parent abstract class. Class describes the common underlying structure for sensors.
/// </summary>
public abstract class Sensor : MonoBehaviour
{
    [Header("Base Sensor Attributes")]
    [Tooltip("Unique identifier for sensor.")] public string SensorName;
    [Tooltip("Describes type of sensor mechanism.")] [SerializeField] protected Type SensorType;
    public UnityAction<BaseSensorReading> GenerateSensorReadingEvent;   // event where sensor generates a sensor reading

    public enum Type
    {
        Temperature,
        Presence,
        PropertyChange,
        Light,
        Interaction,
        Toggle
    }

    /// <summary>
    /// Helper method for assisting conversions between text and the sensor type enum.
    /// </summary>
    /// <param name="sensorTypeString">Sensor type in string format.</param>
    /// <returns>Sensor type enum from specified string.</returns>
    public static Type StringToSensorType(string sensorTypeString)
    {
        switch (sensorTypeString)
        {
            case "Temperature":
                return Type.Temperature;
            case "Motion":
                return Type.Presence;
            case "PropertyChange":
                return Type.PropertyChange;
            case "Light":
                return Type.Light;
            case "Pressure":
                return Type.Toggle;
            case "Interaction":
            default:
                return Type.Interaction;
        }
    }

    /// <summary>
    /// Helper method for acquiring sensor type from a sensor object.
    /// </summary>
    /// <returns>Type enum indicating sensor type.</returns>
    public Type GetSensorType()
    {
        return SensorType;
    }

    /// <summary>
    /// Parent utility method for generating a sensor reading and notifying
    /// listeners of the generation of the sensor reading.
    /// </summary>
    protected virtual void GenerateBaseSensorReading()
    {
        BaseSensorReading newBaseSensorReading = new BaseSensorReading();
        UpdateDetailsInSensorReading(newBaseSensorReading);
        PublishSensorReading(newBaseSensorReading);
    }

    /// <summary>
    /// Parent utility method for generating a sensor reading of float
    /// type and notifying listeners of the generation for this new
    /// sensor reading.
    /// </summary>
    /// <param name="sensorValue"></param>
    protected virtual void GenerateFloatSensorReading(float sensorValue)
    {
        FloatSensorReading newFloatSensorReading = new FloatSensorReading
        {
            sensorValue = sensorValue
        };
        UpdateDetailsInSensorReading(newFloatSensorReading);
        PublishSensorReading(newFloatSensorReading);
    }

    /// <summary>
    /// Helper method for updating the name, type, date, time, and area details
    /// for sensor readings.
    /// </summary>
    /// <param name="newSensorReading">Sensor reading for which details are to be updated.</param>
    protected void UpdateDetailsInSensorReading(BaseSensorReading newSensorReading)
    {
        if (newSensorReading != null)
        {
            newSensorReading.sensorName = SensorName;
            newSensorReading.sensorType = SensorType;

            newSensorReading.dateTime = System.DateTime.MaxValue;

            if (TimeController.Instance)
                newSensorReading.dateTime = TimeController.Instance.GetCurrentTime(newSensorReading.dateTime);
            else
                Debug.LogError("Time controller instance does not exist in the scene.");
            if (DateController.Instance)
                newSensorReading.dateTime = DateController.Instance.GetCurrentDate(newSensorReading.dateTime);
            else
                Debug.LogError("Date controller instance does not exist in the scene.");

            /*
             * Assumes that the sensor is:
             * 1.) Attached to a home object
             * 2.) The home object is categorised under a parent game object indicating the environment area
             * 
             * Future implementations should utilise editor tools to create these objects at runtime, allowing
             * code to do the heavy lifting with categorisation and referencing of various attributes.
             */

            newSensorReading.areaName = transform.parent.parent.name;
        }
    }

    /// <summary>
    /// Helper method which notifies listeners of newly generated sensor readings. 
    /// </summary>
    /// <param name="sensorReading">Sensor reading which is sent to listeners via event.</param>
    protected void PublishSensorReading(BaseSensorReading sensorReading)
    {
        if (GenerateSensorReadingEvent != null)
            GenerateSensorReadingEvent(sensorReading);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
