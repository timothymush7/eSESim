using UnityEngine;

/// <summary>
/// Class describing Voxel information. Created to be attached to
/// game objects representing voxels, to provide voxel information 
/// that is visible in the editor. Purely debugging purposes.
/// </summary>
public class VoxelInfo : MonoBehaviour
{
    [Header("Voxel Attributes/Properties")]
    [Tooltip("Current temperature of this voxel.")] [SerializeField] private float CurrentTemperature;
    [Tooltip("Boolean indicating whether the voxel collides with an obstacle.")] [SerializeField] private bool IsOnObstacle = false;

    [Header("Voxel Coordinates")]
    [Tooltip("Indices for voxel in voxel mesh.")] [SerializeField] private Vector3 VoxelMeshIndices;
    [Tooltip("Position of voxel in voxel mesh.")] [SerializeField] private Vector3 InitialMeshPosition;
    [Tooltip("Position of voxel in voxel mesh (centered)")] [SerializeField] private Vector3 CenteredMeshPosition;

    /// <summary>
    /// Utility method for updating the voxel information in this component
    /// using a specified voxel.
    /// </summary>
    /// <param name="aVoxel">Voxel whose information is used for updating this component.</param>
    public void UpdateVoxelInformation(Voxel aVoxel)
    {
        VoxelMeshIndices = aVoxel.VoxelMeshIndices;
        InitialMeshPosition = aVoxel.InitialMeshPosition;
        CenteredMeshPosition = aVoxel.CenteredMeshPosition;
        CurrentTemperature = aVoxel.GetTemperature();
        IsOnObstacle = aVoxel.IsOnObstacle;
    }
}
