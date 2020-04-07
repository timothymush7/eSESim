using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity event exclusively used by heat emitters for notifying the voxel mesh.
/// This unity event takes in three arguments, namely a Vector3, integer, and a float.
/// </summary>
public class UnityEventEmitHeat : UnityEvent<Vector3, int, float>
{

}
