using LevelRunner.Mathematics;
using LevelRunner.Terrains;
using System.Drawing;

namespace LevelRunner.GameWorld.Map
{
    public class Map
    {
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

        public Map(int sizeX, int sizeY)
        {
            TerrainLayer = new Terrain[sizeY, sizeX];
            PatencyLayer = new Patency[sizeY, sizeX];
            GeneratePlateau();
            GenerateRiver();
        }

        public Map(Map savedMap)
        {
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
                    PatencyLayer[i, j] = TerrainLayer[i, j].Patency;
                }
            }
        }

        private void GenerateRiver() // !!!!!
        {
            Point start = new Point(Calculate.GetRandom(Width), 0);
            Point end = new Point(start.X, Height - 1);
            Point currentChunk = start;
            while (currentChunk.Y <= end.Y)
            {
                TerrainLayer[currentChunk.Y, currentChunk.X] = new Water();
                PatencyLayer[currentChunk.Y, currentChunk.X] = TerrainLayer[currentChunk.Y++, currentChunk.X].Patency;
            }
        }
    }
}
