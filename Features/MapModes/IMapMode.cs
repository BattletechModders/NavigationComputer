using BattleTech;

namespace NavigationComputer.Features.MapModes
{
    public interface IMapMode
    {
        string Name { get; }
        void Apply(SimGameState simGameState);
        void Unapply(SimGameState simGameState);
    }
}
