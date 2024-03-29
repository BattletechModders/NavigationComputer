﻿using BattleTech;
using BattleTech.UI;
using NavigationComputer.Features;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace NavigationComputer.Patches
{
    [HarmonyPatch(typeof(SGNavigationScreen), "Update")]
    public static class SGNavigationScreen_Update_Patch
    {
        public static void Postfix()
        {
            foreach (var key in MapModesUI.DiscreteMapModes.Keys)
            {
                if (Input.GetKeyUp(key))
                    MapModesUI.ToggleMapMode(key);
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
                MapModesUI.StartSearching();
        }
    }

    [HarmonyPatch(typeof(SGNavigationScreen), "Init", typeof(SimGameState), typeof(SGRoomController_Navigation))]
    public static class SGNavigationScreen_Init_Patch
    {
        public static void Postfix(SGNavigationScreen __instance, SimGameState simGame)
        {
            MapModesUI.SetupUIObjects(__instance);
            MapModesUI.SimGame = simGame;
        }
    }

    [HarmonyPatch(typeof(SGNavigationScreen), "HandleEscapeKeypress")]
    public static class SGNavigationScreen_HandleEscapeKeypress_Patch
    {
        public static void Prefix(ref bool __runOriginal, ref bool __result)
        {
            if (!__runOriginal) return;
            if (MapModesUI.CurrentMapMode == null)
            {
                __runOriginal = true;
                return;
            }
            
            // the return value in __result is if the esc was handled
            MapModesUI.TurnMapModeOff();
            __result = true;
            __runOriginal = false;
            return;
        }
    }
}
