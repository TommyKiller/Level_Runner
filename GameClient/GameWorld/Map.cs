using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.Actors;
using LevelRunner.Terrains;

namespace LevelRunner.GameWorld
{
    public class Map
    {
        private World Parent { get; }
        public Terrain[,] TerrainLayer { get; set; }
        public Patency[,] PatencyLayer { get; set; }
        public int Width
        {
            get
            {
                return TerrainLayer.GetLength(1);
            }
        }
        public int Height
        {
            get
            {
                return TerrainLayer.GetLength(0);
            }
        }

        public Map(int sizeX, int sizeY, World parent)
        {
            Parent = parent;
            TerrainLayer = new Terrain[sizeY, sizeX];
            PatencyLayer = new Patency[sizeY, sizeX];
            GeneratePlateau();
            GenerateRiver();
            ReWritePatency();
        }

        public Map(Map savedMap, World parent)
        {
            Parent = parent;
            TerrainLayer = savedMap.TerrainLayer;
            PatencyLayer = savedMap.PatencyLayer;
        }

        private void GeneratePlateau() // !!!!!
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    TerrainLayer[i, j] = new Grass();
                }
            }
        }

        private void GenerateRiver() // !!!!!
        {
            Point start = new Point(Mathematics.GetRandomCoordinate(Width), 0);
            Point end = new Point(start.X, Height - 1);
            Point currentChunk = start;
            Point direction = Mathematics.GetDirection(start, end);
            while (currentChunk.Y <= end.Y)
            {
                TerrainLayer[currentChunk.Y++, currentChunk.X] = new Water();
            }
        }

        private void ReWritePatency()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    PatencyLayer[i, j] = TerrainLayer[i, j].Patency;
                }
            }

            if (Parent.Actors.Count > 0)
            {
                foreach (Character character in Parent.Actors)
                {
                    PatencyLayer[character.Y, character.X] = Patency.Occupied;
                }
            }
        }
    }
}
