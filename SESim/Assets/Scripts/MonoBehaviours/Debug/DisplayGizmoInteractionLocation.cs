using UnityEngine;

/// <summary>
/// Child implementation of the display gizmo class. This is used to
/// describe gizmo squares for interactable locations, which may use
/// a different colour.
/// </summary>
public class DisplayGizmoInteractionLocation : DisplayGizmo
{
    public override void SetDrawColour()
    {
        Gizmos.color = Color.red;
    }
}
