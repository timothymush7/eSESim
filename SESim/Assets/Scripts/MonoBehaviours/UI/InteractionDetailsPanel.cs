using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This GUI class/component includes text objects which are updated with
/// inhabitant destination information.
/// </summary>
public class InteractionDetailsPanel : MonoBehaviour
{
    /*
        Text objects for displaying destination-related information.
    */

    public Text NameText, DescriptionText, ReactionCollectionText;
    public Text InitialDurationText, CurrentDurationText, IsInterruptionText;

    void Start()
    {
        if (CheckIfAllTextsAreReferenced())
        {
            ResetTexts();
            SubscribeToGUIEvents();
        }
        else
            Debug.LogError("One of the text objects are not referenced.");
    }

    private bool CheckIfAllTextsAreReferenced()
    {
        return ((NameText) && (DescriptionText) && (ReactionCollectionText)
                && (InitialDurationText) && (CurrentDurationText) && (IsInterruptionText));
    }

    /// <summary>
    /// Helper method for subscribing to the appropriate GUI events.
    /// </summary>
    private void SubscribeToGUIEvents()
    {
        if (GUILayerEventsObserver.Instance)
        {
            GUILayerEventsObserver.Instance.AddListenerToInteractionSelectionEvent(UpdateText);
            GUILayerEventsObserver.Instance.AddListenerToNoArgumentEvent(GUILayerEventsObserver.KEY_CLEAR_CURRENT_INTERACTION_PANEL, ResetTexts);
        }
    }

    /// <summary>
    /// GUI callback for updating the text in the text objects.
    /// </summary>
    /// <param name="newDestination">Next destination for the inhabitant.</param>
    /// <param name="newInteractionDuration">Duration for the next interaction.</param>
    /// <param name="isInterruption">Boolean which indicates whether the next interaction is a interruption.</param>
    public void UpdateText(InteractionData newDestination, float newInteractionDuration, bool isInterruption)
    {
        NameText.text = newDestination.TargetGameObjectName;
        DescriptionText.text = newDestination.Description;
        ReactionCollectionText.text = newDestination.ReactionCollectionDescription;
        InitialDurationText.text = newDestination.InitialInteractionDuration + "";
        CurrentDurationText.text = newInteractionDuration + "";
        IsInterruptionText.text = isInterruption + "";
    }

    /// <summary>
    /// Helper callback for resetting the text in the text objects.
    /// </summary>
    private void ResetTexts()
    {
        NameText.text = string.Empty;
        DescriptionText.text = string.Empty;
        ReactionCollectionText.text = string.Empty;
        InitialDurationText.text = string.Empty;
        CurrentDurationText.text = string.Empty;
        IsInterruptionText.text = string.Empty;
    }
}
