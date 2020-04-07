using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// This class/component contains functional logic for the
/// presence sensor model. This sensor generates sensor readings
/// when an inhabitant 
/// </summary>
public class PresenceSensor : Sensor
{
    [Header("Presence Sensor Attributes")]
    [Tooltip("Boolean which indicates whether inhabitants are detected.")] [ReadOnly] public bool CurrentSensorValue = false;
    public UnityAction FirstPresenceDetectedEvent, NoPresenceDetectedEvent;     // Presence-related events
    private List<Collider> DetectedColliders = new List<Collider>();            // List of collider objects which are currently within the monitoring radius of this sensor

    /// <summary>
    /// Unity callback from the collider components. This callback is processed
    /// when a colliding gameobject comes into contact with the sensor game object
    /// and the colliding game object is defined as a trigger.
    /// </summary>
    /// <param name="other">Collider object from colliding game object.</param>
    public void OnTriggerEnter(Collider other)
    {
        // Only monitor collisions from inhabitants
        if (other.tag.Equals("Inhabitant"))
        {
            // Fresh activation?
            if (!CurrentSensorValue)
            {
                CurrentSensorValue = true;

                // Notify listeners of activation
                if (FirstPresenceDetectedEvent != null)
                    FirstPresenceDetectedEvent();

                // Publish sensor reading on first detection? (also convert bool value to float)
                GenerateFloatSensorReading(System.Convert.ToSingle(CurrentSensorValue));
            }

            // Cache inhabitant collision
            if (!DetectedColliders.Contains(other))
                DetectedColliders.Add(other);
        }
    }

    /// <summary>
    /// Unity callback from the collider components, which is processed when any existing
    /// colliding game objects leave the "collision radius" of the sensor game object.
    /// </summary>
    /// <param name="other">Collider object from colliding game object.</param>
    public void OnTriggerExit(Collider other)
    {
        DetectedColliders.Remove(other);

        // Only change back to sensor state back to false if no other colliders
        if (DetectedColliders.Count == 0)
        {
            CurrentSensorValue = false;

            // Notify listeners of this event
            if (NoPresenceDetectedEvent != null)
                NoPresenceDetectedEvent();

            // Publish sensor reading on first reset?
            GenerateFloatSensorReading(System.Convert.ToSingle(CurrentSensorValue));
        }
    }
}
