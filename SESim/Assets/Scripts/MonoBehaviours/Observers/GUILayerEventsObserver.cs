using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Observer used to handle communication between game objects and GUI
/// layer game objects without explicitly defining dependencies between
/// game objects.
/// </summary>
public class GUILayerEventsObserver : Singleton<GUILayerEventsObserver>
{

    /*
     * Events are used for notification purposes. Assists with
     * encapsulation and also with decoupling explicit dependencies 
     * between objects.
     * 
     * This observer was primarily created to decouple the player 
     * from UI panels when publishing updates in player statistics.
     * 
     * If multiple types of this action/event is required, then a list of
     * these events could be created.
     */

    public override void Awake()
    {
        base.Awake();
        EventKeyToADLRunStatEvents = new Dictionary<string, UnityEventFloat>();
        EventKeyToNoArgumentEvents = new Dictionary<string, UnityEvent>();
    }

    #region OnSelectNewInteractionEvent

    private UnityAction<InteractionData, float, bool> OnSelectNewInteractionEvent;

    public bool TriggerInteractionSelectionEvent(InteractionData newInteraction,
        float newInteractionDuration, bool isInterruption)
    {
        if (OnSelectNewInteractionEvent != null)
        {
            OnSelectNewInteractionEvent(newInteraction, newInteractionDuration, isInterruption);
            return true;
        }
        return false;
    }

    public void AddListenerToInteractionSelectionEvent(
        UnityAction<InteractionData, float, bool> listener)
    {
        OnSelectNewInteractionEvent += listener;
    }

    public void RemoveListenerFromInteractionSelectionEvent(
        UnityAction<InteractionData, float, bool> listener)
    {
        OnSelectNewInteractionEvent -= listener;
    }

    #endregion

    #region BroadcastMessageToMainPanel

    private UnityAction<string> BroadcastMessageToMainPanelEvent;

    public bool TriggerBroadcastMessageToMainPanelEvent(string message)
    {
        if (BroadcastMessageToMainPanelEvent != null)
        {
            BroadcastMessageToMainPanelEvent(message);
            return true;
        }
        return false;
    }

    public void AddListenerToBroadcastMessageEvent(UnityAction<string> listener)
    {
        BroadcastMessageToMainPanelEvent += listener;
    }

    public void RemoveListenerToBroadcastMessageEvent(UnityAction<string> listener)
    {
        BroadcastMessageToMainPanelEvent -= listener;
    }

    #endregion

    #region 1-Float Argument Events

    /*
     * Mainly used for ADL Run Stats Panel Updates.
     * 
     * At least these events could be generalised into a collection.
     * 
     * Here I used float unity actions (even though some only require integers)
     * as I could use float numbers to describe counters and specific calculations.
     * 
     */

    public static string KEY_TOTAL_INTERACTION_COUNT = "totalcount";
    public static string KEY_CURRENT_INTERACTION_COUNT = "currentcount";

    public static string KEY_INTERRUPT_COUNT = "interruptcount";
    public static string KEY_MAX_INTERRUPT_COUNT = "maxinterruptcount";
    public static string KEY_INTERRUPTION_RATE = "interruptionrate";

    private Dictionary<string, UnityEventFloat> EventKeyToADLRunStatEvents;

    public bool TriggerADLRunStatEvent(string eventKey, float statValue)
    {
        UnityEventFloat anADLRunStatEvent;
        if (EventKeyToADLRunStatEvents.TryGetValue(eventKey, out anADLRunStatEvent))
        {
            anADLRunStatEvent.Invoke(statValue);
            return true;
        }
        return false;
    }

    public void AddListenerToADLRunStatEvent(string eventKey, UnityAction<float> listener)
    {
        UnityEventFloat anADLRunStatEvent;
        if (EventKeyToADLRunStatEvents.TryGetValue(eventKey, out anADLRunStatEvent))
        {
            anADLRunStatEvent.AddListener(listener);
        }
        else
        {
            anADLRunStatEvent = new UnityEventFloat();
            anADLRunStatEvent.AddListener(listener);
            EventKeyToADLRunStatEvents.Add(eventKey, anADLRunStatEvent);
        }
    }

    public void RemoveListenerFromADLRunStatEvent(string eventKey, UnityAction<float> listener)
    {
        UnityEventFloat anADLRunStatEvent;
        if (EventKeyToADLRunStatEvents.TryGetValue(eventKey, out anADLRunStatEvent))
            anADLRunStatEvent.RemoveListener(listener);
    }

    #endregion

    #region No-Argument Events

    public static string KEY_CLEAR_CURRENT_INTERACTION_PANEL = "clearcurrentinteractionpanel";
    public static string KEY_CLEAR_ADL_RUN_STATS_PANEL = "clearadlrunstatspanel";
    public static string KEY_CLEAR_MAIN_TEXT_PANEL = "clearmaintextpanel";

    private Dictionary<string, UnityEvent> EventKeyToNoArgumentEvents;

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
}
