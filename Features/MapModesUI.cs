using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using NavigationComputer.Features.MapModes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NavigationComputer.Features
{
    public static class MapModesUI
    {
        internal static IMapMode CurrentMapMode;
        internal static readonly Dictionary<KeyCode, IMapMode> DiscreteMapModes = new Dictionary<KeyCode, IMapMode>();
        internal static IMapMode SearchMapMode;

        internal static SimGameState SimGame;
        internal static SGNavigationScreen NavigationScreen;

        private static readonly MaterialPropertyBlock MPB = new MaterialPropertyBlock();
        private static float? _oldTravelIntensity;

        internal static GameObject MapModeTextGameObject;
        internal static TextMeshProUGUI MapModeText;
        internal static GameObject MapSearchGameObject;
        internal static TMP_InputField MapSearchInputField;


        public static void Setup()
        {
            // add the map modes
            DiscreteMapModes.Add(KeyCode.F1, new Unvisited());
            DiscreteMapModes.Add(KeyCode.F2, new Difficulty());
            SearchMapMode = new Search();
        }


        // UI STUFF
        internal static void SetupUIObjects(SGNavigationScreen navScreen)
        {
            NavigationScreen = navScreen;

            if (MapModeTextGameObject == null)
            {
                MapModeTextGameObject = new GameObject("NavigationComputer-Text");
                MapModeTextGameObject.AddComponent<RectTransform>();
                MapModeText = MapModeTextGameObject.AddComponent<TextMeshProUGUI>();
                MapModeText.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                MapModeText.alignment = TextAlignmentOptions.Center;
            }

            if (MapSearchGameObject == null)
            {
                MapSearchGameObject = new GameObject("NavigationComputer-Search");
                MapSearchGameObject.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                MapSearchInputField = MapSearchGameObject.AddComponent<TMP_InputField>();

                var textArea = new GameObject("NavigationComputer-Search-TextArea");
                textArea.AddComponent<RectMask2D>();
                textArea.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                textArea.transform.SetParent(MapSearchGameObject.transform);

                var text = new GameObject("NavigationComputer-Search-Text");
                text.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
                var textTMP = text.AddComponent<TextMeshProUGUI>();
                text.transform.SetParent(textArea.transform);

                textTMP.SetText(string.Empty);
                textTMP.enableWordWrapping = false;
                textTMP.extraPadding = true;
                textTMP.alignment = TextAlignmentOptions.Center;
                textTMP.fontSize = textTMP.fontSize * 0.75f;

                MapSearchInputField.textComponent = textTMP;
                MapSearchInputField.textViewport = textArea.GetComponent<RectTransform>();
            }

            MapSearchGameObject.transform.SetParent(navScreen.transform);
            MapModeTextGameObject.transform.SetParent(navScreen.transform);

            // set font in the most roundabout way ever
            var fonts = Resources.FindObjectsOfTypeAll(typeof(TMP_FontAsset));
            foreach (var o in fonts)
            {
                var font = (TMP_FontAsset) o;

                if (font.name == "UnitedSansReg-Black SDF")
                    MapModeText.font = font;

                if (font.name == "UnitedSansReg-Medium SDF")
                    MapSearchInputField.textComponent.font = font;
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
        internal static void SetMapStuffActive(bool active)
        {
            var starmapBorder = SimGame.Starmap.Screen.transform.Find("RegionBorders").gameObject
                .GetComponent<StarmapBorders>();
            SimGame.Starmap.Screen.transform.Find("Background").gameObject.SetActive(active);

            // hide the annoying yellow undertone
            if (active)
            {
                if (_oldTravelIntensity != null)
                    starmapBorder.travelIntensity = (float) _oldTravelIntensity;
            }
            else
            {
                _oldTravelIntensity = _oldTravelIntensity ?? starmapBorder.travelIntensity;
                starmapBorder.travelIntensity = 0;

                SimGame.Starmap.Screen.ForceClickSystem((StarmapSystemRenderer) null);

                Traverse.Create(NavigationScreen).Method("ResetSpecialIndicators").GetValue();
            }

            SimGame.Starmap.Screen.RefreshBorders();
        }

        internal static void DimSystem(string system, float dimLevel)
        {
            MPB.Clear();

            var systemRenderer = SimGame.Starmap.Screen.GetSystemRenderer(system);
            var starOuter = Traverse.Create(systemRenderer).Field("starOuter").GetValue<Renderer>();
            var starInner = Traverse.Create(systemRenderer).Field("starInner").GetValue<Renderer>();
            var starInnerUnvisited = Traverse.Create(systemRenderer).Field("starInnerUnvisited").GetValue<Renderer>();

            var newColor = systemRenderer.systemColor / dimLevel;

            // set outer color
            MPB.SetColor("_Color", newColor);
            starOuter.SetPropertyBlock(MPB);

            // set inner color
            MPB.SetColor("_Color", newColor * 2f);
            starInner.SetPropertyBlock(MPB);
            starInnerUnvisited.SetPropertyBlock(MPB);
        }

        internal static void ScaleSystem(string system, float scale)
        {
            var systemRenderer = SimGame.Starmap.Screen.GetSystemRenderer(system);

            //HBSLog.Log($"Scaling {system} to {scale} -- old scale {systemRenderer.transform.localScale}");
            systemRenderer.transform.localScale = new Vector3(scale, scale, scale);
        }


        // MEAT
        internal static void TurnMapModeOn(IMapMode mapMode)
        {
            if (CurrentMapMode != null)
                TurnMapModeOff();

            CurrentMapMode = mapMode;
            Main.HBSLog.Log($"Turning on map mode \"{CurrentMapMode.Name}\"");
            CurrentMapMode.Apply(SimGame);

            SetMapModeText(CurrentMapMode.Name);
            SetMapStuffActive(false);
        }

        internal static void TurnMapModeOff()
        {
            ResetMapUI();

            if (CurrentMapMode == null)
                return;

            Main.HBSLog.Log($"Turning off map mode \"{CurrentMapMode.Name}\"");

            CurrentMapMode.Unapply(SimGame);
            CurrentMapMode = null;

            SetMapStuffActive(true);
            NavigationScreen.RefreshWidget();
        }

        internal static void ToggleMapMode(KeyCode key)
        {
            if (!SimGame.Starmap.Screen.StarmapVisible)
                return;

            if (DiscreteMapModes[key] == CurrentMapMode)
                TurnMapModeOff();
            else
                TurnMapModeOn(DiscreteMapModes[key]);
        }

        internal static void StartSearching()
        {
            if (!SimGame.Starmap.Screen.StarmapVisible)
                return;

            if (CurrentMapMode != SearchMapMode)
            {
                TurnMapModeOff();
            }
            else
            {
                MapSearchInputField.DeactivateInputField();
                MapSearchInputField.ActivateInputField();
                MapSearchInputField.Select();
            }

            if (CurrentMapMode == null)
                TurnMapModeOn(SearchMapMode);
        }
    }
}
