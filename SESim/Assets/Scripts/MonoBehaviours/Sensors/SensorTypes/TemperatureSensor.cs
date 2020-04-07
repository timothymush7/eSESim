using UnityEngine;

/// <summary>
/// Class/component which contains the functional logic for the
/// temperature sensor model.
/// </summary>
public class TemperatureSensor : PollSensor
{
    [Header("Temperature Sensor Attributes")]
    [Tooltip("Numerical value describing the current temperature of nearest air space.")] public float SensorValue = 0f;
    private Voxel ClosestVoxelToSensor;     // Closest air space to sensor, which is monitored for temperature changes

    protected override void Start()
    {
        ReferenceClosestVoxelToSensor();
        base.Start();
    }

    private void ReferenceClosestVoxelToSensor()
    {
        if (VoxelMesh.Instance)
            VoxelMesh.Instance.TryFindVoxelAtSpecifiedPosition(transform.position, out ClosestVoxelToSensor);
        else
            Debug.LogError("'" + SensorName + "': No voxel manager game object defined.");
    }

    protected override void Poll()
    {
        UpdateSensorValue();
        GenerateFloatSensorReading(SensorValue);
    }

    /// <summary>
    /// Helper method which acquires the current temperature of the referenced
    /// voxel (nearest air space to this sensor) and updates the monitored
    /// temperature value of this sensor to the newly acquired tempearture.
    /// </summary>
    private void UpdateSensorValue()
    {
        if (ClosestVoxelToSensor != null)
            SensorValue = ClosestVoxelToSensor.GetTemperature();
    }
}
