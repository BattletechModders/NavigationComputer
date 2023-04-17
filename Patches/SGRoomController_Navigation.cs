using BattleTech.UI;
using NavigationComputer.Features;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace NavigationComputer.Patches
{
    [HarmonyPatch(typeof(SGRoomController_Navigation), "ExitNavScreen")]
    public static class SGRoomController_Navigation_ExitNavScreen_Patch
    {
        public static void Prefix()
        {
            MapModesUI.TurnMapModeOff();
        }
    }
}
