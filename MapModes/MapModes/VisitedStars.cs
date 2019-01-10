using BattleTech;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace MapModes
{
    public class VisitedStars : IMapMode
    {
        private MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        public string Name { get; set; } = "Visited Stars";
        public float DimLevel { get; private set; }

        public VisitedStars(float dimLevel = 3f)
        {
            DimLevel = dimLevel;
        }

        public void Apply(SimGameState simGame)
        {
            var visitedSystems = Traverse.Create(simGame).Field("VisitedStarSystems").GetValue<List<string>>();

            mpb.Clear();
            foreach (var system in simGame.StarSystemDictionary.Keys)
            {
                if (visitedSystems.Contains(system))
                    continue;

                var systemRenderer = simGame.Starmap.Screen.GetSystemRenderer(system);
                var starOuter = Traverse.Create(systemRenderer).Field("starOuter").GetValue<Renderer>();
                var starInner = Traverse.Create(systemRenderer).Field("starInner").GetValue<Renderer>();

                var newColor = systemRenderer.systemColor / DimLevel;
                newColor.a = .5f;

                // set outer color
                mpb.SetColor("_Color", newColor);
                starOuter.SetPropertyBlock(mpb);

                // set inner color
                mpb.SetColor("_Color", newColor * 2f);
                starInner.SetPropertyBlock(mpb);
            }
        }

        public void Unapply(SimGameState simGame)
        {
        }
    }
}
