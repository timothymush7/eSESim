using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A GUI class/component which displays inhabitant-related information.
/// This component is communicated with using events.
/// </summary>
public class ADLStatisticsPanel : MonoBehaviour
{
    /*
        The below Text objects represent Text Areas (GUI) in the scene.
        Each of the text objects are used to illustrate specific bits of
        information.
    */

    public Text TotalDestinationCountText, CurrentDestinationCountText, MaxInterruptCountText;
    public Text CurrentInterruptCountText, InterruptionRateText;

    void Start()
    {
        if (AreAllTextsReferenced())
        {
            ResetTexts();
            SubscribeToGUIEvents();
        }
        else
        {
            Debug.LogError("At least one GUI text object is not referenced.");
        }
    }

    /// <summary>
    /// Helper method for checking whether all GUI objects are referenced.
    /// </summary>
    /// <returns>True if all GUI objects are referenced. False if otherwise.</returns>
    private bool AreAllTextsReferenced()
    {
        return ((TotalDestinationCountText) && (CurrentDestinationCountText) && (MaxInterruptCountText) &&
        (CurrentInterruptCountText) && (InterruptionRateText));
    }

    /// <summary>
    /// Helper method which resets all descriptions in the text objects.
    /// </summary>
    private void ResetTexts()
    {
        TotalDestinationCountText.text = "0";
        CurrentDestinationCountText.text = "0";
        MaxInterruptCountText.text = "0";
        CurrentInterruptCountText.text = "0";
        InterruptionRateText.text = "0.0";
    }

    /// <summary>
    /// Helper method which subscribes this class/component to GUI-related events.
    /// </summary>
    private void SubscribeToGUIEvents()
    {
        if (GUILayerEventsObserver.Instance)
        {
            // Clear text callback
            GUILayerEventsObserver.Instance.AddListenerToNoArgumentEvent(GUILayerEventsObserver.KEY_CLEAR_ADL_RUN_STATS_PANEL, ResetTexts);

            // Inhabitant behaviour statistics
            GUILayerEventsObserver.Instance.AddListenerToADLRunStatEvent(GUILayerEventsObserver.KEY_TOTAL_INTERACTION_COUNT, UpdateTotalCountText);
            GUILayerEventsObserver.Instance.AddListenerToADLRunStatEvent(GUILayerEventsObserver.KEY_CURRENT_INTERACTION_COUNT, UpdateCurrentCountText);
            GUILayerEventsObserver.Instance.AddListenerToADLRunStatEvent(GUILayerEventsObserver.KEY_MAX_INTERRUPT_COUNT, UpdateMaxInterruptCountText);
            GUILayerEventsObserver.Instance.AddListenerToADLRunStatEvent(GUILayerEventsObserver.KEY_INTERRUPT_COUNT, UpdateCurrentInterruptCountText);
            GUILayerEventsObserver.Instance.AddListenerToADLRunStatEvent(GUILayerEventsObserver.KEY_INTERRUPTION_RATE, UpdateInterruptionRateText);
        }
    }

    #region GUI Callback Methods

    public void UpdateTotalCountText(float statValue)
    {
        TotalDestinationCountText.text = statValue.ToString();
    }

    public void UpdateCurrentCountText(float statValue)
    {
        CurrentDestinationCountText.text = statValue.ToString();
    }

    public void UpdateMaxInterruptCountText(float statValue)
    {
        MaxInterruptCountText.text = statValue.ToString();
    }

    public void UpdateCurrentInterruptCountText(float statValue)
    {
        CurrentInterruptCountText.text = statValue.ToString();
    }

    public void UpdateInterruptionRateText(float statValue)
    {
        InterruptionRateText.text = statValue.ToString();
    }

    #endregion
}
