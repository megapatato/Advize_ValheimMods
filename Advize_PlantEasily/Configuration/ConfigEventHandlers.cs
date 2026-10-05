namespace Advize_PlantEasily;

using System;
using static ModContext;

internal static class ConfigEventHandlers
{
    internal static void GridSizeChanged(object sender, EventArgs e) => GhostGrid.Instance.ResizeGrid();

    internal static void KeybindsChanged(object sender, EventArgs e) => KeyHintPatches.UpdateKeyHintText();

    internal static void GridSpacingChanged(object sender, EventArgs e) => PickableDB.InitPickableSpacingConfig();

    internal static void GridColorChanged(object sender, EventArgs e)
    {
        if (!GhostGrid.Instance.DirectionRenderer) return;

        GhostGrid.Instance.LineRenderers[0].startColor = config.RowStartColor;
        GhostGrid.Instance.LineRenderers[0].endColor = config.RowEndColor;
        GhostGrid.Instance.LineRenderers[1].startColor = config.ColumnStartColor;
        GhostGrid.Instance.LineRenderers[1].endColor = config.ColumnEndColor;
        GhostGrid.Instance.LineRenderers[2].startColor = config.SnapStartColor;
        GhostGrid.Instance.LineRenderers[2].endColor = config.SnapEndColor;
    }
}
