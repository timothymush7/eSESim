using UnityEngine;

/// <summary>
/// Class/component which contains the functional logic for the
/// toggle sensor model. This sensor model monitors the start and
/// end of interactions with a game object.
/// </summary>
public class ToggleSensor : Sensor
{
    [Header("Toggle Sensor Attributes")]
    [Tooltip("Game object with interactable component which is monitored.")] public Interactable InteractableTriggerObject;

    protected void Start()
    {
        TryListenToInteractableInteractionEvents();
    }

    private void TryListenToInteractableInteractionEvents()
    {
        if (InteractableTriggerObject)
        {
            InteractableTriggerObject.OnStartInteractEvent +=
                GenerateFloatSensorReading;
            InteractableTriggerObject.OnStopInteractEvent +=
                GenerateFloatSensorReading;
        }
        else
            Debug.LogError(SensorName + " game object has no interactable component.");
    }
}
