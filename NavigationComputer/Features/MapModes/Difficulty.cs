using BattleTech;

namespace NavigationComputer.Features.MapModes
{
    public class Difficulty : IMapMode
    {
        public string Name { get; } = "System Difficulty";

        public void Apply(SimGameState simGame)
        {
            foreach (var system in simGame.StarSystemDictionary.Keys)
            {
                var starSystem = simGame.StarSystemDictionary[system];
                var difficulty = starSystem.Def.GetDifficulty(simGame.SimGameMode);

                MapModesUI.ScaleSystem(system, difficulty / 5f);
            }
        }

        public void Unapply(SimGameState simGame)
        {
            foreach (var system in simGame.StarSystemDictionary.Keys)
                MapModesUI.ScaleSystem(system, 0.5f);
        }
    }
}
