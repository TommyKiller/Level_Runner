using LevelRunner.Actors;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace LevelRunner.GameWorld
{
    public class Scene
    {
        // Fields
        private List<Point> oldChunks;

        // Propereties
        private bool BackGroundRepaint { get; set; }
        public World Parent { get; }
        public Point Coordinates { get; private set; }
        public Size Size { get; private set; }

        public Scene(Point coordinates, Size size, World parent)
        {
            Coordinates = coordinates;
            Size = size;
            Parent = parent;
            Parent.Canvas = Parent.CreateGraphics();
            oldChunks = new List<Point>();
            BackGroundRepaint = true;
        }

        public void Repaint()
        {
            if (BackGroundRepaint)
            {
                RepaintBackGround();
                BackGroundRepaint = false;
            }
            RepaintOldChunks();
            RepaintActors();
        }

        private void RepaintBackGround()
        {
            for (int i = Coordinates.X; i < (Coordinates.X + Size.Width); i++)
            {
                for (int j = Coordinates.Y; j < (Coordinates.Y + Size.Height); j++)
                {
                    Parent.Canvas.DrawImage(Parent.Map.TerrainLayer[j, i].Image, new Point((i * Parent.Settings.ChunkSize.Width), (j * Parent.Settings.ChunkSize.Height)));
                }
            }
        }

        private void RepaintActors()
        {
            Monitor.Enter(Parent.Actors);
            if (Parent.Actors.Count > 0)
            {
                foreach (Character character in Parent.Actors)
                {
                    if ((character.Coordinates.X >= Coordinates.X) &&
                        (character.Coordinates.X <= Size.Width) &&
                        (character.Coordinates.Y >= Coordinates.Y) &&
                        (character.Coordinates.Y <= Size.Height))
                        Parent.Canvas.DrawImage(character.Image, new Point(character.Coordinates.X * Parent.Settings.ChunkSize.Width,
                            character.Coordinates.Y * Parent.Settings.ChunkSize.Height));
                }
            }
            Monitor.Exit(Parent.Actors);
        }

        private void RepaintOldChunks()
        {
            Monitor.Enter(oldChunks);
            foreach (Point point in oldChunks)
            {
                if ((point.X < Parent.Map.Width) && (point.Y < Parent.Map.Height) &&
                    (point.X >= 0) && (point.Y >= 0))
                {
                    RepaintChunk(point);
                }
            }
            oldChunks.Clear();
            Monitor.Exit(oldChunks);
        }

        private void RepaintChunk(Point point)
        {
            Parent.Canvas.DrawImage(Parent.Map.TerrainLayer[point.Y, point.X].Image,
                new Point(point.X * Parent.Settings.ChunkSize.Width, point.Y * Parent.Settings.ChunkSize.Height));
        }

        public void AddOldChunk(Point point)
        {
            Monitor.Enter(oldChunks);
            if (!oldChunks.Contains(point)) oldChunks.Add(point);
            Monitor.Exit(oldChunks);
        }
    }
}
