namespace Advize_PlantEasily;

using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static ModContext;
using static PlacementState;

internal abstract class GhostGrid
{
    internal readonly List<GameObject> ExtraGhosts = [];
    internal readonly List<GameObject> ValidExtraGhosts = [];
    internal readonly List<Status> GhostPlacementStatus = [];

    internal GameObject DirectionRenderer;
    internal List<LineRenderer> LineRenderers = [];

    protected readonly int ghostLayer = LayerMask.NameToLayer("ghost");

    protected int _gridVersion;
    protected int _ghostUpdateIndex;

    protected Vector3 _lastBasePosition;
    protected Quaternion _lastBaseRotation;
    protected Quaternion _lastEffectiveRotation;

    protected string _lastPieceName;
    protected bool _preservePool;

    public static Dictionary<GridType, GhostGrid> Instances;
    static GhostGrid()
    {
        Instances = new Dictionary<GridType, GhostGrid>()
        {
            [GridType.Rectangular] = new GhostGridRectangular(),
            [GridType.Triangular] = new GhostGridTriangular()
        };
    }

    internal abstract int MaxActiveGhosts { get; }
    protected abstract void InitializeGhosts(GameObject rootGhost);
    internal abstract void Update(Player player);

    internal void ResizeGrid()
    {
        _preservePool = true;
        // Not the cleanest, but this next part triggers UpdatePlacementGhost to re-invoke SetupPlacementGhost which calls PrepareGhostPool followed by BuildGrid()
        GhostPlacementStatus.Clear();
    }

    internal void BuildGrid(GameObject rootGhost)
    {
        //Dbgl("BuildGrid");
        GrowPoolIfNeeded(rootGhost);

        GhostPlacementStatus.Clear();

        InitializeGhosts(rootGhost);

        DeactivateExcessGhosts();
    }

    internal void PrepareGhostPool(GameObject currentPlacementGhost)
    {
        //Dbgl("PrepareGhostPool");
        DirectionRenderer?.SetActive(false);

        DetectPieceChange(currentPlacementGhost);

        //Dbgl($"InPlaceMode? {Player.m_localPlayer?.InPlaceMode()}");

        if (ShouldPreservePool())
        {
            //Dbgl("Preserving Ghosts: Not destroying and clearing extraGhosts");
            return;
        }

        DestroyExtraGhosts();
    }
    
    protected void GrowPoolIfNeeded(GameObject rootGhost)
    {
        //Dbgl("GrowPoolIfNeeded");
        int poolSize = ExtraGhosts.Count;
        string rootName = rootGhost.name;

        while (poolSize < MaxActiveGhosts && poolSize < config.MaxConcurrentPlacements - 1)
        {
            ZNetView.m_forceDisableInit = true;
            GameObject newGhost = UnityEngine.Object.Instantiate(rootGhost);
            newGhost.AddComponent<GhostCache>().Init();
            ZNetView.m_forceDisableInit = false;

            newGhost.name = rootName;

            foreach (Transform t in newGhost.GetComponentsInChildren<Transform>())
                t.gameObject.layer = ghostLayer;

            ExtraGhosts.Add(newGhost);
            poolSize++;
        }
    }

    protected void DeactivateExcessGhosts()
    {
        //Dbgl("DeactivateExcessGhosts");
        for (int i = MaxActiveGhosts; i < ExtraGhosts.Count; i++)
            ExtraGhosts[i].SetActive(false);
    }

    protected void DetectPieceChange(GameObject currentPlacementGhost)
    {
        if (!currentPlacementGhost)
            return;

        string currentPieceName = currentPlacementGhost.name;

        // First time init
        if (string.IsNullOrEmpty(_lastPieceName))
        {
            _lastPieceName = currentPieceName;
            return;
        }

        if (currentPieceName == _lastPieceName)
        {
            //Dbgl($"No piece change: {_lastPieceName} : {currentPieceName}");
            _preservePool = true;
            return;
        }

        //Dbgl($"Detected piece change: from {_lastPieceName} to {currentPieceName}");
        _lastPieceName = currentPieceName;
    }

    protected bool ShouldPreservePool()
    {
        if (!_preservePool || !config.ModActive)
            return false;

        _preservePool = false;
        return true;
    }

    internal void DestroyExtraGhosts()
    {
        foreach (GameObject ghost in ExtraGhosts)
            UnityEngine.Object.Destroy(ghost);

        ExtraGhosts.Clear();
    }

    protected void UpdatePieceCost(Piece piece, int ghostIndex, int baseCost)
    {
        piece.m_resources[0].m_amount = baseCost * (ghostIndex + 1);
    }

    protected void UpdateVisibility()
    {
        bool active = PlacementGhost && PlacementGhost.activeSelf;

        for (int i = 0; i < ExtraGhosts.Count; i++)
        {
            bool shouldBeActive = active && i < MaxActiveGhosts;

            if (ExtraGhosts[i].activeSelf != shouldBeActive)
                ExtraGhosts[i].SetActive(shouldBeActive);
        }
    }

    protected GameObject GetGhostObject(int index)
    {
        return index == 0 ? PlacementGhost : ExtraGhosts[index - 1];
    }

    protected GhostCache GetGhostCache(GameObject ghost)
    {
        return ghost == PlacementGhost ? null : ghost.GetComponent<GhostCache>();
    }

    protected void UpdateGhostTransform(GameObject ghost, Vector3 position)
    {
        ghost.transform.position = position;
        ghost.transform.rotation = BaseRotation;
    }

    protected void UpdateGhostStatus(Player player, Piece piece, GameObject ghost, int index)
    {
        Status baseStatus =
            !player.m_noPlacementCost && !player.HaveRequirements(piece, Player.RequirementMode.CanBuild)
                ? Status.LackResources
                : Status.Healthy;

        Status finalStatus = GhostStatus.EvaluateStatus(ghost, baseStatus);

        ghost.GetComponent<Piece>().SetInvalidPlacementHeightlight(finalStatus != Status.Healthy);

        GhostPlacementStatus[index] = finalStatus;

        if (index == 0 && finalStatus == Status.Healthy)
        {
            if (config.HighlightRootPlacementGhost && GhostPlacementStatus.Count > 1)
            {
                MaterialMan.instance.SetValue(ghost, ShaderProps._Color, config.RootGhostHighlightColor);
                MaterialMan.instance.SetValue(ghost, ShaderProps._EmissionColor, config.RootGhostHighlightColor * 0.7f);
            }

            player.m_placementStatus = 0;
        }
    }

    internal static GhostGrid Instance => Instances[config.GridType];
}

/// <summary>
/// Class <c>GhostArray</c> represents a layout of rows & columns; all rows have the same number of ghosts, as do all columns. It is meant to be derived into specialized child classes, notably square & triangular grids.
/// </summary>
internal abstract class GhostArray : GhostGrid
{
    internal override int MaxActiveGhosts => Mathf.Min(config.GridSizeB * config.GridSizeA - 1, config.MaxConcurrentPlacements - 1);
    protected int TotalCells => 1 + MaxActiveGhosts;
    protected int ActualRows => (TotalCells + config.GridSizeA - 1) / config.GridSizeA;
    protected int ActualColumns => Mathf.Min(config.GridSizeA, TotalCells);

    private static int _lastRows;
    private static int _lastColumns;

    protected abstract void ShowGridDirections();
    protected abstract Vector3 GetGhostPosition(int row, int column, int index);

    protected override void InitializeGhosts(GameObject rootGhost)
    {
        Transform rootTransform = rootGhost.transform;
        int index = 0;

        for (int row = 0; row < config.GridSizeB; row++)
        {
            for (int column = 0; column < config.GridSizeA; column++)
            {
                if (row == 0 && column == 0)
                {
                    SetReferences(rootGhost);

                    GhostPlacementStatus.Add(Status.Healthy);
                    continue;
                }

                if (index >= MaxActiveGhosts)
                    return;

                GameObject ghost = ExtraGhosts[index++];
                ghost.SetActive(true);

                Transform t = ghost.transform;
                t.position = rootTransform.position;
                t.localScale = rootTransform.localScale;

                GhostPlacementStatus.Add(Status.Healthy);
            }
        }
    }

    internal override void Update(Player player)
    {
        UpdateVisibility();

        if (config.ShowGridDirections)
            ShowGridDirections();

        if (!PlacementGhost)
            return;

        Piece piece = Piece;
        int baseCost = piece.m_resources[0].m_amount;

        UpdateGridVersion();

        // Always update root ghost immediately, even though this results in the occasional double update per frame
        UpdateGhost(player, piece, 0, 0, 0);

        int totalGhosts = 1 + MaxActiveGhosts; // Root + extras
        int updatesThisFrame = Mathf.Min(config.GhostUpdateBatchSize, totalGhosts);

        for (int i = 0; i < updatesThisFrame; i++)
        {
            int ghostIndex = _ghostUpdateIndex % totalGhosts;
            _ghostUpdateIndex++;

            int row = ghostIndex / config.GridSizeA;
            int column = ghostIndex % config.GridSizeA;

            if (row >= config.GridSizeB)
                continue;

            UpdatePieceCost(piece, ghostIndex, baseCost);
            UpdateGhost(player, piece, row, column, ghostIndex);
        }

        UpdatePieceCost(piece, 0, baseCost);
    }

    protected void UpdateGridVersion()
    {
        bool changed = false;

        if ((BasePosition - _lastBasePosition).sqrMagnitude > 0.0001f)
        {
            _lastBasePosition = BasePosition;
            changed = true;
        }

        if (Quaternion.Angle(BaseRotation, _lastBaseRotation) > 1f)
        {
            _lastBaseRotation = BaseRotation;
            changed = true;
        }

        Quaternion effectiveRotation = SavedBaseRotation ?? BaseRotation;
        if (Quaternion.Angle(SavedBaseRotation ?? BaseRotation, _lastEffectiveRotation) > 0.1f)
        {
            _lastEffectiveRotation = effectiveRotation;
            changed = true;
        }

        if (config.GridSizeB != _lastRows || config.GridSizeA != _lastColumns)
        {
            _lastRows = config.GridSizeB;
            _lastColumns = config.GridSizeA;
            changed = true;
        }

        if (changed)
            _gridVersion++;
    }

    protected void UpdateGhost(Player player, Piece piece, int row, int column, int index)
    {
        GameObject ghost = GetGhostObject(index);
        GhostCache cache = GetGhostCache(ghost);

        bool hasCache = cache is not null;

        if (hasCache && cache.lastUpdatedVersion == _gridVersion)
            return;

        UpdateGhostTransform(ghost, GetGhostPosition(row, column, index));
        UpdateGhostStatus(player, piece, ghost, index);

        if (hasCache)
            cache.lastUpdatedVersion = _gridVersion;
    }
}


/// <summary>
/// Class <c>GhostGridRectangular</c> tiles its ghosts with squares, generating a rectangular grid.
/// </summary>
internal sealed class GhostGridRectangular : GhostArray
{
    public GhostGridRectangular() { }

    protected override void ShowGridDirections()
    {
        DirectionRenderer.SetActive(PlacementGhost.activeSelf);
        Vector3 vertex = BasePosition + Vector3.up * 0.5f;
        LineRenderers[0].SetPositions([vertex, vertex + (RowDirection * (ActualRows - 1))]);
        LineRenderers[1].SetPositions([vertex, vertex + (ColumnDirection * (ActualColumns - 1))]);
        // Debug purposes, show direction to snap origin
        LineRenderers[2].gameObject.SetActive(config.ShowSnapDirection);
        LineRenderers[2].SetPositions([vertex, vertex + SnapDirection * RowDirection.magnitude]);
    }

    protected override Vector3 GetGhostPosition(int row, int column, int index)
    {
        Vector3 pos = index == 0 ? BasePosition : BasePosition + RowDirection * row + ColumnDirection * column;

        Heightmap.GetHeight(pos, out float height);
        pos.y = height;

        return pos;
    }
}


/// <summary>
/// Class <c>GhostGridTriangular</c> tiles its ghosts with triangles, generating a semi-rectangular grid with 2 opposite ragged edges along the column axis.
/// </summary>
internal sealed class GhostGridTriangular : GhostArray
{
    public GhostGridTriangular() { }

    protected override void ShowGridDirections()
    {
        DirectionRenderer.SetActive(PlacementGhost.activeSelf);
        Vector3 vertex = BasePosition + Vector3.up * 0.5f;
        LineRenderers[0].SetPositions([vertex, vertex + (RowDirection * Mathf.Sqrt(0.75f) * (ActualRows - 1))]);
        LineRenderers[1].SetPositions([vertex, vertex + (ColumnDirection * (ActualColumns - 0.5f))]);
        // Debug purposes, show direction to snap origin
        LineRenderers[2].gameObject.SetActive(config.ShowSnapDirection);
        LineRenderers[2].SetPositions([vertex, vertex + SnapDirection * RowDirection.magnitude]);
    }

    protected override Vector3 GetGhostPosition(int row, int column, int index)
    {
        Vector3 row_delta = new(0, 0, 0);
        Vector3 col_delta = new(0, 0, 0);
        if (index != 0) {
            row_delta = RowDirection * Mathf.Sqrt(0.75f) * row;   // height of unit triangle = sqrt(1^2 - 0.5^2)
            if (row % 2 == 0)
            {
                col_delta = ColumnDirection * column;
            }
            else
            {
                col_delta = ColumnDirection * (column + 0.5f);
            }
        }
        Vector3 pos = BasePosition + row_delta + col_delta;
        Heightmap.GetHeight(pos, out float height);
        pos.y = height;

        return pos;
    }
}


enum GridType : int {
    Rectangular = 0,
    Triangular = 1,
    // Hexagonal = 11,
}
