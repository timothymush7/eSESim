using UnityEngine;

/// <summary>
/// Child implementation of the display gizmo class. This is used to
/// describe gizmo squares for spawners, which may use a different colour.
/// </summary>
public class DisplayGizmoSpawner : DisplayGizmo
{
    public override void SetDrawColour()
    {
        Gizmos.color = Color.blue;
    }
}
