using LevelRunner.GameWorld.Map;
using System.Drawing;

namespace LevelRunner.Terrains
{
    public abstract class Terrain
    {
        public abstract Patency Patency { get; set; }
        public abstract Image Image { get; set; }
    }
}
