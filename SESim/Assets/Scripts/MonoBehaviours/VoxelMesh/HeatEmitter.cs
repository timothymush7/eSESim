using UnityEngine;

/// <summary>
/// Class/component containing the functional logic for the heat emission
/// game object. This game object influences the temperature of air spaces in
/// the voxel mesh.
/// </summary>
public class HeatEmitter : MonoBehaviour
{
    [Header("Emission Attributes")]
    [Tooltip("Range of voxels which are affected by emissions from the heat emitter.")] public int EmissionRangeInVoxels;
    [Tooltip("The rate of heat emissions produced from the heat emitter.")] public float EmissionRateInSeconds;

    [Header("Interactable Attributes")]
    public Interactable InteractableForTogglingState;

    [Header("Temperature Property Attributes")]
    [Tooltip("Parent name associated with a temperature property.")] public string TempPropParentName;
    [Tooltip("Description associated with a temperature property.")] public string TempPropDescription;
    private Property TemperatureProperty;                           // Temperature property used to decide temperature of heat emissions

    private PollTimer EmitHeatTimer;                                // Timer for handling when heat emissions are processed
    public static string TOGGLE_EVENT_IDENTIFIER = "heatemitter";   // Identifier for reactions to trigger heat emitters

    private void Start()
    {
        // Only do setup if temperature property found
        if (TryFindPropertyForTemperatureUpdates(out TemperatureProperty))
        {
            SetupHeatEmitterToggling();

            EmitHeatTimer = new PollTimer(EmissionRateInSeconds);
            EmitHeatTimer.OnTimerPoll += EmitHeatToVoxelMesh;
        }
    }

    /// <summary>
    /// Helper method which queries for a specific property using a name and description.
    /// </summary>
    /// <param name="temperatureProperty">Temperature property to be referenced from query.</param>
    /// <returns>True if temperature property found. False if otherwise.</returns>
    private bool TryFindPropertyForTemperatureUpdates(out Property temperatureProperty)
    {
        // Query for the property using specified parent name and description
        temperatureProperty = PropertiesCollection.Instance.
            TryGetPropertyUsingParentNameAndDescription(TempPropParentName, TempPropDescription);

        // Error checks on the property
        if (temperatureProperty == null)
        {
            Debug.LogError(gameObject.transform.parent.name + "' - specified property cannot be found.");
            return false;
        }
        else if (!temperatureProperty.propertyType.Equals(Property.PROPERTY_TYPE_FLOAT))
        {
            Debug.LogError(gameObject.name + "' - specified property is not a float property.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method for subscribing to interactable toggle event. This enables toggling of the
    /// heat emitter on interaction with the attached interactable (via reaction).
    /// </summary>
    /// <returns>True if subscribed to interactable toggle event. False if otherwise.</returns>
    private bool SetupHeatEmitterToggling()
    {
        if (InteractableForTogglingState)
        {
            InteractableForTogglingState.AddListenerToToggleEvent(ToggleEmissions, TOGGLE_EVENT_IDENTIFIER);
            return true;
        }

        Debug.LogError("No interactable attached with heat emitter: '" + gameObject.name + "'.");
        return false;
    }

    /// <summary>
    /// Utility method for toggling state of the heat emitter.
    /// </summary>
    /// <param name="activate">Boolean which indicates whether the heat emitter is enabled.</param>
    public void ToggleEmissions(bool activate)
    {
        if (activate)
            EmitHeatTimer.Play();
        else
            EmitHeatTimer.Pause();
    }

    /// <summary>
    /// Utility method for communication heat emissions to voxel mesh.
    /// </summary>
    private void EmitHeatToVoxelMesh()
    {
        // Temperature is expected to be defined here, since event setup is only done if property found.
        if (SimulationLayerEventsObserver.Instance)
        {
            SimulationLayerEventsObserver.Instance.TriggerEmitHeatProducerEvent(transform.position,
                EmissionRangeInVoxels, TemperatureProperty.floatValue);
        }
        else
            Debug.LogError("Simulation Layer Events Observer is not defined in the scene.");
    }

    public void Update()
    {
        if (EmitHeatTimer != null)
            EmitHeatTimer.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
