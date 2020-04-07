using UnityEngine;

/// <summary>
/// Parent class that handles drawing of game object transform onto the ground.
/// This class is primarily created to facilitate the display of locations or positions
/// in the scene for empty game objects with only a transform component. For example,
/// spawn locations or interaction locations.
/// </summary>
[RequireComponent(typeof(Transform))]
public abstract class DisplayGizmo : MonoBehaviour
{
    [Tooltip("Boolean which indicates whether the gizmo should be drawn.")] public bool DisplayLocationGizmo = true;

    private void OnDrawGizmos()
    {
        if (DisplayLocationGizmo)
        {
            SetDrawColour();
            DrawTransformSquare();
        }
    }

    public virtual void SetDrawColour()
    {
        // Gizmo colour is white by default
        Gizmos.color = Color.white;
    }

    public virtual void DrawTransformSquare()
    {
        SquarePositions squarePositions = CalculateSquarePositions(transform.position, Vector3.one, 0.5f);

        /*
            The following lines of code draw a square with the specified 
            transform position as the center, and also include the diagonal
            lines of the square to make the gizmos obvious in the scene.
        */

        Gizmos.DrawLine(squarePositions.BottomLeft, squarePositions.BottomRight);
        Gizmos.DrawLine(squarePositions.TopLeft, squarePositions.BottomLeft);
        Gizmos.DrawLine(squarePositions.TopLeft, squarePositions.TopRight);
        Gizmos.DrawLine(squarePositions.TopRight, squarePositions.BottomRight);
        Gizmos.DrawLine(squarePositions.TopRight, squarePositions.BottomLeft);
        Gizmos.DrawLine(squarePositions.TopLeft, squarePositions.BottomRight);
    }

    /// <summary>
    /// Struct which describes the four points of a square as positions.
    /// </summary>
    protected struct SquarePositions
    {
        public Vector3 TopLeft, TopRight, BottomLeft, BottomRight;
    }

    /// <summary>
    /// Helper method which calculates the positions of the four square positions using a specified
    /// transform, scale, and height. It is important to note that the transform position is used
    /// as the center for the square positions.
    /// </summary>
    /// <param name="transform">Center position for calculating square positions</param>
    /// <param name="scale">Scale between the square positions</param>
    /// <param name="height">Height of the square positions</param>
    /// <returns></returns>
    protected SquarePositions CalculateSquarePositions(Vector3 transform, Vector3 scale, float height)
    {
        return new SquarePositions
        {
            TopRight = new Vector3(transform.x + scale.x / 2, height, transform.z + scale.z / 2),
            TopLeft = new Vector3(transform.x - scale.x / 2, height, transform.z - scale.z / 2),
            BottomRight = new Vector3(transform.x + scale.x / 2, height, transform.z - scale.z / 2),
            BottomLeft = new Vector3(transform.x - scale.x / 2, height, transform.z + scale.z / 2)
        };
    }
}
