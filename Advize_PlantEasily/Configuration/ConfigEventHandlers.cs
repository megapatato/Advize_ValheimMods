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
        foreach (GhostGrid instance in GhostGrid.Instances.Values)
        {
            if (!instance.DirectionRenderer) continue;

            instance.LineRenderers[0].startColor = config.RowStartColor;
            instance.LineRenderers[0].endColor = config.RowEndColor;
            instance.LineRenderers[1].startColor = config.ColumnStartColor;
            instance.LineRenderers[1].endColor = config.ColumnEndColor;
            instance.LineRenderers[2].startColor = config.SnapStartColor;
            instance.LineRenderers[2].endColor = config.SnapEndColor;
        }
    }
}
