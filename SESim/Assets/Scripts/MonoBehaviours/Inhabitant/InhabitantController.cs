using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
/// <summary>
/// The inhabitant controller is responsible for controlling inhabitant behaviour and
/// movement. Movement actions are received by a PBInhabitantHandler component, which
/// uses BT scripts to specify inhabitant behaviour and movement.
/// </summary>
public class InhabitantController : MonoBehaviour
{
    [Header("Inhabitant Attributes")]
    [Tooltip("Unique inhabitant identifier.")] public string InhabitantID = "";
    [Tooltip("Interpolation constant for calculating rotations.")] public float RotationSmoothingRate = 15f;
    [Tooltip("Speed threshold for processing inhabitant rotations.")] public float RotateSpeedThreshold = 0f;
    [Tooltip("Amount of time before change in move speed is applied.")] public float MoveSpeedDampTimeForAnimator = 1f;

    [Header("Interaction-related Data")]
    [Tooltip("The current game object destination for the inhabitant.")] [ReadOnly] public Interactable CurrentInteractable;

    /*
        NOTE:

        Important to understand that CanReceiveMovementActions != !isInteracting
        - CanReceiveMovementActions indicates when the inhabitant is available to
          set new destinations for itself.
        - isInteracting indicates whether the inhabitant is busy interacting with
          a game object.

        Once the inhabitant reaches a destination, the inhabitant may be "available"
        to receive new movement actions, however it may be busy interacting with a
        game object.
    */

    [Tooltip("Indicates whether new movement actions can be specified.")] [ReadOnly] public bool CanReceiveMovementActions = true;
    [Tooltip("Indicates whether the inhabitant is currently processing an interaction.")] [ReadOnly] public bool isInteracting = false;

    private readonly int MoveSpeedHash
        = Animator.StringToHash("MoveSpeed");                           // Hash value representing the paramater in the ANIMATOR, 'MoveSpeed'
    private Animator InhabitantAnimator;                                // Reference to the animation controller
    private NavMeshAgent InhabitantNavMeshAgent;                        // Reference to the nav mesh agent
    private Vector3 DestinationPosition;                                // Destination position which the player is travelling to
    private string ReactionCollectionDescriptionForInteraction = "";    // Description for indicating the reactions to process during an interaction.
    private StopwatchTimer InteractionWaitTimer;                        // Stopwatch timer object which is used for handling the waiting period for interactions

    /// <summary>
    ///  Unity method callback which is called at the start when running a scene.
    ///  This method handles the initialisation for inhabitant controller.
    /// </summary>
    void Start()
    {
        // Reference the appropriate components (should never be null due to restriction)
        InhabitantAnimator = GetComponent<Animator>();
        InhabitantNavMeshAgent = GetComponent<NavMeshAgent>();

        // Initially set destination = current position, thus inhabitant is ready for movement
        DestinationPosition = transform.position;

        // Disable automatic rotation updates - this is handled manually
        InhabitantNavMeshAgent.updateRotation = false;

        // Setup timer to handle interaction waiting
        InteractionWaitTimer = new StopwatchTimer(0f);
        InteractionWaitTimer.OnDelayElapsed += OnInteractionWaitFinish;
    }

    /// <summary>
    /// Callback method which is only executed once the waiting period
    /// for an interaction has elapsed.
    /// </summary>
    private void OnInteractionWaitFinish()
    {
        if (CurrentInteractable)
        {
            CurrentInteractable.StopInteract();
            CurrentInteractable = null;
            CanReceiveMovementActions = true;
            isInteracting = false;
        }
    }

    /// <summary>
    /// Unity callback method which is called during each frame update. Movement and behaviour
    /// updates are processed here.
    /// </summary>
    void Update()
    {
        // If calculating path towards a destination - stop, no movement to process
        if (InhabitantNavMeshAgent.pathPending)
            return;

        // Update timer if inhabitant is busy with interacting, otherwise update movement
        if (isInteracting)
        {
            if (InteractionWaitTimer != null)
                InteractionWaitTimer.Update();
        }
        else
            UpdateInhabitantMovement();
    }

    /// <summary>
    /// Method which updates the movement speed of the inhabitant based on its current actions.
    /// </summary>
    private void UpdateInhabitantMovement()
    {
        // Reference latest movement speed from nav mesh agent component
        float moveSpeed = InhabitantNavMeshAgent.desiredVelocity.magnitude;

        // Within stopping distance of destination? -> stop.
        if (InhabitantNavMeshAgent.remainingDistance <= InhabitantNavMeshAgent.stoppingDistance)
        {
            ApplyStopping();
            moveSpeed = 0f;
        }
        // Otherwise, apply rotation to inhabitant (movement is done by nav mesh agent). 
        else if (moveSpeed > RotateSpeedThreshold)
            ApplyRotation();

        // Update movement speed value in animator - let's animator know what animation to use
        InhabitantAnimator.SetFloat(MoveSpeedHash, moveSpeed,
            MoveSpeedDampTimeForAnimator, Time.deltaTime);
    }

    /// <summary>
    /// This method applies the functional logic to stop the inhabitant at the
    /// destination, which has been reached by the inhabitant.
    /// </summary>
    private void ApplyStopping()
    {
        // Apply stopping status, etc.
        InhabitantNavMeshAgent.isStopped = true;
        TeleportInhabitantToPosition(DestinationPosition);

        // If at destination -> perform interaction if defined
        if (IsInhabitantAtDestination())
            if (CurrentInteractable)
                InitiateInteraction(CurrentInteractable);
    }

    /// <summary>
    /// Method which handles the processing of interactions with an
    /// interactable object.
    /// </summary>
    /// <param name="anInteractable">Interactable component of the target game object</param>
    private void InitiateInteraction(Interactable anInteractable)
    {
        // Update GUI about interaction
        if (GUILayerEventsObserver.Instance)
            GUILayerEventsObserver.Instance.TriggerBroadcastMessageToMainPanelEvent(
                MainTextPanelMessage.INTERACTING_TEXT + anInteractable.name);

        // Make inhabitant face towards interactable from the interaction location
        transform.LookAt(anInteractable.transform.position);

        // Initiate interaction and waiting timer for the interaction
        anInteractable.InteractWithReaction(
            ReactionCollectionDescriptionForInteraction);
        InteractionWaitTimer.Reset();
        isInteracting = true;
    }

    /// <summary>
    /// Method which manually handles the rotation of the inhabitant game
    /// object during movement.
    /// </summary>
    private void ApplyRotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(InhabitantNavMeshAgent.desiredVelocity);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
            RotationSmoothingRate * Time.deltaTime);
    }

    /// <summary>
    /// Unity method callback which is invoked during frame updates after the state machines
    /// and animations have been calculated.
    /// </summary>
    private void OnAnimatorMove()
    {
        /*
        *   This part needs to be fixed in a future update.
        *   Original code used animator speed, however this did not work.
        *   Decided to use nav mesh desired velocity in the meantime.
        */

        InhabitantNavMeshAgent.velocity = InhabitantNavMeshAgent.desiredVelocity / Time.deltaTime;
    }

    /// <summary>
    /// Helper method which teleports the inhabitant game object to the specified
    /// position.
    /// </summary>
    /// <param name="newPosition">Target position for teleporting the inhabitant game object</param>
    public void TeleportInhabitantToPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    /// <summary>
    /// Helper method which checks whether the inhabitant has reached it's specified destination.
    /// </summary>
    /// <returns>True if inhabitant has reached it's destination. False otherwise.</returns>
    public bool IsInhabitantAtDestination()
    {
        if ((InhabitantNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete))
            return true;
        return false;
    }

    public void SetupInteractionAsNextDestination(GameObject interactableGameObject,
        string reactionCollectionDescription,
        float interactionDuration)
    {
        // Set next destination for inhabitant
        CanReceiveMovementActions = false;
        CurrentInteractable = interactableGameObject.GetComponent<Interactable>();
        SetDestinationPosition(CurrentInteractable.InteractionLocation.position);

        // Prepare interaction-related data (for when the destination has been reached)
        InteractionWaitTimer.SetDuration(interactionDuration, false);
        ReactionCollectionDescriptionForInteraction = reactionCollectionDescription;
    }

    private void SetDestinationPosition(Vector3 destinationPosition)
    {
        DestinationPosition = destinationPosition;
        InhabitantNavMeshAgent.SetDestination(DestinationPosition);
        InhabitantNavMeshAgent.isStopped = false;
        Debug.Log("New destination set.");
    }
}
