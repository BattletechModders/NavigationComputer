using BattleTech.UI;
using Harmony;

namespace MapModes
{
    //[HarmonyPatch(typeof(SGRoomController_Navigation), "EnterRoom")]
    //public static class SGRoomController_Navigation_EnterRoom_Patch
    //{
    //    public static void Postfix(SGRoomController_Navigation __instance)
    //    {
    //    }
    //}

    [HarmonyPatch(typeof(SGRoomController_Navigation), "LeaveRoom")]
    public static class SGRoomController_Navigation_LeaveRoom_Patch
    {
        public static void Postfix(SGRoomController_Navigation __instance)
        {
            Main.TurnMapModeOff();
        }
    }
}