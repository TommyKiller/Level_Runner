using LevelRunner.GameWorld.Map;
using LevelRunner.Properties;
using System.Drawing;

namespace LevelRunner.Terrains
{
    public class Water : Terrain
    {
        public override Patency Patency { get; set; }
        public override Image Image { get; set; }

        public Water()
        {
            Patency = new Patency
            {
                AirPatency = AirPatencyMode.Free,
                GroundPatency = GroundPatencyMode.None,
                WaterPatency = WaterPatencyMode.Free
            };
            Image = Resources.Water;
        }
    }
}
