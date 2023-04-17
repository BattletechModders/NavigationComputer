using BattleTech;
using NavigationComputer.Features;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace NavigationComputer.Patches
{
    [HarmonyPatch(typeof(Starmap), "SetSelectedSystem", typeof(StarSystemNode))]
    public static class Starmap_SetSelectedSystem_Patch
    {
        public static void Prefix(ref bool __runOriginal, Starmap __instance, StarSystemNode node)
        {
            if (!__runOriginal) return;
            var result = ShiftClickMove.HandleClickSystem(__instance, node);
            if (!result)
            {
                __runOriginal = false;
                return;
            }
            __runOriginal = true;
            return;
        }
    }

    [HarmonyPatch(typeof(Starmap), "GetLocationByUV")]
    public static class Starmap_GetLocationByUV_Patch
    {
        // only call to this function is from StarmapScreen.Update()
        // intercept and check shift status
        public static void Postfix(Starmap __instance, StarSystemNode __result)
        {
            if (__result != null && __result != __instance.CurSelected && __result != __instance.CurPlanet
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                ShiftClickMove.NextSelectIsShiftClick = true;
        }
    }
}
