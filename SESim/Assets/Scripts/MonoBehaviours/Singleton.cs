using UnityEngine;

/// <summary>
/// Parent abstract class for single monobehaviours (components). This enables class
/// enables components to be exposed globally, where only one instance of the component
/// exists (singleton).
/// </summary>
/// <typeparam name="T">The class/component to be implemented as a singleton</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            try
            {
                if (!instance)
                    instance = FindObjectOfType(typeof(T)) as T;

                if (!instance)
                    Debug.LogError("Missing " + typeof(T).FullName +
                        " game object in scene. Please create one in the scene.");

                return instance;
            }
            catch (System.NullReferenceException e)
            {
                throw e;
            }
        }
    }

    public virtual void Awake()
    {
        // DEBUG: If finding that the game managers are null, useful to use to check if duplicates
        //if (instance)
        //{
        //    Debug.Log("Instance of " + typeof(T).FullName + " already exists. Ensure that " +
        //        "there are not multiple copies of this component.");

        //    return;
        //}

        if (!instance)
            instance = this as T;
    }

    public virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
