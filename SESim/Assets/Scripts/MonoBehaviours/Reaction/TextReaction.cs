/// <summary>
/// Implementation of a normal reaction. Text is displayed as a
/// result of the reaction.
/// </summary>
public class TextReaction : Reaction
{
    public string Message;      //  Text for the message to be displayed

    public override void React()
    {
        if (GUILayerEventsObserver.Instance)
            GUILayerEventsObserver.Instance.TriggerBroadcastMessageToMainPanelEvent(Message);
    }
}