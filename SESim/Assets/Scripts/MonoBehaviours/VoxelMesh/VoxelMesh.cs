using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class/component which contains the functional logic for describing
/// the voxel mesh.
/// </summary>
public class VoxelMesh : Singleton<VoxelMesh>
{
    [Tooltip("Dimensions of the voxel mesh.")] [SerializeField] private Vector3 VoxelMeshDimensions;
    [Tooltip("Offset constant to compensate for ground (generating voxel mesh above ground).")] [SerializeField] private float VoxelMeshHeightOffset = 0.5f;
    [Tooltip("Prefab used to represent a voxels in the mesh.")] public GameObject VoxelPrefab;
    [Tooltip("Values indicating the number of voxels per axis in the voxel mesh.")] public Vector3 TotalVoxelsPerAxis;
    private Vector3 CenteredOffset;                     // Offset used for centering voxel positions.
    private Vector3 VoxelDimensions;                    // Dimensions of voxel calculated using voxel mesh dimensions and total voxels per axis.

    [Header("Voxel Emission Attributes")]
    [Tooltip("Magnitude of heat emissions between voxels.")] [Range(0f, 1f)] [SerializeField] private float HeatEmissionRate;
    [Tooltip("Heat emission range to surrounding voxels (in # of voxels affected).")] [Range(0, 10)] [SerializeField] private int HeatEmissionRange = 1;
    [Tooltip("Initial temperature for newly created voxels.")] [SerializeField] private float InitialVoxelTemperature = 20f;
    [Tooltip("Minimum temperature threshold for voxels.")] [SerializeField] private float MinVoxelTemperature = -10f;
    [Tooltip("Maximum temperature threshold for voxels.")] [SerializeField] private float MaxVoxelTemperature = 60f;
    [Tooltip("Layer mask for identifying game objects tagged as obstacles.")] [SerializeField] private LayerMask ObstacleCollisionMask;

    [Header("Voxel Mesh Update Attributes")]
    [Tooltip("Number of times the voxel mesh has processed heat emissions.")] [ReadOnly] public int UpdateCounter = 0;
    [Tooltip("Rate at which heat emissions in the voxel mesh are processed (in seconds).")] [SerializeField] private float UpdateRateInSeconds = 1f;
    private PollTimer HeatEmissionPollTimer;            // Poll timer for managing when heat emission processing takes place.

    private Voxel[,,] CurrentVoxelMesh;                 // 3D array containing voxels of the current time step.
    private Voxel[,,] NextIterationVoxelMesh;           // 3D array containing voxels of the following/next time step.
    private Color[] VoxelTemperatureColours;            // Array of colours for describing voxels at different temperature ranges.
    private bool IsVoxelMeshConstructed = false;        // Status of whether the voxel mesh has been constructed.

    [Header("Display Attributes")]
    [Tooltip("Boolean which indicates if the voxel mesh should be visible during runtime.")] public bool ShowVoxelMesh;

    #region Initialisation Methods

    void Start()
    {
        // Register for appropriate events + setup poll timer for emissions
        if (SimulationLayerEventsObserver.Instance)
        {
            InitialiseVoxelMesh();
            HeatEmissionPollTimer = new PollTimer(UpdateRateInSeconds);
            HeatEmissionPollTimer.OnTimerPoll += UpdateVoxelMesh;
            HeatEmissionPollTimer.Play();

            SimulationLayerEventsObserver.Instance.AddListenerToEmitHeatProducerEvent(HeatEmissionFromHeatEmitter);
            SimulationLayerEventsObserver.Instance.AddListenerToNoArgumentEvent(SimulationLayerEventsObserver.KEY_VOXEL_MESH_RESET, Reset);
            SimulationLayerEventsObserver.Instance.AddListenerToToggleVoxelMeshEvent(ToggleVoxelMesh);
        }
        else
            Debug.LogError("Simulation Layer Events Observer is not defined in the scene.");
    }

    private void InitialiseVoxelMesh()
    {
        /*
            Notice that there are two voxel meshes which are managed, namely
            'CurrentVoxelMesh' and 'NextIterationVoxelMesh'.

            CurrentVoxelMesh is the voxel mesh which is used when querying
            for the current temperature of voxels.

            NextIterationVoxelMesh is the voxel mesh which is used for processing
            changes and updating CurrentVoxelMesh with the new changes during updates.

            This enables calculations with voxels to not interrupt queries with the
            voxels from the current voxel mesh. If one voxel mesh was used only, then
            weird artifacts may occur.
        */

        // Initialise voxel mesh, voxel temperature colours, etc.
        VoxelTemperatureColours = GetVoxelTemperatureColourArray();
        CurrentVoxelMesh = CreateVoxelMesh(true);
        NextIterationVoxelMesh = CopyVoxelMesh(CurrentVoxelMesh);
        IsVoxelMeshConstructed = true;

        if (!ShowVoxelMesh)
            ToggleVoxelMeshVisibility(false, CurrentVoxelMesh);

    }

    /// <summary>
    /// Helper method for acquiring an array of preset colours for describing
    /// voxel temperatures.
    /// </summary>
    /// <returns>Colour array for voxel temperatures.</returns>
    private Color[] GetVoxelTemperatureColourArray()
    {
        Color[] voxelTemperatureColours = new Color[7];
        voxelTemperatureColours[6] = new Color(1, 0, 0, 1f);                 // Red
        voxelTemperatureColours[5] = new Color(1, 0.65f, 0, 0.2f);           // Orange
        voxelTemperatureColours[4] = new Color(1, 1, 0, 0.2f);               // Yellow
        voxelTemperatureColours[3] = new Color(0.41f, 1, 0.27f, 0.2f);       // Green
        voxelTemperatureColours[2] = new Color(1, 1, 1, 0.2f);               // White
        voxelTemperatureColours[1] = new Color(0.58f, 0.89f, 1, 0.2f);       // Teal
        voxelTemperatureColours[0] = new Color(0, 0.38f, 1, 0.2f);           // Blueish

        return voxelTemperatureColours;
    }

    /// <summary>
    /// Primary helper method for creating and returning a voxel mesh.
    /// </summary>
    /// <param name="createGameObjects">Boolean which indicates if game objects should be created for the voxels.</param>
    /// <returns></returns>
    private Voxel[,,] CreateVoxelMesh(bool createGameObjects)
    {
        /*
            Note:
            This method is huge. Could break it down further, but the method is already
            O(n^3) due to the use of 3D voxels. Thus the voxels were created and acted
            upon in one passing to minimise unnecessary iteration through the voxel mesh.
            
            Several calculations:
            - Voxel Dimensions:     the dimensions of the individual voxels of the voxel mesh.
                                    these are calculated based on the voxel mesh dimensions and
                                    the number of voxels per axis.
            - Height Offset:        the offset of the voxel mesh from the ground. This is a vector3
                                    to make it easier for vector calculations.
            - Centered Offset:      the offset to correct the positions of voxels generated from indices.
                                    Without this offset, the voxels would be generated from the bottom left.
                                    Use the debug gizmos to visualise this effect.
            - Temperature Vector:   No calculation. Just includes the various thresholds and initial temperatures.
                                    x = minimum, y = maximum, z = initial.
        */

        VoxelDimensions = new Vector3(VoxelMeshDimensions.x / TotalVoxelsPerAxis.x,
            VoxelMeshDimensions.y / TotalVoxelsPerAxis.y, VoxelMeshDimensions.z / TotalVoxelsPerAxis.z);
        Vector3 heightOffset = new Vector3(0, (VoxelDimensions.y / 2) + VoxelMeshHeightOffset, 0);
        CenteredOffset = new Vector3((-VoxelMeshDimensions.x + VoxelDimensions.x) / 2, 0,
            (-VoxelMeshDimensions.z + VoxelDimensions.z) / 2);
        Vector3 temperatureVector = new Vector3(MinVoxelTemperature, MaxVoxelTemperature, InitialVoxelTemperature);

        // Parent game object to act as a container for the voxels
        GameObject parentGameObject = new GameObject("Voxels");
        parentGameObject.transform.parent = transform;

        // Jagged array to contain all the voxels
        Voxel[,,] voxels = new Voxel[(int)TotalVoxelsPerAxis.x, (int)TotalVoxelsPerAxis.y, (int)TotalVoxelsPerAxis.z];

        // Generate positions for each voxel, create voxel objects
        for (int x = 0; x < TotalVoxelsPerAxis.x; x++)
        {
            for (int y = 0; y < TotalVoxelsPerAxis.y; y++)
            {
                for (int z = 0; z < TotalVoxelsPerAxis.z; z++)
                {
                    // Calculate voxel position, create voxel using the defined positions, reset to initial temperature...
                    Vector3 originalMeshPosition = new Vector3(x * VoxelDimensions.x, y * VoxelDimensions.y, z * VoxelDimensions.z) + heightOffset;
                    voxels[x, y, z] = new Voxel(new Vector3(x, y, z), originalMeshPosition, originalMeshPosition + CenteredOffset,
                       temperatureVector.x, temperatureVector.y);
                    voxels[x, y, z].ResetTemperature(temperatureVector.z);

                    // Create game objects if specified (ignored for debug purposes if specified)
                    if (createGameObjects)
                    {
                        // Create voxel game object, resize, and reference...
                        GameObject newVoxelGameObject = Object.Instantiate(VoxelPrefab, voxels[x, y, z].CenteredMeshPosition, Quaternion.identity, parentGameObject.transform);
                        newVoxelGameObject.transform.localScale = VoxelDimensions;
                        voxels[x, y, z].VoxelGameObject = newVoxelGameObject;

                        // Update voxel collision status, colour, and information component...
                        voxels[x, y, z].UpdateVoxelCollisionStatus(ObstacleCollisionMask);
                        voxels[x, y, z].UpdateVoxelColour(ShowVoxelMesh, VoxelTemperatureColours);
                        voxels[x, y, z].UpdateVoxelInfoComponent();
                    }
                }
            }
        }

        return voxels;
    }

    /// <summary>
    /// Method that reproduces a copy of a specified 3D voxel array.
    /// </summary>
    /// <param name="anotherVoxelArray">3D voxel array to be copied</param>
    /// <returns></returns>
    private Voxel[,,] CopyVoxelMesh(Voxel[,,] voxelMesh)
    {
        Voxel[,,] copiedVoxelArray = new Voxel[voxelMesh.GetLength(0), voxelMesh.GetLength(1), voxelMesh.GetLength(2)];

        for (int x = 0; x < voxelMesh.GetLength(0); x++)
            for (int y = 0; y < voxelMesh.GetLength(1); y++)
                for (int z = 0; z < voxelMesh.GetLength(2); z++)
                    copiedVoxelArray[x, y, z] = voxelMesh[x, y, z].Copy();

        return copiedVoxelArray;
    }

    /// <summary>
    /// Helper method for toggling the visibility of game objects from voxels in a voxel mesh.
    /// </summary>
    /// <param name="showVoxels">Boolean indicating if the voxels from voxel mesh should be visible.</param>
    /// <param name="voxelMesh">Voxels from voxel mesh to have their visibility toggled.</param>
    private void ToggleVoxelMeshVisibility(bool showVoxels, Voxel[,,] voxelMesh)
    {
        for (int x = 0; x < voxelMesh.GetLength(0); x++)
            for (int z = 0; z < voxelMesh.GetLength(1); z++)
                for (int y = 0; y < voxelMesh.GetLength(2); y++)
                    if (showVoxels)
                        voxelMesh[x, z, y].VoxelGameObject.transform.localScale = new Vector3(1, 1, 1);
                    else
                        voxelMesh[x, z, y].VoxelGameObject.transform.localScale = new Vector3(0, 0, 0);
    }

    #endregion

    #region Voxel Update Methods

    void Update()
    {
        if (HeatEmissionPollTimer != null)
            HeatEmissionPollTimer.Update();
    }

    /// <summary>
    /// Utility callback for processing heat emissions and updating these
    /// changes to the current voxel mesh.
    /// </summary>
    public void UpdateVoxelMesh()
    {
        for (int x = 0; x < NextIterationVoxelMesh.GetLength(0); x++)
        {
            for (int y = 0; y < NextIterationVoxelMesh.GetLength(1); y++)
            {
                for (int z = 0; z < NextIterationVoxelMesh.GetLength(2); z++)
                {
                    // Emit heat only from non-obstacle voxels
                    if (!NextIterationVoxelMesh[x, y, z].IsOnObstacle)
                    {
                        List<Voxel> neighbourVoxels = FindNeighbourVoxels(CurrentVoxelMesh, CurrentVoxelMesh[x, y, z], HeatEmissionRange);

                        // Update temperature of non-obstacle neighbouring voxels
                        foreach (Voxel neighbourVoxel in neighbourVoxels)
                        {
                            if (!neighbourVoxel.IsOnObstacle)
                                NextIterationVoxelMesh[x, y, z].EmitHeat(neighbourVoxel, HeatEmissionRate);
                        }

                        // Update voxel information and colour
                        NextIterationVoxelMesh[x, y, z].UpdateVoxelInfoComponent();
                        CurrentVoxelMesh[x, y, z].UpdateVoxelColour(ShowVoxelMesh, VoxelTemperatureColours);
                    }
                }
            }
        }

        // Lastly, update all changes
        UpdateChangesFromVoxelMesh(CurrentVoxelMesh, NextIterationVoxelMesh);
        UpdateCounter++;
    }

    /// <summary>
    /// Callback method which resets the temperature/information of the
    /// voxels in the voxel mesh.
    /// </summary>
    public void Reset()
    {
        UpdateCounter = 0;

        for (int x = 0; x < NextIterationVoxelMesh.GetLength(0); x++)
        {
            for (int y = 0; y < NextIterationVoxelMesh.GetLength(1); y++)
            {
                for (int z = 0; z < NextIterationVoxelMesh.GetLength(2); z++)
                {
                    NextIterationVoxelMesh[x, y, z].ResetTemperature(InitialVoxelTemperature);
                    NextIterationVoxelMesh[x, y, z].UpdateVoxelInfoComponent();
                }
            }
        }

        HeatEmissionPollTimer.Reset();
    }

    /// <summary>
    /// Callback method for enabling/disabling the voxel mesh heat emissions.
    /// </summary>
    /// <param name="enableVoxelMesh">Boolean indicating whether the voxel mesh should be enabled.</param>
    public void ToggleVoxelMesh(bool enableVoxelMesh)
    {
        if (HeatEmissionPollTimer != null)
        {
            if (enableVoxelMesh)
                HeatEmissionPollTimer.Play();
            else
                HeatEmissionPollTimer.Pause();
        }
    }

    /// <summary>
    /// Helper method for referencing all the changes from voxel mesh B onto voxel mesh A.
    /// </summary>
    /// <param name="voxelMeshA">Voxel mesh which acquires new changes.</param>
    /// <param name="voxelMeshB">Voxel mesh from which changes are referenced.</param>
    private void UpdateChangesFromVoxelMesh(Voxel[,,] voxelMeshA, Voxel[,,] voxelMeshB)
    {
        for (int x = 0; x < voxelMeshA.GetLength(0); x++)
            for (int y = 0; y < voxelMeshA.GetLength(1); y++)
                for (int z = 0; z < voxelMeshA.GetLength(2); z++)
                    voxelMeshA[x, y, z].ReferenceVoxelProperties(voxelMeshB[x, y, z]);
    }

    /// <summary>
    /// Callback from heat emitters. Processes a heat emission from a heat emitter game object.
    /// </summary>
    /// <param name="heatEmitterPosition">Position of the heat emitter.</param>
    /// <param name="neighbourRange">Range of neighbours affected by heat emission.</param>
    /// <param name="temperatureValue">Temperature of the heat emission.</param>
    public void HeatEmissionFromHeatEmitter(Vector3 heatEmitterPosition, int neighbourRange, float temperatureValue)
    {
        if (IsVoxelMeshConstructed)
        {
            Voxel nearestVoxelToProducer;
            if (TryFindVoxelAtSpecifiedPosition(heatEmitterPosition, out nearestVoxelToProducer))
            {
                // Get neighbours around voxel + add itself (since it will also be affected)
                List<Voxel> affectedVoxels = FindNeighbourVoxels(CurrentVoxelMesh, nearestVoxelToProducer, neighbourRange);
                affectedVoxels.Add(nearestVoxelToProducer);

                // Iterate through the voxels and update temperature for each of them
                foreach (Voxel aVoxel in affectedVoxels)
                {
                    NextIterationVoxelMesh[(int)aVoxel.VoxelMeshIndices.x, (int)aVoxel.VoxelMeshIndices.y, (int)aVoxel.VoxelMeshIndices.z].
                        UpdateTemperatureFromHeatEmission(temperatureValue, HeatEmissionRate);
                }
            }
        }
    }

    /// <summary>
    /// Helper method for acquiring neighbouring voxels around a specific voxel.
    /// </summary>
    /// <param name="voxels">Voxels from a voxel mesh to be searched.</param>
    /// <param name="aVoxel">Center voxel which is used to find neighbouring voxels.</param>
    /// <param name="range">Range of surrounding voxels for identifying neighbour voxels.</param>
    /// <returns></returns>
    private List<Voxel> FindNeighbourVoxels(Voxel[,,] voxels, Voxel aVoxel, int range)
    {
        List<Voxel> neighbourVoxels = new List<Voxel>();

        for (int x = -range; x <= range; x++)
        {
            int neighbourX = x + (int)aVoxel.VoxelMeshIndices.x;
            if (neighbourX < 0 || neighbourX >= voxels.GetLength(0))
                continue;

            for (int y = -range; y <= range; y++)
            {
                int neighbourY = y + (int)(aVoxel.VoxelMeshIndices.y);
                if (neighbourY < 0 || neighbourY >= voxels.GetLength(1))
                    continue;

                for (int z = -range; z <= range; z++)
                {
                    // Ignore base case (which is itself)
                    if (x == 0 && z == 0 && y == 0)
                        continue;

                    int neighbourZ = z + (int)(aVoxel.VoxelMeshIndices.z);
                    if (neighbourZ < 0 || neighbourZ >= voxels.GetLength(2))
                        continue;

                    neighbourVoxels.Add(voxels[neighbourX, neighbourY, neighbourZ]);
                }
            }
        }

        return neighbourVoxels;
    }

    /// <summary>
    /// Utility method for finding the nearest voxel at a specified position.
    /// </summary>
    /// <param name="position">Position for finding the nearest voxel.</param>
    /// <param name="aVoxel">Referenced voxel from query.</param>
    /// <returns></returns>
    public bool TryFindVoxelAtSpecifiedPosition(Vector3 position, out Voxel aVoxel)
    {
        int xIndex, yIndex, zIndex;
        ScalePositionToVoxelArrayIndexes(position - CenteredOffset, out xIndex, out yIndex, out zIndex);

        if (AreIndicesValidInVoxelMesh(xIndex, yIndex, zIndex))
        {
            aVoxel = CurrentVoxelMesh[xIndex, yIndex, zIndex];
            return true;
        }
        else
        {
            aVoxel = null;
            return false;
        }
    }

    /// <summary>
    /// Helper method for scaling a position to voxel mesh indices.
    /// </summary>
    /// <param name="position">Position to be scaled.</param>
    /// <param name="xIndex">X coordinate of index from scaling position.</param>
    /// <param name="yIndex">Y coordinate of index from scaling position.</param>
    /// <param name="zIndex">Z coordinate of index from scaling position.</param>
    private void ScalePositionToVoxelArrayIndexes(Vector3 position, out int xIndex, out int yIndex, out int zIndex)
    {
        xIndex = Mathf.RoundToInt((position.x) / VoxelDimensions.x);
        yIndex = Mathf.RoundToInt(((position.y) / VoxelDimensions.y));
        zIndex = Mathf.RoundToInt((position.z) / VoxelDimensions.z);
    }

    /// <summary>
    /// Helper method which validates indices for a voxel mesh.
    /// </summary>
    /// <param name="xIndex">X coordinate of index for voxel mesh.</param>
    /// <param name="yIndex">Y coordinate of index for voxel mesh.</param>
    /// <param name="zIndex">Z coordinate of index for voxel mesh.</param>
    /// <returns></returns>
    private bool AreIndicesValidInVoxelMesh(int xIndex, int yIndex, int zIndex)
    {
        if (xIndex < 0 || xIndex >= TotalVoxelsPerAxis.x ||
            yIndex < 0 || yIndex >= TotalVoxelsPerAxis.y ||
            zIndex < 0 || zIndex >= TotalVoxelsPerAxis.z)
            return false;
        else
            return true;
    }

    #endregion

    #region Debug Gizmo Methods

    [Header("Debug Attributes")]
    [Tooltip("Boolean which indicates whether debug gizmos are illustrated.")] [SerializeField] private bool ShowDebug = false;
    [Tooltip("Boolean which indicates whether the out mesh gizmo is illustrated.")] [SerializeField] private bool ShowOuterVoxelMesh = false;
    [Tooltip("Boolean which indicates whether the voxel mesh at its original position is illustrated.")] [SerializeField] private bool ShowVoxelMeshOriginalPosition = false;
    [Tooltip("Boolean which indicates whether the voxel mesh at its corrected (centered) position is illustrated.")] [SerializeField] private bool ShowVoxelMeshCorrectedPosition = false;

    void OnDrawGizmos()
    {
        if (VoxelMeshDimensions.x != 0 && VoxelMeshDimensions.y != 0 && VoxelMeshDimensions.z != 0)
        {
            if (ShowDebug)
            {
                if (ShowVoxelMeshOriginalPosition)
                    DrawGizmosVoxelMeshOriginalPosition();
                if (ShowVoxelMeshCorrectedPosition)
                    DrawGizmosVoxelMeshCorrectedPosition();
                if (ShowOuterVoxelMesh)
                    DrawGizmosOuterVoxelMesh();
            }
        }
    }

    /// <summary>
    /// Debug helper method for drawing gizmos of the voxel mesh at its original position.
    /// </summary>
    private void DrawGizmosVoxelMeshOriginalPosition()
    {
        Voxel[,,] voxel3DArray = CreateVoxelMesh(false);

        Gizmos.color = Color.yellow;
        for (int x = 0; x < voxel3DArray.GetLength(0); x++)
            for (int y = 0; y < voxel3DArray.GetLength(1); y++)
                for (int z = 0; z < voxel3DArray.GetLength(2); z++)
                    Gizmos.DrawWireCube(voxel3DArray[x, y, z].InitialMeshPosition, VoxelDimensions);
    }

    /// <summary>
    /// Debug helper method for drawing gizmos of the voxel mesh at its corrected position.
    /// </summary>
    private void DrawGizmosVoxelMeshCorrectedPosition()
    {
        Voxel[,,] voxel3DArray = CreateVoxelMesh(false);
        Gizmos.color = Color.green;
        for (int x = 0; x < voxel3DArray.GetLength(0); x++)
            for (int y = 0; y < voxel3DArray.GetLength(1); y++)
                for (int z = 0; z < voxel3DArray.GetLength(2); z++)
                    Gizmos.DrawWireCube(voxel3DArray[x, y, z].CenteredMeshPosition, VoxelDimensions);
    }

    /// <summary>
    /// Debug helper method for drawing gizmos for the outer voxel mesh.
    /// </summary>
    private void DrawGizmosOuterVoxelMesh()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.up * ((VoxelMeshDimensions.y / 2) + VoxelMeshHeightOffset), VoxelMeshDimensions);
    }

    #endregion
}
