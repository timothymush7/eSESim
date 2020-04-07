using UnityEngine;
using Panda;

/// <summary>
/// This PB method class/component contains several PB callbacks that are used
/// when processing activity performances. These callbacks are primarily used
/// to interact with simulation layer-related game objects.
/// </summary>
public class PBSimulationLayerMethods : MonoBehaviour
{
    /*
		DEBUG NOTE:
		
		This class/component is currently attached to inhabitant game objects.
		However, this should be moved to a central game object which manages
		inhabitants to better support multiple inhabitants.
	*/

    #region Sensor Stream Methods

    [Task]
    public bool StartNewSensorStream()
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerNoArgumentEvent(SimulationLayerEventsObserver.KEY_START_NEW_SENSOR_STREAM))
                return true;

            Debug.LogError("The event/action: " + SimulationLayerEventsObserver.KEY_START_NEW_SENSOR_STREAM + " is not defined in the LogicEventsObserver.");
            return false;
        }

        Debug.LogError("The Logic Events Observer is not defined in the scene.");
        return false;
    }

    [Task]
    public bool EndCurrentSensorStream()
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerNoArgumentEvent(SimulationLayerEventsObserver.KEY_END_CURRENT_SENSOR_STREAM))
                return true;

            Debug.LogError("The event/action: " + SimulationLayerEventsObserver.KEY_END_CURRENT_SENSOR_STREAM + " is not defined in the LogicEventsObserver.");
            return false;
        }

        Debug.LogError("The Logic Events Observer is not defined in the scene.");
        return false;
    }

    [Task]
    public bool WriteSensorReadingsToDB()
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerNoArgumentEvent(SimulationLayerEventsObserver.KEY_WRITE_SENSOR_READINGS_TO_DB))
                return true;

            Debug.LogError("The event/action: " + SimulationLayerEventsObserver.KEY_WRITE_SENSOR_READINGS_TO_DB + " is not defined in the LogicEventsObserver.");
            return false;
        }

        Debug.LogError("The Logic Events Observer is not defined in the scene.");
        return true;
    }

    #endregion

    #region Voxel Mesh Methods

    [Task]
    public bool ResetVoxelMesh()
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerNoArgumentEvent(SimulationLayerEventsObserver.KEY_VOXEL_MESH_RESET))
                return true;

            Debug.LogError("The event/action: " + SimulationLayerEventsObserver.KEY_VOXEL_MESH_RESET + " is not defined in the Logic Events Observer.");
            return false;
        }

        Debug.LogError("The Logic Events Observer is not defined in the scene.");
        return false;
    }

    [Task]
    public bool ToggleVoxelMesh(bool activateVoxelMesh)
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerToggleVoxelMeshEvent(activateVoxelMesh))
                return true;

            Debug.LogError("The event/action: ToggleVoxelMesh is not defined in the Logic Events Observer.");
            return false;
        }

        Debug.LogError("The Logic Events Observer is not defined in the scene.");
        return false;
    }

    #endregion

    #region All Home Objects Methods

    [Task]
    public bool ToggleLights(bool activateLights)
    {
        if (AllHomeObjects.Instance)
        {
            AllHomeObjects.Instance.ToggleLights(activateLights);
            return true;
        }

        Debug.LogError("Home objects manager instance is null.");
        return false;
    }

    [Task]
    public bool ToggleLight(string gameObjectName, bool activateLight)
    {
        if (AllHomeObjects.Instance)
        {
            AllHomeObjects.Instance.ToggleLight(gameObjectName, activateLight);
            return true;
        }

        Debug.LogError("Home object manager instance is null.");
        return false;
    }

    [Task]
    public bool ToggleProducers(bool activateProducers)
    {
        if (AllHomeObjects.Instance)
        {
            AllHomeObjects.Instance.ToggleAllHeatEmitters(activateProducers);
            return true;
        }

        Debug.LogError("Home objects manager instance is null.");
        return false;
    }

    [Task]
    public bool TogglePollSensors(bool activatePollSensors)
    {
        if (AllHomeObjects.Instance)
        {
            AllHomeObjects.Instance.ToggleAllPollSensors(activatePollSensors);
            return true;
        }

        Debug.LogError("Home objects manager instance is null.");
        return false;
    }

    #endregion
}
