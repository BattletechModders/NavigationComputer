using BattleTech;

namespace NavigationComputer.MapModes
{
    public interface IMapMode
    {
        string Name { get; }
        void Apply(SimGameState simGameState);
        void Unapply(SimGameState simGameState);
    }
}
