using UnityEngine;
using Panda;

/// <summary>
/// This class is designed to facilitate communication between BT scripts
/// and the inhabitant controller. BT scripts use this class to manage the
/// behaviours of inhabitants.
/// </summary>
[RequireComponent(typeof(InhabitantController))]
public class PBInhabitantHandler : MonoBehaviour
{
    [Header("Inhabitant Activity Parameters")]
    [Tooltip("Probability of interruptions occuring during activity performances")] [Range(0f, 1f)] public float CurrentInterruptionRate = 0f;
    [Tooltip("Maximum threshold for adjusting interaction durations")] [Range(0, 20)] public float MaxInteractionDurationVariance = 0f;
    [Tooltip("Maximum threshold for interruptions occuring during activity performances")] [Range(0, 10)] public int MaxNumberOfInterruptions = 0;

    // DEBUG: Later Unity versions have a IntVector, which could be used to store data for the following time parameters.
    [Tooltip("Maximum threshold for adjusting the time (hour) of activity performances")] [Range(0, 23)] public int MaxTimeHoursVariance = 0;
    [Tooltip("Maximum threshold for adjusting the time (minutes) of activity performances")] [Range(0, 59)] public int MaxTimeMinutesVariance = 0;
    [Tooltip("Maximum threshold for adjusting the time (seconds) of activity performances")] [Range(0, 59)] public int MaxTimeSecondsVariance = 0;

    private InhabitantController InhabitantControllerComponent;
    private int CurrentInterruptionCount = 0;
    private int CurrentInteractionCount = 0;
    private InteractionData[] InteractionSequence;
    private InteractionData[] InterruptionSequence;

    void Start()
    {
        // Should never be null due to restriction
        InhabitantControllerComponent = GetComponent<InhabitantController>();
    }

    /// <summary>
    /// This callback is used to select an interaction sequence to be performed by the inhabitant.
    /// This callback can also be used to select an interaction sequence for interruptions.
    /// 
    /// </summary>
    /// <param name="listName">List name used for selecting an interaction sequence</param>
    /// <param name="forInterruptions">Boolean to indicate if the selected interaction sequence is used for interruptions</param>
    /// <returns>True if interaction sequence found. False if otherwise.</returns>
    [Task]
    public bool SelectRandomInteractionSequence(string listName, bool forInterruptions)
    {
        if (forInterruptions)
            return TrySelectSequenceFromInteractionDataCollection(listName, out InterruptionSequence);
        return TrySelectSequenceFromInteractionDataCollection(listName, out InteractionSequence);
    }

    /// <summary>
    /// This method is used to return an interaction sequence from the Interaction Data Collection
    /// using a provided list name.
    /// </summary>
    /// <param name="listName">List name for retrieving a interaction sequence</param>
    /// <param name="sequence">Array which is assigned with the selected interaction sequence</param>
    /// <returns>True if a sequence was assigned/found. False if otherwise.</returns>
    private bool TrySelectSequenceFromInteractionDataCollection(string listName, out InteractionData[] sequence)
    {
        sequence = null;

        // Try acquire interaction sequence from IDC using the list name, and update GUI
        if (InteractionDataCollection.Instance)
        {
            if (InteractionDataCollection.Instance.TrySelectRandomSequenceFromList(listName, out sequence))
            {
                if (GUILayerEventsObserver.Instance)
                    GUILayerEventsObserver.Instance.TriggerADLRunStatEvent(GUILayerEventsObserver.KEY_TOTAL_INTERACTION_COUNT, sequence.Length);
                else
                    Debug.LogError("GUIEventsObserver is not defined in the scene.");

                return true;
            }

            Debug.LogError("No interaction sequence found in Interaction Data Collection instance using list name: '" + listName + "'");
            return false;
        }

        Debug.LogError("Interaction Data Collection is not defined in the scene.");
        return false;
    }

    /// <summary>
    /// Main BT script looping callback which handles the processing of
    /// interactions in the specified list. Note: this method will constantly be
    /// called during ticks until the task has succeeded or failed.
    /// </summary>
    [Task]
    public void ProcessInteractions()
    {
        // Only bother processing if the inhabitant can actually receive movement actions
        if (InhabitantControllerComponent.CanReceiveMovementActions)
        {
            /*
                In the context of this callback, the following if statement checks
                if all interactions, from initiating an interaction sequence, were
                performed - which is the success condition for actions in the activity script.
            */

            // If no more interactions from sequence to perform - we are done (succeed)
            if (!AreThereInteractionsToPerform())
                Task.current.Succeed();

            /*
                The only way this callback fails is if the interaction was unable to be
                setup for the inhabitant.
            */

            // Otherwise setup next interaction to perform.
            else if (!SetupNextInteractionToPerform())
                Task.current.Fail();
        }
    }

    /// <summary>
    /// Helper method which checks if there are remaining interactions in the sequence to perform.
    /// </summary>
    /// <returns>True if there are remaining interactions to perform. False otherwise.</returns>
    private bool AreThereInteractionsToPerform()
    {
        if (InteractionSequence != null)
            if (CurrentInteractionCount < InteractionSequence.Length)
                return true;
        return false;
    }

    /// <summary>
    /// Helper method which acquires and setups the next interaction for the inhabitant to perform.
    /// </summary>
    /// <returns>True if an interaction was correctly setup. False otherwise.</returns>
    private bool SetupNextInteractionToPerform()
    {
        // Acquire the appropriate interaction data from the sequence
        bool isInterruption;
        InteractionData nextInteractionData = TryGetNextInteraction(out isInterruption);
        GameObject interactionGameObject;

        // ... then acquire the appropriate game object (of the interaction data)
        if (TryGetGameObjectUsingInteractionData(out interactionGameObject, nextInteractionData))
        {
            // ... and then do the setup in the inhabitant controller component
            float nextInteractionDuration =
                CalculateInteractionDurationVariance(nextInteractionData.InitialInteractionDuration);
            InhabitantControllerComponent.SetupInteractionAsNextDestination(interactionGameObject,
                nextInteractionData.ReactionCollectionDescription, nextInteractionDuration);

            UpdateGUI(nextInteractionData, nextInteractionDuration, isInterruption);
            return true;
        }

        // Return false if interaction data or interaction game object was not found for any reason.
        return false;
    }

    /// <summary>
    /// Helper method which acquires the next interaction for the inhabitant to perform. This method
    /// should never return a null interaction data due to a previous check for remaining interactions
    /// in the sequence.
    /// </summary>
    /// <param name="isInterruption">Boolean which indicates whether the interaction was selected as an interruption.</param>
    /// <returns>An interaction which was randomly selected from the interaction sequence</returns>
    private InteractionData TryGetNextInteraction(out bool isInterruption)
    {
        InteractionData nextInteraction;
        isInterruption = DoesInterruptionOccur();

        if (isInterruption)
        {
            // Get a random interaction from the interruption sequence
            int randomIndex = RNG.Instance.Range(0, InterruptionSequence.Length);
            nextInteraction = InterruptionSequence[randomIndex];
            CurrentInterruptionCount++;
        }
        else
        {
            // ... Otherwise, get the next interaction from the interaction sequence
            nextInteraction = InteractionSequence[CurrentInteractionCount];
            CurrentInteractionCount++;
        }

        return nextInteraction;
    }

    /// <summary>
    /// Helper method which determines whether an interruption occurs.
    /// </summary>
    /// <returns>Boolean which indicates whether an interruption occurs.</returns>
    private bool DoesInterruptionOccur()
    {
        // Only check for interruption if maximum threshold not exceeded
        if (CurrentInterruptionCount < MaxNumberOfInterruptions)
        {
            if (RNG.Instance)

                // Determine if an interruption occurs
                if (RNG.Instance.Range(0f, 1f) <= CurrentInterruptionRate)
                    return true;

            Debug.LogError("RNG is not defined in the scene.");
        }

        return false;
    }

    /// <summary>
    /// Helper method which calculates a modified interaction duration
    /// based on preset parameters. This is done to introduce variation
    /// to interaction durations.
    /// </summary>
    /// <param name="initialDuration">The initial interaction duration</param>
    /// <returns>Modified interaction duration</returns>
    private float CalculateInteractionDurationVariance(float initialDuration)
    {
        if (RNG.Instance)
        {
            // NOTE: Hard clamp on duration variance, could introduce properties to control this.
            float randomDurationVariance = RNG.Instance.Range(-MaxInteractionDurationVariance, MaxInteractionDurationVariance);
            return Mathf.Clamp(initialDuration + randomDurationVariance, 0f, 100f);
        }

        Debug.LogError("RNG is not defined in the scene.");
        return 0f;
    }

    /// <summary>
    /// Helper method which queries for a specific game object containing an interactable
    /// component and a specific name. This query is supported using the AllHomeObjects
    /// class/component.
    /// </summary>
    /// <param name="interactionGameObject">The target game object which was queried.</param>
    /// <param name="interactionData">Metadata for querying a specific game object.</param>
    /// <returns>A game object containing a interactable component with the specified name in the passed metadata.</returns>
    private bool TryGetGameObjectUsingInteractionData(out GameObject interactionGameObject,
        InteractionData interactionData)
    {
        interactionGameObject = null;
        if (AllHomeObjects.Instance)
        {
            if (AllHomeObjects.Instance.TryFindInteractableGameObjectUsingName(
                interactionData.TargetGameObjectName, out interactionGameObject))
                return true;
        }

        Debug.LogError("Home Objects Manager not defined in the scene.");
        return false;
    }

    /// <summary>
    /// Helper method which notifies the GUI events observer of the next interaction to be performed
    /// by the inhabitant.
    /// </summary>
    /// <param name="nextInteraction">Contains data of the interaction to be performed</param>
    /// <param name="interactionDuration">Adjusted duration of the interaction to be performed</param>
    /// <param name="isInterruption">Boolean which indicates whether the interaction is an interruption</param>
    /// <returns>True if the observer was successfully notified. False if otherwise.</returns>
    private bool UpdateGUI(InteractionData nextInteraction, float interactionDuration, bool isInterruption)
    {
        // Update GUI with the appropriate information
        if (GUILayerEventsObserver.Instance)
        {
            GUILayerEventsObserver.Instance.TriggerInteractionSelectionEvent(nextInteraction, interactionDuration, isInterruption);

            if (isInterruption)
            {
                GUILayerEventsObserver.Instance.TriggerBroadcastMessageToMainPanelEvent(MainTextPanelMessage.INTERRUPT_TEXT);
                GUILayerEventsObserver.Instance.TriggerADLRunStatEvent(GUILayerEventsObserver.KEY_INTERRUPT_COUNT, CurrentInterruptionCount);
            }
            else
            {
                GUILayerEventsObserver.Instance.TriggerBroadcastMessageToMainPanelEvent(MainTextPanelMessage.MOVE_TO_TEXT +
                    nextInteraction.TargetGameObjectName);
                GUILayerEventsObserver.Instance.TriggerADLRunStatEvent(GUILayerEventsObserver.KEY_CURRENT_INTERACTION_COUNT, CurrentInteractionCount);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Helper BT callback method which simply resets interaction-related data for
    /// future activity performances.
    /// </summary>
    /// <returns>True if GUI updated. False if otherwise.</returns>
    [Task]
    public bool ResetInteractionData()
    {
        // Reset the interaction counters and update GUI
        CurrentInterruptionCount = 0;
        CurrentInteractionCount = 0;

        if (GUILayerEventsObserver.Instance)
        {
            GUILayerEventsObserver.Instance.TriggerADLRunStatEvent(GUILayerEventsObserver.KEY_INTERRUPT_COUNT,
                CurrentInterruptionCount);
            GUILayerEventsObserver.Instance.TriggerADLRunStatEvent(GUILayerEventsObserver.KEY_CURRENT_INTERACTION_COUNT,
                CurrentInteractionCount);
            return true;
        }

        Debug.LogError("GUIEventsObserver is not defined in the scene.");
        return false;
    }

    /// <summary>
    /// BT callback method which randomises the inhabitants position based on defined
    /// spawn locations in the scene. This method requires the AllHomeObjects game
    /// object to be defined in the scene.
    /// </summary>
    /// <returns>True if inhabitant was successfully moved to a random spawner. False if otherwise.</returns>
    [Task]
    public bool TeleportInhabitantToRandomSpawner()
    {
        GameObject spawnLocation;
        if (AllHomeObjects.Instance)
        {
            // Queries for spawner using AllHomeObjects
            if (AllHomeObjects.Instance.TryGetRandomSpawner(out spawnLocation))
            {
                // ... and teleports inhabitant by telling the inhabitant controller
                InhabitantControllerComponent.TeleportInhabitantToPosition(spawnLocation.transform.position);
                return true;
            }

            Debug.LogError("No spawners are defined in the scene.");
            return false;
        }

        Debug.LogError("AllHomeObjects is not defined in the scene.");
        return false;
    }

    private System.DateTime DateTimeForNextActivity;

    /// <summary>
    /// BT helper callback method which enables users to specify the initial time
    /// for activity performances.
    /// </summary>
    /// <param name="hours"></param>
    /// <param name="minutes"></param>
    /// <param name="seconds"></param>
    /// <returns>True if initial time was sucessfully specified. False if otherwise.</returns>
    [Task]
    public bool SetInitialTimeForNextActivity(int hours, int minutes, int seconds)
    {
        if (DateController.Instance)
        {
            // Reference current date and update with the specified time parameters
            DateTimeForNextActivity = System.DateTime.MaxValue;
            DateTimeForNextActivity = DateController.Instance.GetCurrentDate(DateTimeForNextActivity);
            DateTimeForNextActivity = DateTimeForNextActivity.Date + new System.TimeSpan(hours, minutes, seconds);
            return true;
        }

        Debug.LogError("DateController is not defined in the scene.");
        return false;
    }

    /// <summary>
    /// BT helper callback method which adjusts the initial time for activity performances
    /// based on defined threshold parameters.
    /// </summary>
    /// <returns>True if time was adjusted successfully. False if otherwise.</returns>
    [Task]
    public bool AdjustTimeForNextActivity()
    {
        if (RNG.Instance)
        {
            // Adjust the time parameters
            DateTimeForNextActivity.AddHours(
                Mathf.Clamp(RNG.Instance.Range(-MaxTimeHoursVariance, MaxTimeHoursVariance), 0, TimeController.HOURS_IN_DAY - 1));
            DateTimeForNextActivity.AddMinutes(
                Mathf.Clamp(RNG.Instance.Range(-MaxTimeMinutesVariance, MaxTimeMinutesVariance), 0, TimeController.MINUTES_IN_HOUR - 1));
            DateTimeForNextActivity.AddSeconds(
                Mathf.Clamp(RNG.Instance.Range(-MaxTimeSecondsVariance, MaxTimeSecondsVariance), 0, TimeController.SECONDS_IN_MINUTE - 1));

            // ... then update the time controller using the adjusted time
            if (TimeController.Instance)
            {
                TimeController.Instance.SetTimeOfDay(DateTimeForNextActivity, false);
                return true;
            }

            Debug.LogError("TimeController is not defined in the scene.");
            return false;
        }

        Debug.LogError("RNG is not defined in the scene.");
        return false;
    }

    /// <summary>
    /// BT helper callback for adding a sensor bookmark to the sensor stream.
    /// </summary>
    /// <param name="newBookmarkName">Bookmark identifier to be added to sensor stream.</param>
    /// <returns>True if bookmark successfully added to sensor stream. False if otherwise.</returns>
    [Task]
    public bool AddSensorBookmark(string newBookmarkName)
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerAddSensorBookmarkEvent(newBookmarkName))
                return true;

            Debug.LogError("Sensor stream event/action for adding sensor bookmark does not exist.");
        }

        Debug.LogError("Logic Events Observer is not defined in the scene.");
        return false;
    }

    /// <summary>
    /// BT helper callback for annotating the end of a sensor bookmark in the sensor stream.
    /// </summary>
    /// <param name="bookmarkName">Bookmark identifier to be modified in the sensor stream.</param>
    /// <returns>True if bookmark was modified. False if otherwise.</returns>
    [Task]
    public bool AnnotateSensorBookmarkEnd(string bookmarkName)
    {
        if (SimulationLayerEventsObserver.Instance)
        {
            if (SimulationLayerEventsObserver.Instance.TriggerAnnotateEndSensorBookmarkEvent(bookmarkName))
                return true;

            Debug.LogError("Sensor stream event/action for annotating the end of a sensor bookmark does not exist.");
        }

        Debug.LogError("Logic Events Observer is not defined in the scene.");
        return false;
    }
}
