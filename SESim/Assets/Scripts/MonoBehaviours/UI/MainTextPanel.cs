using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the central/main panel of the UI, found at the bottom of the screen.
/// The controller manages how strings are displayed on the panel, intended to display
/// current workings or status of the player.
/// </summary>
public class MainTextPanel : MonoBehaviour
{
    [Tooltip("The primary text object which displays user-specified text.")] public Text MainPanelText;
    [Tooltip("Constant which indicates the number of past messages cached.")] public int MaxNumberOfMessagesStored = 10;
    private Queue<MainTextPanelMessage> PastMessagesDisplayed;      // Queue of past messages which were cached

    public void Awake()
    {
        PastMessagesDisplayed = new Queue<MainTextPanelMessage>();
    }

    public void Start()
    {
        if (MainPanelText)
        {
            ResetText();
            TrySubscribeToBroadcastMessageEvent();
        }
        else
            Debug.LogError("Text object not referenced.");
    }

    /// <summary>
    /// Helper callback for clearing text and past messages which were cached.
    /// </summary>
    private void ResetText()
    {
        MainPanelText.text = string.Empty;
        PastMessagesDisplayed.Clear();
    }

    /// <summary>
    /// Helper method for subscribing to the appropriate GUI events.
    /// </summary>
    private void TrySubscribeToBroadcastMessageEvent()
    {
        if (GUILayerEventsObserver.Instance)
        {
            GUILayerEventsObserver.Instance.AddListenerToBroadcastMessageEvent(DisplayNewMessage);
            GUILayerEventsObserver.Instance.AddListenerToNoArgumentEvent(GUILayerEventsObserver.KEY_CLEAR_MAIN_TEXT_PANEL, ResetText);
        }
    }

    /// <summary>
    /// Helper callback for updating the text for the main text panel.
    /// </summary>
    /// <param name="message">Text to be displayed in the main text panel.</param>
    public void DisplayNewMessage(string message)
    {
        if (PastMessagesDisplayed.Count >= MaxNumberOfMessagesStored)
            PastMessagesDisplayed.Dequeue();

        PastMessagesDisplayed.Enqueue(
            new MainTextPanelMessage
            {
                Message = message
            }
        );

        MainPanelText.text = message;
    }
}
