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
        public bool BackGroundRepaint { get; set; }
        public World Parent { get; }

        public Scene(World parent)
        {
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
            for (int i = Parent.Camera.Coordinates.X; i < Parent.Camera.Coordinates.X + Parent.Camera.Size.Width; i++)
            {
                for (int j = Parent.Camera.Coordinates.Y; j < Parent.Camera.Coordinates.Y + Parent.Camera.Size.Height; j++)
                {
                    if ((i >= 0) && (i < Parent.Map.Width) &&
                        (j >= 0) && (j < Parent.Map.Height))
                    {
                        Parent.Canvas.DrawImage(Parent.Map.TerrainLayer[j, i].Image, new Point(
                        ((i - Parent.Camera.Coordinates.X) * Parent.Settings.ChunkSize.Width),
                        ((j - Parent.Camera.Coordinates.Y) * Parent.Settings.ChunkSize.Height)));
                    }
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
                    if (((character.Coordinates.X - Parent.Camera.Coordinates.X) >= 0) &&
                        ((character.Coordinates.X - Parent.Camera.Coordinates.X) < Parent.Camera.Size.Width) &&
                        ((character.Coordinates.Y - Parent.Camera.Coordinates.Y) >= 0) &&
                        ((character.Coordinates.Y - Parent.Camera.Coordinates.Y) < Parent.Camera.Size.Height))
                        Parent.Canvas.DrawImage(character.Image, new Point(
                            (character.Coordinates.X - Parent.Camera.Coordinates.X) * Parent.Settings.ChunkSize.Width,
                            (character.Coordinates.Y - Parent.Camera.Coordinates.Y) * Parent.Settings.ChunkSize.Height));
                }
            }
            Monitor.Exit(Parent.Actors);
        }

        private void RepaintOldChunks()
        {
            Monitor.Enter(oldChunks);
            foreach (Point point in oldChunks)
            {
                if ((point.X >= 0) && (point.X <= Parent.Map.Width) &&
                    (point.Y >= 0) && (point.Y <= Parent.Map.Height))
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
                new Point(
                    (point.X - Parent.Camera.Coordinates.X) * Parent.Settings.ChunkSize.Width,
                    (point.Y - Parent.Camera.Coordinates.Y) * Parent.Settings.ChunkSize.Height));
        }

        public void AddOldChunk(Point point)
        {
            Monitor.Enter(oldChunks);
            if (!oldChunks.Contains(point)) oldChunks.Add(point);
            Monitor.Exit(oldChunks);
        }
    }
}
