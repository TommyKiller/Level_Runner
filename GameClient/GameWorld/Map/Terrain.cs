﻿using LevelRunner.GameWorld.Map;
using System.Collections.Generic;
using System.Drawing;

namespace LevelRunner.GameWorld.Map
{
    public static class Terrain
    {
        public enum Terrains
        {
            Grass,
            Water
        }

        public static Patency GrassPatency = new Patency(PatencyMode.Free, PatencyMode.Free, PatencyMode.None);
        public static Patency WaterPatency = new Patency(PatencyMode.None, PatencyMode.Free, PatencyMode.Free);

        public static Dictionary<Terrains, Bitmap> TerrainImage { get; set; } = new Dictionary<Terrains, Bitmap>
        {
            { Terrains.Grass, new Bitmap(@"Resources\Assets\Terrain\Grass.jpg") },
            { Terrains.Water, new Bitmap(@"Resources\Assets\Terrain\Water.jpg") }
        };

        public class Patency
        {
            public PatencyMode GroundPatency { get; set; }
            public PatencyMode AirPatency { get; set; }
            public PatencyMode WaterPatency { get; set; }

            public Patency(PatencyMode groundPatency, PatencyMode airPatency, PatencyMode waterPatency)
            {
                GroundPatency = groundPatency;
                AirPatency = airPatency;
                WaterPatency = waterPatency;
            }

            public Patency(Patency patency)
            {
                GroundPatency = patency.GroundPatency;
                AirPatency = patency.AirPatency;
                WaterPatency = patency.WaterPatency;
            }
        }

        public enum PatencyMode
        {
            Free = 0,
            Occupied = 1,
            None = 2
        }
    }
}
