using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.GameWorld;

namespace LevelRunner.Terrains
{
    public abstract class Terrain
    {
        public abstract Patency Patency { get; set; }
        public abstract Image Image { get; set; }
    }
}
