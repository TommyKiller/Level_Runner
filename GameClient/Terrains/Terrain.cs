using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Terrains
{
    public abstract class Terrain
    {
        /// <summary>
        /// Patency:
        /// 0 - can go;
        /// 1 - can't go;
        /// 2 - can't fly.
        /// </summary>
        public abstract int Patency { get; set; }
        public abstract Image Image { get; set; }
    }
}
