﻿using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS.Logging;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            MapModes.Add(KeyCode.F1, new TagSearch());
            MapModes.Add(KeyCode.F2, new Unvisited());
        }


        // UI STUFF
        internal static GameObject MapModeTextGameObject;
        internal static TextMeshProUGUI MapModeText;
        internal static GameObject MapSearchGameObject;
        internal static TMP_InputField MapSearchInputField;
        internal static void SetupUIObjects(SGNavigationScreen navScreen)
        {
            if (MapModeTextGameObject == null)
            {
                MapModeTextGameObject = new GameObject("MapModes-Text");
                MapModeTextGameObject.AddComponent<RectTransform>();
                MapModeText = MapModeTextGameObject.AddComponent<TextMeshProUGUI>();
                MapModeText.alignment = TextAlignmentOptions.Center;
            }

            if (MapSearchGameObject == null)
            {
                MapSearchGameObject = new GameObject("MapModes-Search");
                MapSearchGameObject.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                MapSearchInputField = MapSearchGameObject.AddComponent<TMP_InputField>();

                var textArea = new GameObject("MapModes-Search-TextArea");
                textArea.AddComponent<RectMask2D>();
                textArea.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                textArea.transform.SetParent(MapSearchGameObject.transform);

                var text = new GameObject("MapModes-Search-Text");
                text.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                var textTMP = text.AddComponent<TextMeshProUGUI>();
                text.transform.SetParent(textArea.transform);

                textTMP.SetText(string.Empty, new object[0]);
                textTMP.enableWordWrapping = false;
                textTMP.extraPadding = true;
                textTMP.alignment = TextAlignmentOptions.Center;
                textTMP.fontSize = textTMP.fontSize * 0.75f;

                MapSearchInputField.textComponent = textTMP;
                MapSearchInputField.textViewport = textArea.GetComponent<RectTransform>();
                MapSearchInputField.interactable = false;
            }

            MapSearchGameObject.transform.SetParent(navScreen.transform);
            MapModeTextGameObject.transform.SetParent(navScreen.transform);

            // set font in the most roundabout way ever
            var fonts = Resources.FindObjectsOfTypeAll(typeof(TMP_FontAsset));
            foreach (TMP_FontAsset font in fonts)
            {
                if (font.name == "UnitedSansReg-Black SDF")
                    MapModeText.SetFont(font);

                if (font.name == "UnitedSansReg-Medium SDF")
                    MapSearchInputField.textComponent.SetFont(font);
            }

            ResetMapUI();
        }

        private static void SetMapModeText(string text)
        {
            MapModeText.text = text;
            MapModeTextGameObject.SetActive(true);

            // position text + search
            var textRectTransform = MapModeTextGameObject.GetComponent<RectTransform>();
            var searchRectTransform = MapSearchGameObject.GetComponent<RectTransform>();

            textRectTransform.anchorMin = new Vector2(0.5f, 1);
            textRectTransform.anchorMax = new Vector2(0.5f, 1);
            textRectTransform.anchoredPosition = new Vector3(0, -75, 0);

            searchRectTransform.anchorMin = new Vector2(0.5f, 1);
            searchRectTransform.anchorMax = new Vector2(0.5f, 1);
            searchRectTransform.anchoredPosition = new Vector3(0, -115, 0);
        }

        public static void ResetMapUI()
        {
            MapModeText.text = "MAP MODE";
            MapModeTextGameObject.SetActive(false);

            MapSearchInputField.text = "";
            MapSearchGameObject.SetActive(false);
        }


        // UTIL
        private static float? oldTravelIntensity = null;
        internal static void MapStuffSetActive(bool value)
        {
            var starmapBorder = SimGame.Starmap.Screen.transform.Find("RegionBorders").gameObject.GetComponent<StarmapBorders>();
            SimGame.Starmap.Screen.transform.Find("Background").gameObject.SetActive(value);

            if (value)
            {
                if (oldTravelIntensity != null)
                    starmapBorder.travelIntensity = (float)oldTravelIntensity;
            }
            else
            {
                oldTravelIntensity = oldTravelIntensity ?? starmapBorder.travelIntensity;
                starmapBorder.travelIntensity = 0;
            }

            SimGame.Starmap.Screen.RefreshBorders();
        }


        // MEAT
        internal static void TurnMapModeOn(IMapMode mapMode)
        {
            if (CurrentMapMode != null)
                TurnMapModeOff();

            CurrentMapMode = mapMode;
            HBSLog.Log($"Turning on map mode \"{CurrentMapMode.Name}\"");
            CurrentMapMode.Apply(SimGame);

            SetMapModeText(CurrentMapMode.Name);
            MapStuffSetActive(false);
        }

        internal static void TurnMapModeOff()
        {
            ResetMapUI();

            if (CurrentMapMode == null)
                return;

            HBSLog.Log($"Turning off map mode \"{CurrentMapMode.Name}\"");

            CurrentMapMode.Unapply(SimGame);
            CurrentMapMode = null;

            MapStuffSetActive(true);
            SimGame.Starmap.Screen.RefreshSystems();
        }

        internal static void ToggleMapMode(KeyCode key)
        {
            if (MapModes[key] == CurrentMapMode)
                TurnMapModeOff();
            else
                TurnMapModeOn(MapModes[key]);
        }

    }
}