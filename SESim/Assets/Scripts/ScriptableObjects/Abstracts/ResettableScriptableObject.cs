using UnityEngine;


/// <summary>
/// Scriptable objects are versatile data containers in unity, often strongly
/// associated with editors. Read more on this @ (https://docs.unity3d.com/Manual/class-ScriptableObject.html).
/// 
/// The resettable scriptable object includes a reset method, which enables a
/// clean method of resetting data in a scriptable object.
/// </summary>
public abstract class ResettableScriptableObject : ScriptableObject
{
    public abstract void Reset();
}
