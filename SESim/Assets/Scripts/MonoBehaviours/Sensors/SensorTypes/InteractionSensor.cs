using UnityEngine;

/// <summary>
/// Class/component which contains functional logic for the
/// interaction sensor model. This sensor monitors interactions
/// from a game object and publishes sensor readings based on
/// interactions.
/// </summary>
public class InteractionSensor : Sensor
{
    [Header("Interaction Sensor Attributes")]
    [Tooltip("Game object with interactable component which is monitored.")] public Interactable InteractableTriggerObject;

    protected void Start()
    {
        TryListenToInteractableInteractionEvents();
    }

    private void TryListenToInteractableInteractionEvents()
    {
        if (InteractableTriggerObject)
            InteractableTriggerObject.OnInteractEvent += GenerateBaseSensorReading;
        else
            Debug.LogError(SensorName + " game object has no interactable component.");
    }
}
