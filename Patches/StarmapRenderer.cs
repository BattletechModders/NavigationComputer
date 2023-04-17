using BattleTech;
using NavigationComputer.Features;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace NavigationComputer.Patches
{
    [HarmonyPatch(typeof(StarmapRenderer), "SetSelectedSystemRenderer")]
    public static class StarmapRenderer_SetSelectedSystemRenderer_Patch
    {
        public static void Prefix(ref bool __runOriginal, StarmapRenderer __instance, StarmapSystemRenderer systemRenderer)
        {
            if (!__runOriginal) return;
            if (systemRenderer == null && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                Main.HBSLog.Log("Skipping SetSelectedSystemRenderer with systemRenderer null while shift held");
                __runOriginal = false;
                return;
            }

            if (systemRenderer != null && systemRenderer != __instance.currSystem
                                       && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                ShiftClickMove.NextSelectIsShiftClick = true;

            __runOriginal = true;
            return;
        }
    }
}
