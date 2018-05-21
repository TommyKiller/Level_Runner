using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.GameWorld.Map;
using LevelRunner.Properties;

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
