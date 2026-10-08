namespace Advize_PlantEasily;

using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using static ConfigEventHandlers;
using static ModContext;

sealed class ModConfig
{
    private readonly ConfigFile Config;

    //Controls
    private readonly ConfigEntry<KeyboardShortcut> enableModKey;
    private readonly ConfigEntry<KeyboardShortcut> enableSnappingKey;
    private readonly ConfigEntry<KeyboardShortcut> toggleAutoReplantKey;

    private readonly ConfigEntry<KeyboardShortcut> increaseAKey;
    private readonly ConfigEntry<KeyboardShortcut> increaseBKey;
    private readonly ConfigEntry<KeyboardShortcut> decreaseAKey;
    private readonly ConfigEntry<KeyboardShortcut> decreaseBKey;

    private readonly ConfigEntry<KeyboardShortcut> keyboardModifierKey;
    private readonly ConfigEntry<KeyboardShortcut> gamepadModifierKey;
    private readonly ConfigEntry<KeyboardShortcut> keyboardHarvestModifierKey;

    private readonly ConfigEntry<KeyboardShortcut> nextGridKey;
    private readonly ConfigEntry<KeyboardShortcut> prevGridKey;

    //Difficulty
    private readonly ConfigEntry<bool> preventPartialPlanting;
    private readonly ConfigEntry<bool> preventInvalidPlanting;
    private readonly ConfigEntry<bool> useStamina;
    private readonly ConfigEntry<bool> useDurability;

    //General
    private readonly ConfigEntry<bool> modActive;
    private readonly ConfigEntry<bool> snapActive;
    private readonly ConfigEntry<int> gridSizeA;
    private readonly ConfigEntry<int> gridSizeB;
    private readonly ConfigEntry<bool> randomizeRotation;
    private readonly ConfigEntry<bool> enableScatter;
    private readonly ConfigEntry<float> positionScatterRadius;
    private readonly ConfigEntry<float> rotationScatterAngle;
    private readonly ConfigEntry<bool> enableDebugMessages;

    //Grid
    private readonly ConfigEntry<GridType> gridType;
    private readonly ConfigEntry<bool> globallyAlignGridDirections;
    private readonly ConfigEntry<bool> minimizeGridSpacing;
    private readonly ConfigEntry<GridSnappingStyle> gridSnappingStyle;
    private readonly ConfigEntry<bool> forceAltPlacement;
    private readonly ConfigEntry<bool> preferCardinalSnapping;
    private readonly ConfigEntry<float> extraCropSpacing;
    private readonly ConfigEntry<float> extraSaplingSpacing;

    //Harvesting
    private readonly ConfigEntry<bool> enableBulkHarvest;
    private readonly ConfigEntry<HarvestStyle> harvestStyle;
    private readonly ConfigEntry<float> harvestRadius;
    private readonly ConfigEntry<bool> replantOnHarvest;

    //Performance
    private readonly ConfigEntry<int> maxConcurrentPlacements;
    private readonly ConfigEntry<int> ghostUpdateBatchSize;
    private readonly ConfigEntry<int> bulkPlantingBatchSize;

    //Pickables
    private readonly ConfigEntry<bool> preventOverlappingPlacements;
    private readonly ConfigEntry<float> defaultGridSpacing;

    //UI
    private readonly ConfigEntry<bool> showCost;
    private readonly ConfigEntry<CostDisplayStyle> costDisplayStyle;
    private readonly ConfigEntry<CostDisplayLocation> costDisplayLocation;
    private readonly ConfigEntry<bool> showHUDKeyHints;
    private readonly ConfigEntry<bool> showHoverKeyHints;
    private readonly ConfigEntry<bool> showHoverReplantHint;
    private readonly ConfigEntry<bool> showGhostsDuringPlacement;
    private readonly ConfigEntry<bool> showGridDirections;
    private readonly ConfigEntry<bool> showSnapDirection;
    private readonly ConfigEntry<bool> highlightRootPlacementGhost;
    private readonly ConfigEntry<Color> rootGhostHighlightColor;
    private readonly ConfigEntry<Color> rootYStartColor;
    private readonly ConfigEntry<Color> rootYEndColor;
    private readonly ConfigEntry<Color> rootXStartColor;
    private readonly ConfigEntry<Color> rootXEndColor;
    private readonly ConfigEntry<Color> snapStartColor;
    private readonly ConfigEntry<Color> snapEndColor;

    internal ModConfig(ConfigFile configFile)
    {
        Config = configFile;
        configFile.SaveOnConfigSet = false;

        //Controls
        enableModKey = Config.BindInOrder("Controls", "EnableModKey", new KeyboardShortcut(KeyCode.F8),
            "Key to toggle on/off all mod features. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to toggle on/off all mod features."; });
        enableSnappingKey = Config.BindInOrder("Controls", "EnableSnappingKey", new KeyboardShortcut(KeyCode.F10),
            "Key to toggle on/off piece snapping functionality. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to toggle on/off piece snapping functionality."; });
        toggleAutoReplantKey = Config.BindInOrder("Controls", "ToggleAutoReplantKey", new KeyboardShortcut(KeyCode.F6),
            "Key to toggle on/off the [Harvesting]ReplantOnHarvest setting. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to toggle on/off the [Harvesting]ReplantOnHarvest setting."; });
        increaseAKey = Config.BindInOrder("Controls", "IncreaseAKey", new KeyboardShortcut(KeyCode.RightArrow),
            "Key to increase the grid's size via the A parameter. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to increase the grid's size via the A parameter."; });
        increaseBKey = Config.BindInOrder("Controls", "IncreaseBKey", new KeyboardShortcut(KeyCode.UpArrow),
            "Key to increase the grid's size via the B parameter. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to increase the grid's size via the B parameter."; });
        decreaseAKey = Config.BindInOrder("Controls", "DecreaseAKey", new KeyboardShortcut(KeyCode.LeftArrow),
            "Key to decrease the grid's size via the A parameter. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to decrease the grid's size via the A parameter."; });
        decreaseBKey = Config.BindInOrder("Controls", "DecreaseBKey", new KeyboardShortcut(KeyCode.DownArrow),
            "Key to decrease the grid's size via the B parameter. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key to decrease the grid's size via the B parameter."; });
        keyboardModifierKey = Config.BindInOrder("Controls", "KeyboardModifierKey", new KeyboardShortcut(KeyCode.RightControl),
            "Modifier key when using keyboard controls. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Modifier key when using keyboard controls."; });
        gamepadModifierKey = Config.BindInOrder("Controls", "GamepadModifierKey", new KeyboardShortcut(KeyCode.JoystickButton4),
            "Modifier key when using gamepad controls. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Modifier key when using gamepad controls."; });
        keyboardHarvestModifierKey = Config.BindInOrder("Controls", "KeyboardHarvestModifierKey", new KeyboardShortcut(KeyCode.LeftShift),
            "Modifier key to enable bulk harvest when using keyboard controls. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Modifier key to enable bulk harvest when using keyboard controls."; });
        nextGridKey = Config.BindInOrder("Controls", "NextGridKey", new KeyboardShortcut(KeyCode.E, KeyCode.LeftShift),
            "Key to switch to the next grid layout type. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key or combo to switch to the next layout pattern."; });
        prevGridKey = Config.BindInOrder("Controls", "PrevGridKey", new KeyboardShortcut(KeyCode.Q, KeyCode.LeftShift),
            "Key to switch to the previous grid layout type. See https://docs.unity3d.com/ScriptReference/KeyCode.html",
            a => { a.Description = "Key or combo to switch to the previous layout pattern."; });

        //Difficulty
        preventPartialPlanting = Config.BindInOrder("Difficulty", "PreventPartialPlanting", false, "Prevents placement of resources when any placement ghosts are invalid for any reason.");
        preventInvalidPlanting = Config.BindInOrder("Difficulty", "PreventInvalidPlanting", true, "Prevents plants from being placed where they will be unable to grow.");
        useStamina = Config.BindInOrder("Difficulty", "UseStamina", true, "Consume stamina for every piece placed.");
        useDurability = Config.BindInOrder("Difficulty", "UseDurability", true, "Decrease durability of cultivator for every piece placed.");

        //General
        modActive = Config.BindInOrder("General", "ModActive", true, "Enables all mod features.");
        snapActive = Config.BindInOrder("General", "SnapActive", true, "Enables the grid snapping feature.");
        gridSizeA = Config.BindInOrder("General", "GridSizeA", 3, "A parameter for determining grid size; e.g. a rectangle's width.");
        gridSizeB = Config.BindInOrder("General", "GridSizeB", 2, "B parameter for determining grid size; e.g. a rectangle's height.");
        randomizeRotation = Config.BindInOrder("General", "RandomizeRotation", true, "Randomizes rotation of pieces once placed.");
        enableScatter = Config.BindInOrder("General", "EnableScatter", true, "Enables subtle randomization of placement to prevent grids from looking overly uniform. Controls both positional scatter and rotational tilt.");
        positionScatterRadius = Config.BindInOrder("General", "PositionScatterRadius", 0f, "Applies small random offsets to the X/Z position of placed pieces. Keep values low (0.01–0.05) to avoid visible grid distortion, especially in large grids.");
        rotationScatterAngle = Config.BindInOrder("General", "RotationScatterAngle", 0f, "Applies small random X/Z tilt to placed pieces. Recommended values: 1–3 degrees. Excessive tilt may interfere with crop growth or placement clearance.");
        enableDebugMessages = Config.BindInOrder("General", "EnableDebugMessages", false, "Enable mod debug messages in console.");

        //Grid
        gridType = Config.BindInOrder("Grid", "GridType", GridType.Rectangular, "Layout pattern. Rectangular uses squares; it tiles into rectangular row & column grids. Triangular uses triangles; it tiles into row & column grids. Hexagonal grows concentric hexagons from a center, tiles from triangles.");
        globallyAlignGridDirections = Config.BindInOrder("Grid", "GloballyAlignGridDirections", true, "When set to true, new grid placements will have their column and row directions align with the global grid.");
        minimizeGridSpacing = Config.BindInOrder("Grid", "MinimizeGridSpacing", false, "Allows for tighter grids, but with varying spacing used between diverse/distinct plants. ");
        gridSnappingStyle = Config.BindInOrder("Grid", "GridSnappingStyle", GridSnappingStyle.Intelligent, "Determines grid snapping style. Intelligent will attempt to prevent a new grid from overlapping with an old one. Legacy will allow any orientation of new rows and columns.");
        forceAltPlacement = Config.BindInOrder("Grid", "ForceAltPlacement", false, "When enabled, alternate placement mode for the cultivator is always used. Alternate placement mode allows free rotation when snapping to a single piece.");
        preferCardinalSnapping = Config.BindInOrder("Grid", "PreferCardinalSnapping", false, "Prefer cardinal snap points over diagonal snap points when snapping to existing grids.");
        extraCropSpacing = Config.BindInOrder("Grid", "ExtraCropSpacing", 0f, "Adds extra spacing between crops. Accepts negative values to decrease spacing (not recommended).");
        extraSaplingSpacing = Config.BindInOrder("Grid", "ExtraSaplingSpacing", 0f, "Adds extra spacing between saplings. Accepts negative values to decrease spacing (not recommended).");

        //Harvesting
        enableBulkHarvest = Config.BindInOrder("Harvesting", "EnableBulkHarvest", true, "Enables the ability to harvest multiple resources at once.");
        harvestStyle = Config.BindInOrder("Harvesting", "HarvestStyle", HarvestStyle.AllResources, "Determines bulk harvest style. LikeResources only harvests resources of the type you've interacted with. AllResources harvests all eligible resources.");
        harvestRadius = Config.BindInOrder("Harvesting", "HarvestRadius", 3.0f, "Determines radius used to search for resources when bulk harvesting.");
        replantOnHarvest = Config.BindInOrder("Harvesting", "ReplantOnHarvest", false, "Enables automatic replanting of crops when harvested, provided you have the resources.");

        //Performance
        maxConcurrentPlacements = Config.BindInOrder("Performance", "MaxConcurrentPlacements", 500, "Maximum amount of pieces that can be placed at once with the cultivator.", acceptableValues: new AcceptableValueRange<int>(2, 10000));
        ghostUpdateBatchSize = Config.BindInOrder("Performance", "GhostUpdateBatchSize", 20, "This value determines how many placement ghosts can update their positions, rotations, etc. per frame. Reducing this value will improve performance during placement and snapping.", acceptableValues: new AcceptableValueRange<int>(1, 10000));
        bulkPlantingBatchSize = Config.BindInOrder("Performance", "BulkPlantingBatchSize", 2, "This value determines how many concurrent pieces can be placed per frame. Increase to speed up planting. Reduce this value if the game hangs when placing too many pieces at once.", acceptableValues: new AcceptableValueRange<int>(2, 10000));

        //Pickables
        preventOverlappingPlacements = Config.BindInOrder("Pickables", "PreventOverlappingPlacements", true, "Prevents placement of pickable resources on top of colliding obstructions.");
        defaultGridSpacing = Config.BindInOrder("Pickables", "DefaultGridSpacing", 1.0f, "Determines default distance/spacing between pickable resources when planting.");

        //UI
        showCost = Config.BindInOrder("UI", "ShowCost", true, "Update resource cost in build UI.");
        costDisplayStyle = Config.BindInOrder("UI", "CostDisplayStyle", CostDisplayStyle.TotalCount, "Determines display style of the ShowCost setting. TotalCount shows total number of pieces to be placed. FullCost shows combined resoure cost of all pieces.");
        costDisplayLocation = Config.BindInOrder("UI", "CostDisplayLocation", CostDisplayLocation.RightSide, "Determines whether to prepend or append text to the resource cost in build UI. LeftSide or RightSide will prepend or append respectively.");
        showHUDKeyHints = Config.BindInOrder("UI", "ShowHUDKeyHints", true, "Show KeyHints in build HUD.");
        showHoverKeyHints = Config.BindInOrder("UI", "ShowHoverKeyHints", true, "Show KeyHints in hover text.");
        showHoverReplantHint = Config.BindInOrder("UI", "ShowHoverReplantHint", true, "Show crop to be replanted upon harvest in hover text.");
        showGhostsDuringPlacement = Config.BindInOrder("UI", "ShowGhostsDuringPlacement", true, "Show silhouettes of placement ghosts during placement.");
        showGridDirections = Config.BindInOrder("UI", "ShowGridDirections", true, "Render lines indicating direction of rows and columns.");
        showSnapDirection = Config.BindInOrder("UI", "ShowSnapDirection", true, "Render a line from root placement ghost to the position it's snapping from.");
        highlightRootPlacementGhost = Config.BindInOrder("UI", "HighlightRootGhost", true, "Highlight the root placement ghost while bulk planting.");
        rootGhostHighlightColor = Config.BindInOrder("UI", "RootGhostHighlightColor", Color.green, "Highlight color for root placement ghost when [UI]HighlightRootGhost is enabled.");
        rootXStartColor = Config.BindInOrder("UI", "RootXStartColor", Color.green, "Starting color for the X direction when [UI]ShowGridDirections is enabled.  For row & column arrays, this maps to the A parameter.");
        rootXEndColor = Config.BindInOrder("UI", "RootXEndColor", Color.yellow, "Ending color for the X direction when [UI]ShowGridDirections is enabled.");
        rootYStartColor = Config.BindInOrder("UI", "RootYStartColor", Color.blue, "Starting color for the Y direction when [UI]ShowGridDirections is enabled. For row & column arrays, this maps to the B parameter.");
        rootYEndColor = Config.BindInOrder("UI", "RootYEndColor", Color.cyan, "Ending color for the Y direction when [UI]ShowGridDirections is enabled.");
        snapStartColor = Config.BindInOrder("UI", "SnapStartColor", Color.red, "Starting color for snap direction when [UI]ShowGridDirections is enabled.");
        snapEndColor = Config.BindInOrder("UI", "SnapEndColor", Color.magenta, "Ending color for snap direction when [UI]ShowGridDirections is enabled.");

        configFile.Save();
        configFile.SaveOnConfigSet = true;

        gridSizeA.SettingChanged += GridSizeChanged;
        gridSizeB.SettingChanged += GridSizeChanged;
        maxConcurrentPlacements.SettingChanged += GridSizeChanged;
        increaseAKey.SettingChanged += KeybindsChanged;
        increaseBKey.SettingChanged += KeybindsChanged;
        decreaseAKey.SettingChanged += KeybindsChanged;
        decreaseBKey.SettingChanged += KeybindsChanged;
        keyboardModifierKey.SettingChanged += KeybindsChanged;
        gamepadModifierKey.SettingChanged += KeybindsChanged;
        keyboardHarvestModifierKey.SettingChanged += KeybindsChanged;
        nextGridKey.SettingChanged += KeybindsChanged;
        prevGridKey.SettingChanged += KeybindsChanged;
        showGridDirections.SettingChanged += (_, _) =>
        {
            foreach (GhostGrid instance in GhostGrid.Instances.Values)
                instance.DirectionRenderer?.SetActive(false);
        };
        rootXStartColor.SettingChanged += GridColorChanged;
        rootXEndColor.SettingChanged += GridColorChanged;
        rootYStartColor.SettingChanged += GridColorChanged;
        rootYEndColor.SettingChanged += GridColorChanged;
        snapStartColor.SettingChanged += GridColorChanged;
        snapEndColor.SettingChanged += GridColorChanged;
    }

    internal void BindPickableSpacingSettings()
    {
        Config.SaveOnConfigSet = false;

        Dictionary<string, float> predefinedSpacingDefaults = new()
        {
            { "Pickable_Dandelion", 0.75f },
            { "Pickable_Fiddlehead", 1.0f },
            { "Pickable_Mushroom", 0.5f },
            { "Pickable_Mushroom_blue", 0.5f },
            { "Pickable_Mushroom_yellow", 0.5f },
            { "Pickable_SmokePuff", 1.0f },
            { "Pickable_Thistle", 0.75f },
            { "BlueberryBush", 1.5f },
            { "RaspberryBush", 1.5f },
            { "CloudberryBush", 1.0f },
            { "LingonberryBush", 1.5f}
        };

        foreach (PickableDB pdb in PickableRefs)
        {
            float gridSpacing = predefinedSpacingDefaults.TryGetValue(pdb.key, out float spacing) ? spacing : DefaultGridSpacing;
            pdb.itemName = Localization.instance.Localize(pdb.Prefab.GetComponent<Pickable>().m_itemPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
            pdb.configEntry = Config.Bind("Pickables", $"{pdb.key} GridSpacing", gridSpacing, $"Determines distance/spacing between {pdb.itemName} when planting.");
            pdb.configEntry.SettingChanged += GridSpacingChanged;
        }

        predefinedSpacingDefaults.Clear();
        Config.Save();
        Config.SaveOnConfigSet = true;
    }

    //Controls
    internal KeyboardShortcut EnableModKey => enableModKey.Value;
    internal KeyboardShortcut EnableSnappingKey => enableSnappingKey.Value;
    internal KeyboardShortcut ToggleAutoReplantKey => toggleAutoReplantKey.Value;
    internal KeyCode IncreaseXKey => increaseAKey.Value.MainKey;
    internal KeyCode IncreaseYKey => increaseBKey.Value.MainKey;
    internal KeyCode DecreaseXKey => decreaseAKey.Value.MainKey;
    internal KeyCode DecreaseYKey => decreaseBKey.Value.MainKey;
    internal KeyCode KeyboardModifierKey => keyboardModifierKey.Value.MainKey;
    internal KeyCode GamepadModifierKey => gamepadModifierKey.Value.MainKey;
    internal KeyCode KeyboardHarvestModifierKey => keyboardHarvestModifierKey.Value.MainKey;
    internal KeyCode NextGridKey => nextGridKey.Value.MainKey;
    internal KeyCode PrevGridKey => prevGridKey.Value.MainKey;
    //Difficulty
    internal bool PreventPartialPlanting => preventPartialPlanting.Value;
    internal bool PreventInvalidPlanting => preventInvalidPlanting.Value;
    internal bool UseStamina => useStamina.Value;
    internal bool UseDurability => useDurability.Value;
    //General    
    internal bool ModActive
    {
        get { return modActive.Value; }
        set { modActive.BoxedValue = value; }
    }
    internal bool SnapActive
    {
        get { return snapActive.Value; }
        set { snapActive.BoxedValue = value; }
    }
    internal int GridSizeA
    {
        get { return Mathf.Max(gridSizeA.Value, 1); }
        set { gridSizeA.BoxedValue = Mathf.Max(value, 1); }
    }
    internal int GridSizeB
    {
        get { return Mathf.Max(gridSizeB.Value, 1); }
        set { gridSizeB.BoxedValue = Mathf.Max(value, 1); }
    }
    internal bool RandomizeRotation => randomizeRotation.Value;
    internal bool TryGetScatterRadius(out float radius)
    {
        radius = positionScatterRadius.Value;
        return enableScatter.Value && radius != 0f;
    }
    internal bool TryGetScatterAngle(out float angle)
    {
        angle = rotationScatterAngle.Value;
        return enableScatter.Value && angle != 0f;
    }
    internal bool EnableDebugMessages => enableDebugMessages.Value;
    //Grid
    internal GridType GridType => gridType.Value;
    internal void NextGridType()
    {
        GhostGrid.Instance.DestroyExtraGhosts();
        GhostGrid.Instance.DirectionRenderer.SetActive(false);
        switch (config.GridType)
        {
            case GridType.Rectangular:
                gridType.Value = GridType.Triangular;
                break;
            case GridType.Triangular: /*
                gridType.Value = GridType.Hexagonal;
                break;
            case GridType.Hexagonal: */
                gridType.Value = GridType.Rectangular;
                break;
            default:
                gridType.Value = GridType.Rectangular;
                break;
        }
    }
    internal void PrevGridType()
    {
        GhostGrid.Instance.DestroyExtraGhosts();
        GhostGrid.Instance.DirectionRenderer.SetActive(false);
        switch (config.GridType)
        {
            case GridType.Triangular:
                gridType.Value = GridType.Rectangular;
                break;
            case GridType.Rectangular: /*
                gridType.Value = GridType.Hexagonal;
                break;
            case GridType.Hexagonal: */
                gridType.Value = GridType.Triangular;
                break;
            default:
                gridType.Value = GridType.Rectangular;
                break;
        }
    }
    internal bool GloballyAlignGridDirections => globallyAlignGridDirections.Value;
    internal bool MinimizeGridSpacing => minimizeGridSpacing.Value;
    internal GridSnappingStyle GridSnappingStyle => gridSnappingStyle.Value;
    internal bool ForceAltPlacement => forceAltPlacement.Value;
    internal bool PreferCardinalSnapping => preferCardinalSnapping.Value;
    internal float ExtraCropSpacing => extraCropSpacing.Value;
    internal float ExtraSaplingSpacing => extraSaplingSpacing.Value;
    //Harvesting
    internal bool EnableBulkHarvest => enableBulkHarvest.Value;
    internal HarvestStyle HarvestStyle => harvestStyle.Value;
    internal float HarvestRadius => harvestRadius.Value;
    internal bool ReplantOnHarvest
    {
        get { return replantOnHarvest.Value; }
        set { replantOnHarvest.BoxedValue = value; }
    }
    //Performance
    internal int MaxConcurrentPlacements => maxConcurrentPlacements.Value;
    internal int GhostUpdateBatchSize => ghostUpdateBatchSize.Value;
    internal int BulkPlantingBatchSize => bulkPlantingBatchSize.Value;
    //Pickables
    internal bool PreventOverlappingPlacements => preventOverlappingPlacements.Value;
    internal float DefaultGridSpacing => defaultGridSpacing.Value;
    //UI
    internal bool ShowCost => showCost.Value;
    internal CostDisplayStyle CostDisplayStyle => costDisplayStyle.Value;
    internal CostDisplayLocation CostDisplayLocation => costDisplayLocation.Value;
    internal bool ShowHUDKeyHints => showHUDKeyHints.Value;
    internal bool ShowHoverKeyHints => showHoverKeyHints.Value;
    internal bool ShowHoverReplantHint => showHoverReplantHint.Value;
    internal bool ShowGhostsDuringPlacement => showGhostsDuringPlacement.Value;
    internal bool ShowGridDirections => showGridDirections.Value;
    internal bool ShowSnapDirection => showSnapDirection.Value;
    internal bool HighlightRootPlacementGhost => highlightRootPlacementGhost.Value;
    internal Color RootGhostHighlightColor => rootGhostHighlightColor.Value;
    internal Color RowStartColor => rootYStartColor.Value;
    internal Color RowEndColor => rootYEndColor.Value;
    internal Color ColumnStartColor => rootXStartColor.Value;
    internal Color ColumnEndColor => rootXEndColor.Value;
    internal Color SnapStartColor => snapStartColor.Value;
    internal Color SnapEndColor => snapEndColor.Value;
}
