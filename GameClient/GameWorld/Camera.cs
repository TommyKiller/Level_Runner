using LevelRunner.Actors;
using LevelRunner.Mathematics;
using System.Drawing;

namespace LevelRunner.GameWorld
{
    public class Camera
    {
        // Events
        public static event Delegates.EventDelegate CameraChangedPosition;

        // Fields
        private Size _standardShift;
        private Point _coordinates;

        // Properties
        public Character Actor { get; private set; }
        public World Parent { get; private set; }
        public Point Coordinates
        {
            get => _coordinates;
            private set
            {
                _coordinates = value;
                CameraChangedPosition?.Invoke();
            }
        }
        public Size Size { get; private set; }
        public bool Binded { get; private set; }

        public Camera(World parent)
        {
            Parent = parent;

            int height = Parent.ClientSize.Height / Program.Settings.ChunkSize.Height;
            int width = Parent.ClientSize.Width / Program.Settings.ChunkSize.Width;
            Size = new Size(width, height);

            double x = width / 100.00 * 85.00;
            double y = height / 100.00 * 85.00;
            _standardShift = new Size((int)x, (int)y);
            Binded = false;
        }

        public void Bind(Character actor)
        {
            if (Binded)
            {
                Unbind();
            }
            Actor = actor;
            Centralize(Actor);
            Binded = true;

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
            Binded = false;
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
        }

        private Vector GetDirection()
        {
            // Absolute coordinates in camera area
            Vector direction = new Vector();
            if (Actor.Coordinates.X < Coordinates.X)
            {
                direction.X += Coordinates.X - _standardShift.Width < 0 ? // 0 - absolute 
                    -Coordinates.X : -_standardShift.Width; // coordinate (map related)
            }
            else if (Actor.Coordinates.X >= Coordinates.X + Size.Width)
            {
                direction.X += (Coordinates.X + Size.Width) + _standardShift.Width > Parent.Map.Width ? // Map.Width - absolute
                    Parent.Map.Width - (Coordinates.X + Size.Width) : _standardShift.Width; // coordinate (map related)
            }
            if (Actor.Coordinates.Y < Coordinates.Y)
            {
                direction.Y += Coordinates.Y - _standardShift.Height < 0 ? // 0 - absolute 
                    -Coordinates.Y : -_standardShift.Height; // coordinate (map related)
            }
            else if (Actor.Coordinates.Y >= Coordinates.Y + Size.Height)
            {
                direction.Y += (Coordinates.Y + Size.Height) + _standardShift.Height > Parent.Map.Height ? // Map.Height - absolute
                    Parent.Map.Height - (Coordinates.Y + Size.Height) : _standardShift.Height; // coordinate (map related)
            }
            return direction;
        }
    }
}
