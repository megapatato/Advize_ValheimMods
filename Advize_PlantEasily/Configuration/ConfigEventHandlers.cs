namespace Advize_PlantEasily;

using System;
using static ModContext;

internal static class ConfigEventHandlers
{
    internal static void GridSizeChanged(object sender, EventArgs e) => GhostGridRectangular.Instance.ResizeGrid();

    internal static void KeybindsChanged(object sender, EventArgs e) => KeyHintPatches.UpdateKeyHintText();

    internal static void GridSpacingChanged(object sender, EventArgs e) => PickableDB.InitPickableSpacingConfig();

    internal static void GridColorChanged(object sender, EventArgs e)
    {
        if (!GhostGridRectangular.Instance.DirectionRenderer) return;

        GhostGridRectangular.Instance.LineRenderers[0].startColor = config.RowStartColor;
        GhostGridRectangular.Instance.LineRenderers[0].endColor = config.RowEndColor;
        GhostGridRectangular.Instance.LineRenderers[1].startColor = config.ColumnStartColor;
        GhostGridRectangular.Instance.LineRenderers[1].endColor = config.ColumnEndColor;
        GhostGridRectangular.Instance.LineRenderers[2].startColor = config.SnapStartColor;
        GhostGridRectangular.Instance.LineRenderers[2].endColor = config.SnapEndColor;
    }
}
