using Panda;
using UnityEngine;

/// <summary>
/// Component which contains the PB methods for handling GUI-related callbacks.
/// These callbacks are used to interact with the GUI components via the
/// GUIEventsObserver.
/// </summary>
public class PBGUILayerMethods : MonoBehaviour
{
    [Task]
    public bool DisplayTextInMainPanel(string message)
    {
        if (GUILayerEventsObserver.Instance)
        {
            GUILayerEventsObserver.Instance.TriggerBroadcastMessageToMainPanelEvent(message);
            return true;
        }

        Debug.LogError("GUI Events Observer is not defined in the scene.");
        return false;
    }

    [Task]
    public bool ResetGUI()
    {
        if (GUILayerEventsObserver.Instance)
        {
            GUILayerEventsObserver.Instance.TriggerNoArgumentEvent(GUILayerEventsObserver.KEY_CLEAR_ADL_RUN_STATS_PANEL);
            GUILayerEventsObserver.Instance.TriggerNoArgumentEvent(GUILayerEventsObserver.KEY_CLEAR_MAIN_TEXT_PANEL);
            GUILayerEventsObserver.Instance.TriggerNoArgumentEvent(GUILayerEventsObserver.KEY_CLEAR_CURRENT_INTERACTION_PANEL);
            return true;
        }

        Debug.LogError("GUI Events Observer is not defined in the scene.");
        return true;
    }
}
