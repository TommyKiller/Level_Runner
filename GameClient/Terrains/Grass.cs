using LevelRunner.GameWorld.Map;
using LevelRunner.Properties;
using System.Drawing;

namespace LevelRunner.Terrains
{
    public class Grass : Terrain
    {
        public override Patency Patency { get; set; }
        public override Image Image { get; set; }
        
        public Grass()
        {
            Patency = new Patency
            {
                AirPatency = AirPatencyMode.Free,
                GroundPatency = GroundPatencyMode.Free,
                WaterPatency = WaterPatencyMode.None
            };
            Image = Resources.Grass;
        }
    }
}
