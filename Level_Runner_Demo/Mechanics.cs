using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Level_Runner_Demo
{
    static class Mechanics
    {
        // Components
        private static Random random = new Random();

        public static void RespawnPlayer(string name, Point coordinates, Bitmap image, Character.Characteristics characteristics, string fraction)
        {
            // Debugging
            if (Program.GameClient.debug)
            {
                Program.GameClient.respawned++;
                Console.WriteLine("{0} respawned", name);
            }

            Monitor.Enter(Program.GameClient.actors);
            Program.GameClient.actors.Add(new Player(name, coordinates, image, characteristics, fraction));
            Program.GameClient.actors.Last().CharacterThread.Start();
            Monitor.Exit(Program.GameClient.actors);
        }
        
        public static double GetDistance(Point point1, Point point2)
        {
            return Math.Sqrt(
                Math.Pow(point2.X - point1.X, 2) +
                Math.Pow(point2.Y - point1.Y, 2));
        }

        public static void RespawnNPC(string name, Point coordinates, Bitmap image, Character.Characteristics characteristics, string fraction)
        {
            // Debugging
            if (Program.GameClient.debug)
            {
                Program.GameClient.respawned++;
                Console.WriteLine("{0} respawned", name);
            }

            Monitor.Enter(Program.GameClient.actors);
            Program.GameClient.actors.Add(new NPC(name, coordinates, image, characteristics, fraction));
            Program.GameClient.actors.Last().CharacterThread.Start();
            Monitor.Exit(Program.GameClient.actors);
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
                point = GetRandomPoint();
            } while (!CheckPoint(point));
            return point;
        }

        public static Point GetRandomPoint(int x = -1, int y = -1)
        {
            if (x < 0) x = random.Next(Program.GameClient.map.Width);
            if (y < 0) y = random.Next(Program.GameClient.map.Height);
            return new Point(x, y);
        }

        public static bool CheckPoint(Point point)
        {
            Monitor.Enter(Program.GameClient.map);
            if (Program.GameClient.map.Patency[point.Y, point.X] == 0)
            {
                Monitor.Exit(Program.GameClient.map);
                return true;
            }
            else
            {
                Monitor.Exit(Program.GameClient.map);
                return false;
            }
        }
    }
}
