using BattleTech;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace MapModes
{
    public class Unvisited : IMapMode
    {
        public string Name { get; set; } = "Unvisited Systems";
        private float DimLevel;

        public Unvisited(float dimLevel = 10f)
        {
            DimLevel = dimLevel;
        }

        public void Apply(SimGameState simGame)
        {
            var visitedSystems = Traverse.Create(simGame).Field("VisitedStarSystems").GetValue<List<string>>();

            foreach (var system in visitedSystems)
                Main.DimSystem(system, DimLevel);
        }

        public void Unapply(SimGameState simGame)
        {
        }
    }
}
