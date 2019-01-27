using Harmony;
using HBS.Logging;
using System.Reflection;
using JetBrains.Annotations;

namespace NavigationComputer
{
    public static class Main
    {
        internal static ILog HBSLog;

        // ENTRY POINT
        [UsedImplicitly]
        public static void Init(string modDir, string modSettings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.NavigationComputer");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            HBSLog = Logger.GetLogger("NavigationComputer");

            MapModesUI.Setup();
        }
    }
}
