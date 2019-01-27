using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace NavigationComputer.Patches
{
    [HarmonyPatch(typeof(StarmapRenderer), "SetSelectedSystemRenderer")]
    public static class StarmapRenderer_SetSelectedSystemRenderer_Patch
    {
        public static void Prefix(StarmapSystemRenderer systemRenderer, StarmapSystemRenderer ___currSystem)
        {
            if (systemRenderer != null && systemRenderer != ___currSystem
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                ShiftClickMove.NextSelectIsShiftClick = true;
        }
    }
}
