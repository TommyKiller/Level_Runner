using LevelRunner.Actors;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Mathematics
{
    static class Calculate
    {
        // Components
        private static Random random = new Random();

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
                (point.X < Program.World.Width) && (point.Y < Program.World.Height))
            {
                switch (unitType)
                {
                    case UnitTypes.GroundUnit:
                        Monitor.Enter(Program.World.Map);
                        if (Program.World.Map.PatencyLayer[point.Y, point.X].GroundPatency == GameWorld.Map.GroundPatencyMode.Free)
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
                        if (Program.World.Map.PatencyLayer[point.Y, point.X].AirPatency == GameWorld.Map.AirPatencyMode.Free)
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
                        if (Program.World.Map.PatencyLayer[point.Y, point.X].WaterPatency == GameWorld.Map.WaterPatencyMode.Free)
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
