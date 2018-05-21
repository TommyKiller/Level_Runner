using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LevelRunner.Actors;

namespace LevelRunner
{
    static class Mathematics
    {
        // Components
        private static Random random = new Random();
        
        public static double GetDistance(Point point1, Point point2)
        {
            return Math.Sqrt(
                Math.Pow(point2.X - point1.X, 2) +
                Math.Pow(point2.Y - point1.Y, 2));
        }

        public static Point GetRandomFreePoint(UnitType unitType)
        {
            Point point;
            do
            {
                point = GetRandomPoint(Program.World.Map.Width, Program.World.Map.Height);
            } while (!CheckPoint(point, unitType));
            return point;
        }

        public static bool CheckPoint(Point point, UnitType unitType)
        {
            bool isPointFree = false;
            Monitor.Enter(Program.World.Map);
            switch (unitType)
            {
                case UnitType.Ground:
                    if (Program.World.Map.PatencyLayer[point.Y, point.X].GroundPatency == GameWorld.Map.GroundPatencyMode.Free)
                    {
                        isPointFree = true;
                    }
                    break;
                case UnitType.Air:
                    if (Program.World.Map.PatencyLayer[point.Y, point.X].AirPatency == GameWorld.Map.AirPatencyMode.Free)
                    {
                        isPointFree = true;
                    }
                    break;
                case UnitType.Water:
                    if (Program.World.Map.PatencyLayer[point.Y, point.X].WaterPatency == GameWorld.Map.WaterPatencyMode.Free)
                    {
                        isPointFree = true;
                    }
                    break;
            }
            Monitor.Exit(Program.World.Map);
            return isPointFree;
        }

        public static Point GetRandomPoint(int xMax = 0, int yMax = 0)
        {
            int x = random.Next(xMax);
            int y = random.Next(yMax);
            return new Point(x, y);
        }

        public static int GetRandom()
        {
            return random.Next();
        }

        public static int GetRandom(int max)
        {
            return random.Next(max);
        }
    }
}
