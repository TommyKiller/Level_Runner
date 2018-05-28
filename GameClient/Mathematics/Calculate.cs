using LevelRunner.Actors;
using LevelRunner.GameWorld.Map;
using LevelRunner.Terrains;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Mathematics
{
    static class Calculate
    {
        // Components
        private static Random _random = new Random();

        public static double GetDistance(Point point1, Point point2)
        {
            return Math.Sqrt(
                Math.Pow(point2.X - point1.X, 2) +
                Math.Pow(point2.Y - point1.Y, 2));
        }

        public static Point GetRandomFreePoint(UnitTypes unitType)
        {
            Point point;
            do
            {
                point = GetRandomPoint(Program.World.Map.Width, Program.World.Map.Height);
            } while (!CheckPoint(point, unitType));
            return point;
        }

        public static bool CheckPoint(Point point, UnitTypes unitType)
        {
            if ((point.X >= 0) && (point.Y >= 0) &&
                (point.X < Program.World.Map.Width) && (point.Y < Program.World.Map.Height))
            {
                switch (unitType)
                {
                    case UnitTypes.GroundUnit:
                        Monitor.Enter(Program.World.Map);
                        if (Program.World.Map.PatencyLayer[point.Y, point.X].GroundPatency == Terrain.PatencyMode.Free)
                        {
                            Monitor.Exit(Program.World.Map);
                            return true;
                        }
                        else
                        {
                            Monitor.Exit(Program.World.Map);
                        }
                        break;
                    case UnitTypes.AirUnit:
                        Monitor.Enter(Program.World.Map);
                        if (Program.World.Map.PatencyLayer[point.Y, point.X].AirPatency == Terrain.PatencyMode.Free)
                        {
                            Monitor.Exit(Program.World.Map);
                            return true;
                        }
                        else
                        {
                            Monitor.Exit(Program.World.Map);
                        }
                        break;
                    case UnitTypes.WaterUnit:
                        Monitor.Enter(Program.World.Map);
                        if (Program.World.Map.PatencyLayer[point.Y, point.X].WaterPatency == Terrain.PatencyMode.Free)
                        {
                            Monitor.Exit(Program.World.Map);
                            return true;
                        }
                        else
                        {
                            Monitor.Exit(Program.World.Map);
                        }
                        break;
                }
            }
            return false;
        }

        public static Point GetRandomPoint(int xMax = 0, int yMax = 0)
        {
            int x = _random.Next(xMax);
            int y = _random.Next(yMax);
            return new Point(x, y);
        }

        public static int GetRandom()
        {
            return _random.Next();
        }

        public static int GetRandom(int max)
        {
            return _random.Next(max);
        }
    }
}
