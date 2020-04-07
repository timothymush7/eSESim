using UnityEngine;
using System.Collections;

/// <summary>
/// Class/component which handles the rotation of doors based on
/// the status of a presence sensor.
/// </summary>
public class DoorController : MonoBehaviour
{
    /*
        Purely cosmetic effect. I had this disabled for simulations as it used
        unnecessary computation.
    */

    [Tooltip("Presence sensor which is monitored for triggering door movements.")] public PresenceSensor DoorPresenceSensor;
    [Tooltip("Angle for describing the door after it has been rotated to indicate that it is open.")] public float DoorOpenAngle = 90f;
    [Tooltip("Angle for describing the door after it has been rotated to indicate that it is closed.")] public float DoorClosedAngle = 0f;
    [Tooltip("Constant for describing the transition time between opening and closing the door.")] [Range(0, 1)] public float DoorRotationSmoothingTime = 0.5f;
    [Tooltip("Constant for a delay before beginning the door rotation.")] public float TimeDelayBeforeUpdateRotation = 0.05f;
    [Tooltip("Boolean which indicates whether door rotations are processed.")] public bool ShowDoorRotations = false;

    private WaitForSeconds WaitRotationDelay;
    private bool IsDoorOpen = false;

    void Start()
    {
        if (ShowDoorRotations)
            SetupDoorRotation();
    }

    /// <summary>
    /// Helper method for performing the setup of door rotations.
    /// </summary>
    private void SetupDoorRotation()
    {
        // Do door rotation setup only if the presence sensor is defined
        if (DoorPresenceSensor)
        {
            WaitRotationDelay = new WaitForSeconds(TimeDelayBeforeUpdateRotation);
            DoorPresenceSensor.FirstPresenceDetectedEvent += ToggleDoor;
        }
        else
            Debug.LogError("Door '" + name + "': no motion sensor found.");
    }

    /// <summary>
    /// Primary helper method for toggling door status via rotations.
    /// </summary>
    private void ToggleDoor()
    {
        // Invert door status
        IsDoorOpen = !IsDoorOpen;

        // Stop existing coroutines, begin new one
        StopCoroutine("PerformDoorRotation");
        StartCoroutine("PerformDoorRotation", IsDoorOpen);
    }

    /// <summary>
    /// Coroutine enumerator for processing door rotations.
    /// </summary>
    /// <param name="openDoor">Boolean which indicates whether the door is to be opened or closed.</param>
    /// <returns></returns>
    IEnumerator PerformDoorRotation(bool openDoor)
    {
        // Set target angle based on door action (open or close)
        float targetAngle = DoorClosedAngle;
        if (openDoor)
            targetAngle = DoorOpenAngle;

        // Repeat until the y-rotation has reached destination
        while (transform.rotation.y != targetAngle)
        {
            Quaternion newRotation = Quaternion.AngleAxis(targetAngle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                newRotation, DoorRotationSmoothingTime);
            yield return WaitRotationDelay;
        }
    }
}
