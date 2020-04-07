using UnityEngine;

/// <summary>
/// Class/component which includes the founctional logic for
/// a sensor model which monitors changes in properties.
/// </summary>
public class PropertyChangeSensor : Sensor
{
    /*
        Never used in my research, but may be useful to monitor.
        Decided to keep this code in case it is needed.
    */

    [Header("Property Change Sensor Attributes")]
    [Tooltip("String identifier for game object of property to monitor.")] public string PropertyParentName;
    [Tooltip("String identifier for property to monitor.")] public string PropertyDescription;
    private Property TargetProperty;    // reference to property which is monitored by this sensor object.

    protected void Start()
    {
        TryListenToChangesOfSpecifiedProperty();
    }

    /// <summary>
    /// Helper method which queries the properties collection for the property which matches the
    /// specified details and monitors the property.
    /// </summary>
    private void TryListenToChangesOfSpecifiedProperty()
    {
        if (PropertiesCollection.Instance)
        {
            TargetProperty = PropertiesCollection.Instance.
                TryGetPropertyUsingParentNameAndDescription(PropertyParentName,
                PropertyDescription);

            if (TargetProperty != null)
                TargetProperty.OnValueChange += GenerateBaseSensorReading;
            else
                Debug.LogError(SensorName + "': property not assigned to this sensor.");
        }
        else
            Debug.LogError("Properties Collection is not defined in the scene.");
    }

    private void GenerateAndPublishSensorReading()
    {
        /*
            Can publish sensor readings based on property.
            Currently just publishing base sensor readings (without a value)
        */

        GenerateBaseSensorReading();
    }
}
