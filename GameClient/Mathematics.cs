using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        
        public static Point GetDirection(Point point1, Point point2)
        {
            int xDirection = (point2.X - point1.X + 1) / Math.Abs(point2.X - point1.X + 1);
            int yDirection = (point2.Y - point1.Y + 1) / Math.Abs(point2.Y - point1.Y + 1);
            return new Point(xDirection, yDirection);
        }

        public static Point GetRandomFreePoint()
        {
            Point point;
            do
            {
                point = GetRandomPoint(Program.World.Map.Width, Program.World.Map.Height);
            } while (!CheckPoint(point));
            return point;
        }

        public static Point GetRandomPoint(int xMax = 0, int yMax = 0)
        {
            int x = random.Next(xMax);
            int y = random.Next(yMax);
            return new Point(x, y);
        }

        public static int GetRandomCoordinate()
        {
            return random.Next();
        }

        public static int GetRandomCoordinate(int max)
        {
            return random.Next(max);
        }

        public static bool CheckPoint(Point point)
        {
            Monitor.Enter(Program.World.Map);
            if (Program.World.Map.Patency[point.Y, point.X] == 0)
            {
                Monitor.Exit(Program.World.Map);
                return true;
            }
            else
            {
                Monitor.Exit(Program.World.Map);
                return false;
            }
        }
    }
}
