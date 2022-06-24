using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech.Rendering;
using Harmony;
using HBS.Logging;
using NavigationComputer.Features;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace NavigationComputer
{
    public static class Main
    {
        internal static ILog HBSLog;

        internal static Settings modSettings;
        // ENTRY POINT
        public static void Init(string modDir, string settings)
        {
            HBSLog = Logger.GetLogger("NavigationComputer");
            try
            {
                modSettings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch (Exception ex)
            {
                HBSLog.LogException(ex);
                modSettings = new Settings();
            }
            var harmony = HarmonyInstance.Create("io.github.mpstark.NavigationComputer");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            MapModesUI.Setup();
        }
    }

    class Settings
    {
        public Dictionary<string, string> SearchableTags = new Dictionary<string, string>();
    }
}
