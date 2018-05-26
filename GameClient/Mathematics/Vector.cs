using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Mathematics
{
    public class Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector()
        {
            Point point = new Point();
            X = point.X;
            Y = point.Y;
        }

        public Vector(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point GetStartPoint(Point endPoint)
        {
            return new Point(endPoint.X - X, endPoint.Y - Y);
        }

        public Point GetEndPoint(Point startPoint)
        {
            return new Point(startPoint.X + X, startPoint.Y + Y);
        }
    }
}
