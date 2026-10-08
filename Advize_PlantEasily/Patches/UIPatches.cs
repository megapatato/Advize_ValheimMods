namespace Advize_PlantEasily;

using HarmonyLib;
using TMPro;
using UnityEngine;
using static ModContext;

[HarmonyPatch]
static class UIPatches
{
    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.SetupRequirement)), HarmonyPriority(Priority.Last)]
    static void Postfix(Transform elementRoot, Piece.Requirement req)
    {
        if (GhostGrid.Instance.ExtraGhosts.Count < 1 || !config.ShowCost) return;

        TMP_Text component = elementRoot.transform.Find("res_amount").GetComponent<TMP_Text>();
        int totalGhosts = Mathf.Min(config.GridSizeB * config.GridSizeA, config.MaxConcurrentPlacements);

        string formattedCost = config.CostDisplayStyle == 0 ? config.CostDisplayLocation == 0 ?
            $"{totalGhosts}x" : $"x{totalGhosts}" : $"({req.m_amount * totalGhosts})";

        component.text = config.CostDisplayLocation == 0 ? formattedCost + component.text : component.text + formattedCost;
    }
}
