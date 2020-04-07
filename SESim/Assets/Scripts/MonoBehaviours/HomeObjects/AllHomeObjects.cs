using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager that caches references to various games objects under specific tags. This
/// class/component is used to provide the ability to quickly query for specific game
/// objects without constantly using the Unity Find method.
/// </summary>
public class AllHomeObjects : Singleton<AllHomeObjects>
{
    /*
        Unique strings representing the various tags used in the scene.
        These tags are case-sensitive and must match the tags used
        in the scene.
    */

    public static string TAG_INTERACTABLE = "Interactable";
    public static string TAG_SPAWNER = "SpawnLocation";
    public static string TAG_HEAT_EMITTER = "HeatEmitter";
    public static string TAG_SENSOR = "Sensor";
    public static string TAG_LIGHT = "Light";

    /*
        Custom keys made for storing specific game objects.
        These keys are NOT tags in the scene.
    */

    public static string KEY_POLL_SENSORS = "PollSensors";
    public static string KEY_POINT_LIGHTS = "PointLights";

    private Dictionary<string, GameObject[]> TagNameToGameObjectArrays;

    /*
        Created custom lists for the custom keys to avoid
        storage of redundant properties in array/less memory intensive.
    */

    private List<Light> PointLights;
    private List<PollSensor> PollSensorComponents;

    void Start()
    {
        // Initialise data structures for caching references
        TagNameToGameObjectArrays = new Dictionary<string, GameObject[]>();
        PointLights = new List<Light>();
        PollSensorComponents = new List<PollSensor>();

        // Cache game object references in dictionary
        TryAddGameObjectArrayByTag(TAG_INTERACTABLE, false);
        TryAddGameObjectArrayByTag(TAG_SPAWNER, true);
        TryAddGameObjectArrayByTag(TAG_HEAT_EMITTER, false);
        TryAddGameObjectArrayByTag(TAG_SENSOR, false);
        TryAddGameObjectArrayByTag(TAG_LIGHT, false);

        // Cache game object references in respective lists
        FindAndStoreAllPointLights();
        FindAndStoreAllPollSensors();
    }

    /// <summary>
    /// Helper method for acquiring references to game objects with specific tags and
    /// caching these references in a dictionary.
    /// </summary>
    /// <param name="tagName">Tag for acquiring specific game object references.</param>
    /// <param name="shuffleList">Boolean to indicate whether the list should be shuffled before being cached.</param>
    /// <returns></returns>
    private bool TryAddGameObjectArrayByTag(string tagName, bool shuffleList)
    {
        GameObject[] newComponentGameObjects = GameObject.FindGameObjectsWithTag(tagName);
        if (newComponentGameObjects != null)
        {
            if (shuffleList)
                newComponentGameObjects.ShuffleList();

            TagNameToGameObjectArrays[tagName] = newComponentGameObjects;
            return true;
        }

        Debug.LogError("Game objects with the tag name: '" + tagName + "' was not found in the scene.");
        return false;
    }

    /// <summary>
    /// Primary utility method for querying the cache of game object references.
    /// This method acquires all game objects of a specified tag.
    /// </summary>
    /// <param name="tagName">Tag name for game object references</param>
    /// <param name="gameObjectArray">The output array consisting of game objects with a specific tag</param>
    /// <returns></returns>
    public bool TryFindGameObjectArray(string tagName, out GameObject[] gameObjectArray)
    {
        gameObjectArray = null;
        if (TagNameToGameObjectArrays.ContainsKey(tagName))
        {
            gameObjectArray = TagNameToGameObjectArrays[tagName];
            return true;
        }

        Debug.LogError("Dictionary does not contain key, '" + tagName + "'.");
        return false;
    }

    #region Utility Methods

    /// <summary>
    /// Utility method for acquiring a specific game object containing a interactable component.
    /// </summary>
    /// <param name="name">Name of the specific game object containing a interactable component.</param>
    /// <param name="interactableGameObject">The acquired game object, which is referenced.</param>
    /// <returns></returns>
    public bool TryFindInteractableGameObjectUsingName(string name, out GameObject interactableGameObject)
    {
        // Iterate through all cached interactable game objects
        interactableGameObject = null;
        GameObject[] interactableGameObjects;
        if (TryFindGameObjectArray(TAG_INTERACTABLE, out interactableGameObjects))
        {
            for (int i = 0; i < interactableGameObjects.Length; i++)
            {
                // Return interactable game object with the specific name
                if (interactableGameObjects[i].name.Equals(name))
                {
                    interactableGameObject = interactableGameObjects[i];
                    return true;
                }
            }
        }

        Debug.LogError("HomeObjectsManager - Interactable with name: '"
            + name + "' was not found.");
        return false;
    }

    /// <summary>
    /// Utility method for acquiring a random spawner in the scene.
    /// </summary>
    /// <param name="randomSpawner">Random spawner assigned from this method.</param>
    /// <returns>True if a random spawner was assigned. False otherwise.</returns>
    public bool TryGetRandomSpawner(out GameObject randomSpawner)
    {
        randomSpawner = null;

        if (RNG.Instance)
        {
            GameObject[] spawnLocations;
            if (TryFindGameObjectArray(TAG_SPAWNER, out spawnLocations))
            {
                // Generate random number using RNG, return random spawner
                randomSpawner = spawnLocations[RNG.Instance.Range(0, spawnLocations.Length)];
                return true;
            }

            Debug.LogError("No spawner was found in the scene.");
        }

        Debug.LogError("RNG does not defined in the scene.");
        return false;
    }

    /// <summary>
    /// Utility method for toggling the state of all heat emitting game objects
    /// in the scene.
    /// </summary>
    /// <param name="activateHeatEmitters">Boolean indicating the resultant state of all heat emitting game objects.</param>
    public void ToggleAllHeatEmitters(bool activateHeatEmitters)
    {
        GameObject[] heatEmitterGameObjects;
        if (TryFindGameObjectArray(TAG_HEAT_EMITTER, out heatEmitterGameObjects))
        {
            for (int heIndex = 0; heIndex < heatEmitterGameObjects.Length; heIndex++)
            {
                HeatEmitter producerComponent = heatEmitterGameObjects[heIndex].GetComponent<HeatEmitter>();
                producerComponent.ToggleEmissions(activateHeatEmitters);
            }
        }
    }

    /// <summary>
    /// Utility method for toggling all poll sensors in the scene to a
    /// specific state.
    /// </summary>
    /// <param name="activatePollSensors">Boolean indicating the resulting state of all poll sensors.</param>
    public void ToggleAllPollSensors(bool activatePollSensors)
    {
        foreach (PollSensor pollSensorComp in PollSensorComponents)
            pollSensorComp.TogglePolling(activatePollSensors, true);
    }

    /// <summary>
    /// Utility method for toggling all point lights in the scene to
    /// a specified state.
    /// </summary>
    /// <param name="activateLights">Boolean indicating the resulting state of all point lights.</param>
    public void ToggleLights(bool activateLights)
    {
        // Iterate over cached point lights and toggle all
        foreach (Light aLight in PointLights)
        {
            // Make sure to check for parent and light source controller, might be a lone point light
            if (aLight.transform.parent)
            {
                if (aLight.GetComponentInParent<LightSourceController>())
                    aLight.GetComponentInParent<LightSourceController>().ToggleLightSource(activateLights);
            }
        }
    }

    /// <summary>
    /// Utility method for toggling a specific point light in the scene. Uses
    /// the game object name as a unique identifier.
    /// </summary>
    /// <param name="gameObjectName">Name of the point light game object.</param>
    /// <param name="activateLight">Boolean indicating the toggling state.</param>
    public void ToggleLight(string gameObjectName, bool activateLight)
    {
        // Iterate over cached point lights and toggle the specific point light
        foreach (Light aLight in PointLights)
        {
            // Make sure to check for parent and light source controller, might be a lone point light
            if (aLight.transform.parent)
            {
                if (aLight.transform.parent.name.Equals(gameObjectName))
                {
                    if (aLight.GetComponentInParent<LightSourceController>())
                    {
                        aLight.GetComponentInParent<LightSourceController>().ToggleLightSource(activateLight);
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Helper method which caches all game objects which contain a light source
    /// with the "point" light type.
    /// </summary>
    private void FindAndStoreAllPointLights()
    {
        // Iterate over cached light game objects
        GameObject[] lightGameObjects;
        PointLights.Clear();
        if (TryFindGameObjectArray(TAG_LIGHT, out lightGameObjects))
        {
            foreach (GameObject lightGameObject in lightGameObjects)
            {
                // Light component always defined as only game objects with light components dealt with here
                if (lightGameObject.GetComponent<Light>().type == LightType.Point)
                    PointLights.Add(lightGameObject.GetComponent<Light>());
            }
        }
    }

    /// <summary>
    /// Helper method which caches game objects which contain a poll
    /// sensor component.
    /// </summary>
    private void FindAndStoreAllPollSensors()
    {
        // Iterate over cached sensor game objects
        GameObject[] sensorGameObjects;
        PollSensorComponents.Clear();
        if (TryFindGameObjectArray(TAG_SENSOR, out sensorGameObjects))
        {
            foreach (GameObject sensorGameObject in sensorGameObjects)
            {
                // Check if it has a poll sensor component - add the game object to the resultant list if defined
                PollSensor pollSensorComponent = sensorGameObject.GetComponent<PollSensor>();
                if (pollSensorComponent)
                    PollSensorComponents.Add(pollSensorComponent);
            }
        }
    }

    /// <summary>
    ///  Utility method for acquiring all cached point lights in the scene.
    /// </summary>
    /// <returns>List of light components of all point lights in the scene.</returns>
    public List<Light> GetPointLights()
    {
        return PointLights;
    }

    #endregion
}
