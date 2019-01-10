using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MapModes
{
    [HarmonyPatch(typeof(SGNavigationScreen), "Update")]
    public static class SGNavigationScreen_Update_Patch
    {
        public static void Postfix(SGNavigationScreen __instance)
        {
            foreach (var key in Main.MapModes.Keys)
            {
                if (Input.GetKeyUp(key))
                    Main.ToggleMapMode(key);
            }
        }
    }

    [HarmonyPatch(typeof(SGNavigationScreen), "Init", typeof(SimGameState), typeof(SGRoomController_Navigation))]
    public static class SGNavigationScreen_Init_Patch
    {
        public static void Postfix(SGNavigationScreen __instance, SimGameState simGame)
        {
            Main.SetupUIObjects(__instance);
            Main.SimGame = simGame;
        }
    }
}