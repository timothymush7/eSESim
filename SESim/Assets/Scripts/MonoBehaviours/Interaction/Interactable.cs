using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Class/component which describes game objects as interactables.
/// These objects can be interacted with by inhabitant game objects.
/// Interactable game objects process reactions on interaction.
/// </summary>
public class Interactable : MonoBehaviour
{
    [Tooltip("Transform describing location where inhabitant can interact with the interactable.")] public Transform InteractionLocation;
    [Tooltip("Reactions which are processed on interaction with the interactable.")] public ReactionCollection Reactions;
    public UnityAction OnInteractEvent;                                     // Events used by interaction sensors
    public UnityAction<float> OnStartInteractEvent, OnStopInteractEvent;    // Events used by toggle sensors

    /*
        Specifically kept these events separate from the OnInteract-related events.
        The below events are specifically triggered from reactions. This implementation
        allows inhabitants to interact with interactables for different purposes.

        For example, could interact with a stove for the purpose of switching on or
        switching off the stove.
    */

    private Dictionary<string, UnityEventBool> IdentifierToReactionEvents = new Dictionary<string, UnityEventBool>();   // Dictionary of reaction events

    /// <summary>
    /// Primary method of interactable. This method performs the operations to
    /// model interaction. This method specifically triggers the various appropriate
    /// events and reactions.
    /// </summary>
    /// <param name="reactionDescription">Description of intended reaction collection to process.</param>
    public void InteractWithReaction(string reactionDescription)
    {
        // Trigger sensor-related events
        if (OnInteractEvent != null)
            OnInteractEvent();
        if (OnStartInteractEvent != null)
            OnStartInteractEvent(1f);

        // React specific if the description is not empty
        if (Reactions)
            if (!reactionDescription.Equals(""))
                Reactions.ReactSpecific(reactionDescription);
    }

    /// <summary>
    /// Helper method which triggers the stop interact event for toggle sensors.
    /// </summary>
    public void StopInteract()
    {
        if (OnStopInteractEvent != null)
            OnStopInteractEvent(0f);
    }

    #region Boolean Events Management Code

    public void AddListenerToToggleEvent(UnityAction<bool> listener, string identifier)
    {
        if (!IdentifierToReactionEvents.ContainsKey(identifier))
            IdentifierToReactionEvents.Add(identifier, new UnityEventBool());
        IdentifierToReactionEvents[identifier].AddListener(listener);
    }

    public void RemoveListenerFromToggleEvent(UnityAction<bool> listener, string identifier)
    {
        if (IdentifierToReactionEvents.ContainsKey(identifier))
            IdentifierToReactionEvents[identifier].RemoveListener(listener);
    }

    public void InvokeToggleEvent(string identifier, bool toggle)
    {
        if (IdentifierToReactionEvents.ContainsKey(identifier))
            IdentifierToReactionEvents[identifier].Invoke(toggle);
    }

    #endregion
}
