using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Observer used to handle communication between game objects and simulation
/// layer game objects without explicitly defining dependencies between
/// game objects.
/// </summary>
public class SimulationLayerEventsObserver : Singleton<SimulationLayerEventsObserver>
{
    /*
        As the class name suggests, this class facilitates the
        observer pattern for decoupling dependencies. See the following
        resources to learn more:
        - (http://gameprogrammingpatterns.com/observer.html)
        - (https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/)

        Not going to comment on each part of the code. Each of the
        regions use similar code. Regions were used to seperate
        code based on domain. For example, events for interacting
        with the voxel mesh or with the sensor stream.

        --- NOTE ---

        I originally created these events to get the simulation working.
        I found that many "custom" events had to be created to facilitate
        different events with different parameters.

        These events, however, could probably be refactored into a better or
        more suitable data structure. If you have can improve the code, please do so.
    */

    public override void Awake()
    {
        base.Awake();
        EventKeyToNoArgumentEvents = new Dictionary<string, UnityEvent>();
    }

    #region No-Argument Events

    private Dictionary<string, UnityEvent> EventKeyToNoArgumentEvents;
    public static string KEY_START_NEW_SENSOR_STREAM = "startnewsensorstream";
    public static string KEY_END_CURRENT_SENSOR_STREAM = "endcurrentsensorstream";
    public static string KEY_WRITE_SENSOR_READINGS_TO_DB = "writedatatodb";
    public static string KEY_VOXEL_MESH_RESET = "voxelmeshreset";

    public bool TriggerNoArgumentEvent(string eventKey)
    {
        UnityEvent noArgumentEvent;
        if (EventKeyToNoArgumentEvents.TryGetValue(eventKey, out noArgumentEvent))
        {
            noArgumentEvent.Invoke();
            return true;
        }
        return false;
    }

    public void AddListenerToNoArgumentEvent(string eventKey, UnityAction listener)
    {
        UnityEvent noArgumentEvent = null;
        if (EventKeyToNoArgumentEvents.TryGetValue(eventKey, out noArgumentEvent))
        {
            noArgumentEvent.AddListener(listener);
        }
        else
        {
            noArgumentEvent = new UnityEvent();
            noArgumentEvent.AddListener(listener);
            EventKeyToNoArgumentEvents.Add(eventKey, noArgumentEvent);
        }
    }

    public void RemoveListenerToNoArgumentEvent(string eventKey, UnityAction listener)
    {
        UnityEvent noArgumentEvent = null;
        if (EventKeyToNoArgumentEvents.TryGetValue(eventKey, out noArgumentEvent))
            noArgumentEvent.RemoveListener(listener);
    }

    #endregion

    #region Publish Sensor Reading Event

    private UnityAction<BaseSensorReading> PublishSensorReadingEvent;

    public bool TriggerPublishSensorEvent(BaseSensorReading aSensorReading)
    {
        if (PublishSensorReadingEvent != null)
        {
            PublishSensorReadingEvent(aSensorReading);
            return true;
        }
        return false;
    }

    public void AddListenerToPublishSensorReadingEvent(UnityAction<BaseSensorReading> listener)
    {
        PublishSensorReadingEvent += listener;
    }

    public void RemoveListenerToPublishSensorReadingEvent(UnityAction<BaseSensorReading> listener)
    {
        PublishSensorReadingEvent -= listener;
    }

    #endregion

    #region Sensor Bookmark Events

    private UnityEventString AddSensorBookmarkEvent;
    private UnityEventString AnnotateEndSensorBookmarkEvent;

    public bool TriggerAddSensorBookmarkEvent(string bookmarkName)
    {
        if (AddSensorBookmarkEvent != null)
        {
            AddSensorBookmarkEvent.Invoke(bookmarkName);
            return true;
        }

        return false;
    }

    public void AddListenerToAddSensorBookmarkEvent(UnityAction<string> listener)
    {
        if (AddSensorBookmarkEvent == null)
            AddSensorBookmarkEvent = new UnityEventString();
        AddSensorBookmarkEvent.AddListener(listener);
    }

    public void RemoveListenerToAddSensorBookmarkEvent(UnityAction<string> listener)
    {
        if (AddSensorBookmarkEvent != null)
            AddSensorBookmarkEvent.RemoveListener(listener);
    }

    public bool TriggerAnnotateEndSensorBookmarkEvent(string bookmarkName)
    {
        if (AnnotateEndSensorBookmarkEvent != null)
        {
            AnnotateEndSensorBookmarkEvent.Invoke(bookmarkName);
            return true;
        }

        return false;
    }

    public void AddListenerToAnnotateEndSensorBookmarkEvent(UnityAction<string> listener)
    {
        if (AnnotateEndSensorBookmarkEvent == null)
            AnnotateEndSensorBookmarkEvent = new UnityEventString();
        AnnotateEndSensorBookmarkEvent.AddListener(listener);
    }

    public void RemoveListenerToAnnotateEndSensorBookmarkEvent(UnityAction<string> listener)
    {
        if (AnnotateEndSensorBookmarkEvent != null)
            AnnotateEndSensorBookmarkEvent.RemoveListener(listener);
    }

    #endregion

    #region Voxel Mesh Manager Events

    private UnityEventEmitHeat EmitHeatFromProducerEvent;

    public bool TriggerEmitHeatProducerEvent(Vector3 position, int range, float value)
    {
        if (EmitHeatFromProducerEvent != null)
        {
            EmitHeatFromProducerEvent.Invoke(position, range, value);
            return true;
        }
        return false;
    }

    public void AddListenerToEmitHeatProducerEvent(UnityAction<Vector3, int, float> listener)
    {
        if (EmitHeatFromProducerEvent == null)
            EmitHeatFromProducerEvent = new UnityEventEmitHeat();
        EmitHeatFromProducerEvent.AddListener(listener);
    }

    public void RemoveListenerFromEmitHeatProducerEvent(UnityAction<Vector3, int, float> listener)
    {
        EmitHeatFromProducerEvent.RemoveListener(listener);
    }

    private UnityEventBool ToggleVoxelMeshEvent;

    public bool TriggerToggleVoxelMeshEvent(bool activateVoxelMesh)
    {
        if (ToggleVoxelMeshEvent != null)
        {
            ToggleVoxelMeshEvent.Invoke(activateVoxelMesh);
            return true;
        }
        return false;
    }

    public void AddListenerToToggleVoxelMeshEvent(UnityAction<bool> listener)
    {
        if (ToggleVoxelMeshEvent == null)
            ToggleVoxelMeshEvent = new UnityEventBool();
        ToggleVoxelMeshEvent.AddListener(listener);
    }

    public void RemoveListenerFromToggleVoxelMeshEvent(UnityAction<bool> listener)
    {
        ToggleVoxelMeshEvent.RemoveListener(listener);
    }

    #endregion
}
