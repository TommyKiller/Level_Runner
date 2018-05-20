using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.GameWorld;
using LevelRunner.Properties;

namespace LevelRunner.Terrains
{
    public class Grass : Terrain
    {
        public override Patency Patency { get; set; } = Patency.Ground;
        public override Image Image { get; set; } = Resources.Grass;
    }
}
