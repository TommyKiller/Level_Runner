using LevelRunner.Actors;
using LevelRunner.GameWorld.Map;
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
            // Absolute coordinates in camera area
            for (int i = Parent.Camera.Coordinates.X; i < Parent.Camera.Coordinates.X + Parent.Camera.Size.Width; i++)
            {
                for (int j = Parent.Camera.Coordinates.Y; j < Parent.Camera.Coordinates.Y + Parent.Camera.Size.Height; j++)
                {
                    // Absolute coordinates in map area
                    if ((i >= 0) && (i < Parent.Map.Width) &&
                        (j >= 0) && (j < Parent.Map.Height))
                    {
                        // Reduced coordinates (camera related)
                        Bitmap chunk = new Bitmap(Terrain.TerrainImage[Parent.Map.TerrainLayer[j, i]], Program.Settings.ChunkSize);
                        Parent.Canvas.DrawImage(chunk, new Point(
                        ((i - Parent.Camera.Coordinates.X) * Program.Settings.ChunkSize.Width),
                        ((j - Parent.Camera.Coordinates.Y) * Program.Settings.ChunkSize.Height)));
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
                    // Absolute coordinates in camera area
                    if ((character.Coordinates.X >= Parent.Camera.Coordinates.X) &&
                        (character.Coordinates.X < Parent.Camera.Coordinates.X + Parent.Camera.Size.Width) &&
                        (character.Coordinates.Y >= Parent.Camera.Coordinates.Y) &&
                        (character.Coordinates.Y < Parent.Camera.Coordinates.Y + Parent.Camera.Size.Height))
                    {
                        // Reduced coordinates (camera related)
                        Bitmap chunk = new Bitmap(character.Image, Program.Settings.ChunkSize);
                        Parent.Canvas.DrawImage(chunk, new Point(
                               (character.Coordinates.X - Parent.Camera.Coordinates.X) * Program.Settings.ChunkSize.Width,
                               (character.Coordinates.Y - Parent.Camera.Coordinates.Y) * Program.Settings.ChunkSize.Height));
                    }
                }
            }
            Monitor.Exit(Parent.Actors);
        }

        private void RepaintOldChunks()
        {
            Monitor.Enter(oldChunks);
            foreach (Point point in oldChunks)
            {
                // Absolute coordinates in camera area
                if ((point.X >= Parent.Camera.Coordinates.X) &&
                    (point.X < Parent.Camera.Coordinates.X + Parent.Camera.Size.Width) &&
                    (point.Y >= Parent.Camera.Coordinates.Y) &&
                    (point.Y < Parent.Camera.Coordinates.Y + Parent.Camera.Size.Height))
                {
                    // Reduced coordinates (camera related)
                    Bitmap chunk = new Bitmap(Terrain.TerrainImage[Parent.Map.TerrainLayer[point.Y, point.X]], Program.Settings.ChunkSize);
                    Parent.Canvas.DrawImage(chunk, new Point(
                            (point.X - Parent.Camera.Coordinates.X) * Program.Settings.ChunkSize.Width,
                            (point.Y - Parent.Camera.Coordinates.Y) * Program.Settings.ChunkSize.Height));
                }
            }
            oldChunks.Clear();
            Monitor.Exit(oldChunks);
        }

        public void AddOldChunk(Point point)
        {
            // Absolute coordinates (map related)
            if ((point.X >= 0) && (point.X <= Parent.Map.Width) &&
                (point.Y >= 0) && (point.Y <= Parent.Map.Height))
            {
                Monitor.Enter(oldChunks);
                if (!oldChunks.Contains(point)) oldChunks.Add(point);
                Monitor.Exit(oldChunks);
            }
        }
    }
}
