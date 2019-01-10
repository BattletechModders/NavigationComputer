using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS.Logging;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace MapModes
{
    public static class Main
    {
        internal static ILog HBSLog;

        internal static IMapMode CurrentMapMode;
        internal static Dictionary<KeyCode, IMapMode> MapModes = new Dictionary<KeyCode, IMapMode>();

        internal static SimGameState SimGame;


        // ENTRY POINT
        public static void Init(string modDir, string modSettings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.MapModes");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = HBS.Logging.Logger.GetLogger("MapModes");

            // add the map modes
            MapModes.Add(KeyCode.F2, new VisitedStars());
        }


        // UI STUFF
        private static GameObject MapModeTextGameObject;
        private static TextMeshProUGUI MapModeText;
        public static void SetupUIObjects(SGNavigationScreen navScreen)
        {
            MapModeTextGameObject = MapModeTextGameObject ?? new GameObject("MapModes-Text");
            MapModeTextGameObject.AddComponent<RectTransform>();
            MapModeText = MapModeTextGameObject.AddComponent<TextMeshProUGUI>();
            MapModeTextGameObject.transform.SetParent(navScreen.transform);

            // set font in the most roundabout way ever
            var fonts = Resources.FindObjectsOfTypeAll(typeof(TMP_FontAsset));
            foreach (TMP_FontAsset font in fonts)
            {
                HBSLog.Log($"Found font \"{font.name}\"");

                if (font.name == "UnitedSansReg-Black SDF")
                {
                    MapModeText.SetFont(font);
                    HBSLog.Log($"\tSetting font to {font.name}");
                }
            }

            // set text to center
            MapModeText.alignment = TextAlignmentOptions.Center;

            HideMapModeText();
        }

        private static void SetMapModeText(string text)
        {
            MapModeText.text = text;
            MapModeTextGameObject.SetActive(true);

            // position at top of screen
            var rectTransform = MapModeTextGameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector3(0, -75, 0);
        }

        public static void HideMapModeText()
        {
            MapModeText.text = "MAP MODE";
            MapModeTextGameObject.SetActive(false);
        }


        // UTIL
        public static void MapStuffSetActive(bool value)
        {
            //SimGame.Starmap.Screen.transform.Find("RegionBorders").gameObject.SetActive(value);
            //SimGame.Starmap.Screen.transform.Find("Logos").gameObject.SetActive(value);
            SimGame.Starmap.Screen.transform.Find("Background").gameObject.SetActive(value);
        }


        // MEAT
        public static void TurnMapModeOn(IMapMode mapMode)
        {
            if (CurrentMapMode != null)
                TurnMapModeOff();

            CurrentMapMode = mapMode;
            HBSLog.Log($"Turning on map mode \"{CurrentMapMode.Name}\"");
            CurrentMapMode.Apply(SimGame);

            SetMapModeText(CurrentMapMode.Name);
            MapStuffSetActive(false);
        }

        public static void TurnMapModeOff()
        {
            if (CurrentMapMode == null)
                return;

            HBSLog.Log($"Turning off map mode \"{CurrentMapMode.Name}\"");

            CurrentMapMode.Unapply(SimGame);
            CurrentMapMode = null;

            HideMapModeText();
            MapStuffSetActive(true);

            SimGame.Starmap.Screen.RefreshSystems();
        }

        public static void ToggleMapMode(KeyCode key)
        {
            if (MapModes[key] == CurrentMapMode)
                TurnMapModeOff();
            else
                TurnMapModeOn(MapModes[key]);
        }

    }
}
