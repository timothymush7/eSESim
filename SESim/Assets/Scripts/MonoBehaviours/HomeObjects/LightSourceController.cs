using UnityEngine;

///<summary>
/// Component which consists of the functional logic for managing
/// the behaviour of the light source (toggling via light switch).
///</summary>
public class LightSourceController : MonoBehaviour {

    [Tooltip("The light source game object to be managed.")] public Light LightSource;
    [Tooltip("The interactable component in which interactions are monitored for managing the light source.")] public Interactable LightSwitchInteractable;

    private static string TOGGLE_EVENT_IDENTIFIER = "lightswitch";  // Unique identifier for the light switch event

    private void Awake()
    {
        // Listen to interactions from light switch, setup initial light state
        if (LightSwitchInteractable)
            LightSwitchInteractable.AddListenerToToggleEvent(ToggleLightSource, TOGGLE_EVENT_IDENTIFIER);
    }

    /// <summary>
    /// Method for toggling the attached light source. This method
    /// explicitly enables users to specify the state of the light source.
    /// </summary>
    /// <param name="activateLight">Boolean value to indicate light source state.</param>
    public void ToggleLightSource(bool activateLight)
    {
        if (LightSource)
            LightSource.enabled = activateLight;
    }

    /// <summary>
    ///  Method for toggling the attached light source. This method
    ///  simply inverts the light source state.
    /// </summary>
    public void ToggleLightSource()
    {
        if (LightSource)
            LightSource.enabled = !LightSource.enabled;
    }
}
