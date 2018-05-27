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
        public Character Actor { get; private set; }
        public World Parent { get; private set; }
        public Point Coordinates { get; private set; }
        public Size Size { get; private set; }
        private Size StandardShift { get; set; }

        public Camera(World parent)
        {
            Parent = parent;

            int height = Parent.ClientSize.Height / Parent.Settings.ChunkSize.Height;
            int width = Parent.ClientSize.Width / Parent.Settings.ChunkSize.Width;
            Size = new Size(width, height);

            StandardShift = new Size(63, 27);
        }

        public void Bind(Character actor)
        {
            Actor = actor;
            Centralize(Actor);
            Parent.Scene.BackGroundRepaint = true;

            #region Events
            Actor.CharacterDied += Unbind;
            Actor.CharacterChangedPosition += CheckPosition;
            #endregion
        }

        public void Unbind()
        {
            #region Unsubscribe events
            Actor.CharacterDied -= Unbind;
            Actor.CharacterChangedPosition -= CheckPosition;
            #endregion

            Actor = null;
        }

        public void Rebind(Character actor)
        {
            Unbind();
            Bind(actor);
        }

        public void Centralize(Character actor)
        {
            // Absolute coordinates (map related)
            int newX;
            int newY;
            if (actor.Coordinates.X - (Size.Width / 2) < 0)
            {
                newX = 0;
            }
            else
            {
                newX = actor.Coordinates.X - (Size.Width / 2);
            }
            if (actor.Coordinates.Y - (Size.Height / 2) < 0)
            {
                newY = 0;
            }
            else
            {
                newY = actor.Coordinates.Y - (Size.Height / 2);
            }
            Coordinates = new Point(newX, newY);
        }

        private void CheckPosition()
        {
            // Absolute coordinates in camera area
            if ((Actor.Coordinates.X < Coordinates.X) || (Actor.Coordinates.X >= Coordinates.X + Size.Width) ||
                (Actor.Coordinates.Y < Coordinates.Y) || (Actor.Coordinates.Y >= Coordinates.Y + Size.Height))
            {
                Vector direction = GetDirection();
                Move(direction);
            }
        }

        private void Move(Vector direction)
        {
            Coordinates = direction.GetEndPoint(Coordinates);
            Parent.Scene.BackGroundRepaint = true;
        }

        private Vector GetDirection()
        {
            // Absolute coordinates in camera area
            Vector direction = new Vector();
            if (Actor.Coordinates.X < Coordinates.X)
            {
                direction.X += Coordinates.X - StandardShift.Width < 0 ? // 0 - absolute 
                    - Coordinates.X : - StandardShift.Width; // coordinate (map related)
            }
            else if (Actor.Coordinates.X >= Coordinates.X + Size.Width)
            {
                direction.X += (Coordinates.X + Size.Width) + StandardShift.Width > Parent.Map.Width ? // Map.Width - absolute
                    Parent.Map.Width - (Coordinates.X + Size.Width) : StandardShift.Width; // coordinate (map related)
            }
            if (Actor.Coordinates.Y < Coordinates.Y)
            {
                direction.Y += Coordinates.Y - StandardShift.Height < 0 ? // 0 - absolute 
                    - Coordinates.Y : -StandardShift.Height; // coordinate (map related)
            }
            else if (Actor.Coordinates.Y >= Coordinates.Y + Size.Height)
            {
                direction.Y += (Coordinates.Y + Size.Height) + StandardShift.Height > Parent.Map.Height ? // Map.Height - absolute
                    Parent.Map.Height - (Coordinates.Y + Size.Height) : StandardShift.Height; // coordinate (map related)
            }
            return direction;
        }
    }
}
