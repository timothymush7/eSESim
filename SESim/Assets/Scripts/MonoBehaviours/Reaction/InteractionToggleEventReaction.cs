/// <summary>
/// Reaction implementation for the interaction toggle event. This reaction is used to toggle
/// the state of various components, such as light controllers or heat emitters.
/// </summary>
public class InteractionToggleEventReaction : Reaction
{
    public Interactable Interactable;       // Interactable which is toggled from reaction.
    public string ToggleEventIdentifier;    // Identifier associated with the toggle event
    public bool Toggle;                     // The output state for toggling an interactable

    public override void React()
    {
        if (Interactable)
            Interactable.InvokeToggleEvent(ToggleEventIdentifier, Toggle);
    }
}
