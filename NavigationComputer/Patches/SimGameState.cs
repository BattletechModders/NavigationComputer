using BattleTech;
using Harmony;

namespace NavigationComputer
{
    [HarmonyPatch(typeof(SimGameState), "SetSimRoomState")]
    public static class SimGameState_SetSimRoomState_Patch
    {
        public static void Prefix(SimGameState __instance, DropshipLocation state)
        {
            if (state != DropshipLocation.NAVIGATION)
                Main.TurnMapModeOff();
        }
    }
}

