using LevelRunner.Actors;
using LevelRunner.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.GameWorld
{
    public class Camera
    {
        public Player Actor { get; private set; }
        public World Parent { get; private set; }
        public Point Coordinates { get; private set; }
        public Size Size { get; private set; }
        private Size StandardShift { get; set; }

        public Camera(World parent, Size size)
        {
            Parent = parent;
            Size = size;
            StandardShift = new Size(63, 27);
        }

        public void Bind(Player actor)
        {
            Actor = actor;
            Actor.PlayerChangedPosition += CheckPosition;
            Coordinates = new Point(0, 0);
            Centralize();
            Parent.Scene.BackGroundRepaint = true;
        }

        public void Unbind()
        {
            Actor = null;
            Actor.PlayerChangedPosition -= CheckPosition;
        }

        private void Centralize()
        {
            int newX;
            int newY;
            if (Actor.Coordinates.X - (Size.Width / 2) < 0)
            {
                newX = 0;
            }
            else
            {
                newX = Actor.Coordinates.X - (Size.Width / 2);
            }
            if (Actor.Coordinates.Y - (Size.Height / 2) < 0)
            {
                newY = 0;
            }
            else
            {
                newY = Actor.Coordinates.Y - (Size.Height / 2);
            }
            Coordinates = new Point(newX, newY);
        }

        private void CheckPosition()
        {
            if (((Actor.Coordinates.X - Coordinates.X) < 0) || ((Actor.Coordinates.X - Coordinates.X) >= Size.Width) ||
                ((Actor.Coordinates.Y - Coordinates.Y) < 0) || ((Actor.Coordinates.Y - Coordinates.Y) >= Size.Height))
            {
                Move();
            }
        }

        private void Move()
        {
            Vector direction = GetDirection();
            Coordinates = new Point(Coordinates.X + direction.X, Coordinates.Y + direction.Y);
            Parent.Scene.BackGroundRepaint = true;
        }

        private Vector GetDirection()
        {
            Vector direction = new Vector();
            if ((Actor.Coordinates.X - Coordinates.X) < 0)
            {
                direction.X += Coordinates.X - StandardShift.Width < 0 ?
                    - Coordinates.X : - StandardShift.Width;
            }
            else if ((Actor.Coordinates.X - Coordinates.X) >= Size.Width)
            {
                direction.X += (Coordinates.X + Size.Width) + StandardShift.Width > Parent.Map.Width ?
                    Parent.Map.Width - (Coordinates.X + Size.Width) : StandardShift.Width;
            }
            if ((Actor.Coordinates.Y - Coordinates.Y) < 0)
            {
                direction.Y += Coordinates.Y - StandardShift.Height < 0 ?
                    - Coordinates.Y : -StandardShift.Height;
            }
            else if ((Actor.Coordinates.Y - Coordinates.Y) >= Size.Height)
            {
                direction.Y += (Coordinates.Y + Size.Height) + StandardShift.Height > Parent.Map.Height ?
                    Parent.Map.Height - (Coordinates.Y + Size.Height) : StandardShift.Height;
            }
            return direction;
        }
    }
}
