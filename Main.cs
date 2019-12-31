using System.Reflection;
using Harmony;
using HBS.Logging;
using NavigationComputer.Features;

// ReSharper disable UnusedMember.Global

namespace NavigationComputer
{
    public static class Main
    {
        internal static ILog HBSLog;

        // ENTRY POINT
        public static void Init(string modDir, string modSettings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.NavigationComputer");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            HBSLog = Logger.GetLogger("NavigationComputer");

            MapModesUI.Setup();
        }
    }
}
