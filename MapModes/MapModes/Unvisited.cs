using BattleTech;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace MapModes
{
    public class Unvisited : IMapMode
    {
        private MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        public string Name { get; set; } = "Unvisited Systems";


        private float DimLevel;
        public Unvisited(float dimLevel = 10f)
        {
            DimLevel = dimLevel;
        }

        public void Apply(SimGameState simGame)
        {
            var visitedSystems = Traverse.Create(simGame).Field("VisitedStarSystems").GetValue<List<string>>();

            mpb.Clear();
            foreach (var system in visitedSystems)
            {
                var systemRenderer = simGame.Starmap.Screen.GetSystemRenderer(system);
                var starOuter = Traverse.Create(systemRenderer).Field("starOuter").GetValue<Renderer>();
                var starInner = Traverse.Create(systemRenderer).Field("starInner").GetValue<Renderer>();

                var newColor = systemRenderer.systemColor / DimLevel;

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
