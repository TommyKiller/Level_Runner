using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.GameWorld.Map
{
    public class Patency
    {
        public GroundPatencyMode GroundPatency { get; set; }
        public AirPatencyMode AirPatency { get; set; }
        public WaterPatencyMode WaterPatency { get; set; }
    }

    public enum GroundPatencyMode
    {
        Free = 0,
        Occupied = 1,
        None = 2
    }

    public enum AirPatencyMode
    {
        Free = 0,
        Occupied = 1,
        None = 2
    }

    public enum WaterPatencyMode
    {
        Free = 0,
        Occupied = 1,
        None = 2
    }
}
