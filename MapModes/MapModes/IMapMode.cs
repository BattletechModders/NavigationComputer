using BattleTech;

namespace MapModes
{
    public interface IMapMode
    {
        string Name { get; set; }
        void Apply(SimGameState simGameState);
        void Unapply(SimGameState simGameState);
    }
}
