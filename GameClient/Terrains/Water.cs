using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.Properties;

namespace LevelRunner.Terrains
{
    public class Water : Terrain
    {
        public override int Patency { get; set; } = 1;
        public override Image Image { get; set; } = Resources.Water;
    }
}
