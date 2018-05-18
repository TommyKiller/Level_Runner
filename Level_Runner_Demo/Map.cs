using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Runner_Demo
{
    public class Map
    {
        public int[,] Terrain { get; set; }
        public int[,] Patency { get; set; }
        public int Width
        {
            get
            {
                return Terrain.GetLength(1);
            }
        }
        public int Height
        {
            get
            {
                return Terrain.GetLength(0);
            }
        }

        public void Load(Map savedMap)
        {
            Terrain = savedMap.Terrain;
            Patency = savedMap.Patency;
        }

        public void New(int sizeX, int sizeY)
        {
            Terrain = new int[sizeY, sizeX];
            Patency = new int[sizeY, sizeX];
            GeneratePlateau();
            GenerateRiver();
            ReWritePatency();
        }

        private void GeneratePlateau() // !!!!!
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Terrain[i, j] = 0;
                }
            }
        }

        private void GenerateRiver() // !!!!!
        {
            Point start = Mechanics.GetRandomPoint(y: 0);
            Point end = new Point(start.X, Height - 1);
            Point currentChunk = start;
            Point direction = Mechanics.GetDirection(start, end);
            while (currentChunk.Y <= end.Y)
            {
                Terrain[currentChunk.Y++, currentChunk.X] = 5;
                /*
                if ((currentChunk.X == end.X) || (currentChunk.Y == end.Y))
                {
                    currentChunk = new Point(currentChunk.X + (end.X - currentChunk.X), currentChunk.Y + (end.Y - currentChunk.Y));
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                }
                int sectionChose = new Random().Next(100);
                if (sectionChose < 15)
                {
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                    currentChunk.Y += direction.Y;
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                    currentChunk.X += direction.X;
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                }
                else if ((15 <= sectionChose) && (sectionChose < 30))
                {
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                    currentChunk.X += direction.X;
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                    currentChunk.Y += direction.Y;
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                }
                else if ((30 <= sectionChose) && (sectionChose < 65))
                {
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                    currentChunk.X += direction.X;
                }
                else if (65 <= sectionChose)
                {
                    Terrain[currentChunk.Y, currentChunk.X] = 5;
                    Patency[currentChunk.Y, currentChunk.X] = 0;
                    currentChunk.Y += direction.Y;
                }
                */
            }
        }

        private void ReWritePatency()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Terrain[i, j] < 6) Patency[i, j] = 0;
                    else Patency[i, j] = 1;
                }
            }
            if (GameClient.actors.Count > 0)
            {
                foreach (Character character in GameClient.actors)
                {
                    Patency[character.Y, character.X] = 1;
                }
            }
        }
    }
}
