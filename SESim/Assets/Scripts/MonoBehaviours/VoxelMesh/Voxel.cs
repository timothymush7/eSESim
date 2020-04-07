using UnityEngine;

/// <summary>
/// Class containing the functional logic for the voxel, a single air space in
/// the environment. Voxels are used by voxel meshes to construct a model of 
/// air spaces in the environment.
/// </summary>
public class Voxel
{
    /*
        The voxel maintains three positions, which are cached for debugging purposes:

        1.) VoxelArrayIndices - the indices it represents in the voxel mesh
        2.) InitialMeshPosition - the position of the voxel in the voxel mesh (bottom left as origin)
        3.) CenteredMeshPosition -  the initial mesh position + offset to have the air spaces centered.
    */

    public Vector3 VoxelMeshIndices, InitialMeshPosition, CenteredMeshPosition;

    [Tooltip("Voxel which is associated with this class")] public GameObject VoxelGameObject;
    [Tooltip("Boolean which indicates whether the voxel can emit heat (not colliding with obstacle)")] public bool IsOnObstacle = false;
    private VoxelInfo VoxelInfoComponent;       // Displaying voxel information via component
    private float CurrentTemperature = 20f;     // Current temperature of the voxel
    private float MinTemperature = -10f;        // Minimum temperature threshold for the voxel
    private float MaxTemperature = 60f;         // Maximum temperature threshold for the voxel

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="voxelArrayIndices">Indices for array in voxel mesh.</param>
    /// <param name="originalMeshPosition">Position of voxel in voxel mesh.</param>
    /// <param name="centeredMeshPosition">Centered position of voxel in voxel mesh. </param>
    /// <param name="minTemperatureValue">Minimum threshold for the temperature of the voxel.</param>
    /// <param name="maxTemperatureValue">Maximum threshold for the temperature of the voxel.</param>
    public Voxel(Vector3 voxelArrayIndices, Vector3 originalMeshPosition, Vector3 centeredMeshPosition,
           float minTemperatureValue, float maxTemperatureValue)
    {
        VoxelMeshIndices = voxelArrayIndices;
        InitialMeshPosition = originalMeshPosition;
        CenteredMeshPosition = centeredMeshPosition;
        MinTemperature = minTemperatureValue;
        MaxTemperature = maxTemperatureValue;
    }

    /// <summary>
    /// Utility method for updating the temperature of this voxel from a receiving
    /// heat emission.
    /// </summary>
    /// <param name="temperature">Temperature received from heat emission.</param>
    /// <param name="heatEmissionRate">Magnitude of heat emission influence on voxel.</param>
    public void UpdateTemperatureFromHeatEmission(float temperature, float heatEmissionRate)
    {
        UpdateTemperature((temperature - CurrentTemperature) * heatEmissionRate);
    }

    /// <summary>
    /// Utility method for emitting heat from this voxel to a neighbour voxel.
    /// </summary>
    /// <param name="neighbourVoxel">Neighbouring voxel which is affected by this voxel's heat emission.</param>
    /// <param name="heatEmissionRate">Magnitude of heat emission influence on nehgbour voxel.</param>
    public void EmitHeat(Voxel neighbourVoxel, float heatEmissionRate)
    {
        float heatTransferred = (neighbourVoxel.CurrentTemperature - CurrentTemperature) * heatEmissionRate;

        /*
            Very basic model of heat emission. Heat transferred refers to
            source losing heat energy, and target obtaining heat energy.

            This currently works rather slow, probably want to rework this
            calculation or use another model.
         */

        UpdateTemperature(heatTransferred);
        neighbourVoxel.UpdateTemperature(-heatTransferred);
    }

    /// <summary>
    /// Helper method for updating the temperature of the voxel using a specified temperature.
    /// </summary>
    /// <param name="heatTransferred">Amount of heat transferred from heat emission.</param>
    private void UpdateTemperature(float heatTransferred)
    {
        CurrentTemperature = Mathf.Clamp(CurrentTemperature + heatTransferred, MinTemperature, MaxTemperature); ;
    }

    /// <summary>
    /// Utility method for resetting the temperature of the voxel.
    /// </summary>
    /// <param name="defaultTemperature">Default temperature for resetting the voxel.</param>
    public void ResetTemperature(float defaultTemperature)
    {
        CurrentTemperature = defaultTemperature;
    }

    /// <summary>
    /// Utility method for acquiring the current temperature of this voxel.
    /// </summary>
    /// <returns>The current temperature of this voxel.</returns>
    public float GetTemperature()
    {
        return CurrentTemperature;
    }

    /// <summary>
    /// Utility method for returning a new copy of this voxel.
    /// </summary>
    /// <returns>A new copy of this voxel.</returns>
    public Voxel Copy()
    {
        Voxel voxelCopy = new Voxel(VoxelMeshIndices, InitialMeshPosition, CenteredMeshPosition, MinTemperature, MaxTemperature)
        {
            VoxelGameObject = VoxelGameObject,
            IsOnObstacle = IsOnObstacle
        };
        voxelCopy.ResetTemperature(CurrentTemperature);

        return voxelCopy;
    }

    /// <summary>
    /// Utility method for referencing the properties of a specified voxel.
    /// </summary>
    /// <param name="aVoxel">Voxel object whose properties are to be referenced by this voxel.</param>
    public void ReferenceVoxelProperties(Voxel aVoxel)
    {
        CurrentTemperature = aVoxel.CurrentTemperature;
        IsOnObstacle = aVoxel.IsOnObstacle;
    }

    /// <summary>
    /// Utility method for updating the information in the voxel info component.
    /// </summary>
    public void UpdateVoxelInfoComponent()
    {
        if (VoxelGameObject)
        {
            // Cache voxel info component if not already
            if (!VoxelInfoComponent)
                VoxelInfoComponent = VoxelGameObject.GetComponent<VoxelInfo>();
            VoxelInfoComponent.UpdateVoxelInformation(this);
        }
        else
            Debug.LogError("Voxel game object not yet defined.");
    }

    /// <summary>
    /// Utility method for updating the colour of the voxel.
    /// </summary>
    /// <param name="showVoxelMesh">Boolean indicating whether the voxel mesh is being displayed.</param>
    /// <param name="temperatureColours">Colours available for voxel based on temperature.</param>
    public void UpdateVoxelColour(bool showVoxelMesh, Color[] temperatureColours)
    {
        // Only update colour if voxel mesh is being shown
        if (showVoxelMesh)
        {
            // Black if obstacle, various colours otherwise.
            if (IsOnObstacle)
                VoxelGameObject.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 0, 0, 1f);
            else
                VoxelGameObject.GetComponent<Renderer>().material.color = temperatureColours[GetTemperatureColourIndex()];
        }
    }

    /// <summary>
    /// Helper method for returning an index for the voxel colour array.
    /// </summary>
    /// <returns>Index for voxel colour array.</returns>
    private int GetTemperatureColourIndex()
    {
        /*
            These values are hard-coded. It is advised to look for an alternate
            approach voxel colour selection and thresholds.
        */

        if (CurrentTemperature > 50)
            return 6;
        if (CurrentTemperature <= 50 && CurrentTemperature > 40)
            return 5;
        if (CurrentTemperature <= 40 && CurrentTemperature > 30)
            return 4;
        if (CurrentTemperature <= 30 && CurrentTemperature > 20)
            return 3;
        if (CurrentTemperature <= 20 && CurrentTemperature > 10)
            return 2;
        if (CurrentTemperature <= 10 && CurrentTemperature > 0)
            return 1;

        return 0;
    }

    /// <summary>
    ///  Utility method that updates the collision status of a voxel in a voxel mesh.
    /// </summary>
    /// <param name="obstacleMask">Layer mask for identifying obstacles.</param>
    public void UpdateVoxelCollisionStatus(LayerMask obstacleMask)
    {
        // Compute collisions around voxel
        Collider[] numberOfCollisions = Physics.OverlapBox(CenteredMeshPosition,
            VoxelGameObject.transform.localScale / 2, Quaternion.identity, obstacleMask);

        // If there was a collision, indicate voxel as obstacle
        if (numberOfCollisions.Length > 0)
            IsOnObstacle = true;
    }
}
