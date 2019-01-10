using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MapModes
{
    [HarmonyPatch(typeof(SGRoomController_Navigation), "ExitNavScreen")]
    public static class SGRoomController_Navigation_ExitNavScreen_Patch
    {
        public static void Prefix(SGRoomController_Navigation __instance)
        {
            Main.TurnMapModeOff();
        }
    }
}