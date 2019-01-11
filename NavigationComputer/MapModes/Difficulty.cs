using BattleTech;
using System;
using System.Linq;

namespace NavigationComputer
{
    public class Difficulty : IMapMode
    {
        public string Name { get; set; } = "System Difficulty";
        private float DimLevel;

        public Difficulty(float dimLevel = 10f)
        {
            DimLevel = dimLevel;
        }

        public void Apply(SimGameState simGame)
        {
            //var mechs = simGame.ActiveMechs.Values.OrderByDescending(x => x.Chassis.Tonnage);
            //var heaviestMechRating = SimGameBattleSimulator.GetLanceTonnageRating(simGame, mechs.Take(4).ToList(), out _);

            foreach (var system in simGame.StarSystemDictionary.Keys)
            {
                var starSystem = simGame.StarSystemDictionary[system];
                var difficulty = starSystem.Def.GetDifficulty(simGame.SimGameMode);

                Main.ScaleSystem(system, difficulty / 5f);
                //Main.DimSystem(system, (Math.Abs(heaviestMechRating - difficulty) + 1) * 2f);
            }
        }

        public void Unapply(SimGameState simGame)
        {
            foreach (var system in simGame.StarSystemDictionary.Keys)
                Main.ScaleSystem(system, 0.5f);
        }
    }
}
