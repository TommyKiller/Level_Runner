using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Level_Runner_Demo
{
    public class Scene
    {
        // Components
        private List<Point> oldChunks;

        // Propereties
        public World Parent { get; }
        private Graphics Canvas { get; }
        private bool Saved { get; set; }
        public Point Coordinates { get; private set; }
        public Size Size { get; private set; }

        public Scene(Point coordinates, Size size, World parent)
        {
            Parent = parent;
            Coordinates = coordinates;
            Size = size;
            Canvas = Parent.CreateGraphics();
            oldChunks = new List<Point>();
            Saved = false;
        }

        public void Repaint()
        {
            if (!Saved)
            {
                RepaintBackGround();
                Saved = true;
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
                    Canvas.DrawImage(Parent.terrainImageList[Parent.map.Terrain[j, i]], new Point((i * Parent.settings.ChunkSize.Width), (j * Parent.settings.ChunkSize.Height)));
                }
            }
        }

        private void RepaintOldChunks()
        {
            Monitor.Enter(oldChunks);
            foreach (Point point in oldChunks)
            {
                if ((point.X < Parent.map.Width) && (point.Y < Parent.map.Height) &&
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
            Canvas.DrawImage(Parent.terrainImageList[Parent.map.Terrain[point.Y, point.X]],
                new Point(point.X * Parent.settings.ChunkSize.Width, point.Y * Parent.settings.ChunkSize.Height));
        }

        public void AddOldChunk(Point point)
        {
            Monitor.Enter(oldChunks);
            if (!oldChunks.Contains(point)) oldChunks.Add(point);
            Monitor.Exit(oldChunks);
        }

        private void RepaintActors()
        {
            Monitor.Enter(Parent.actors);
            if (Parent.actors.Count > 0)
            {
                foreach (Character character in Parent.actors)
                {
                    if ((character.X >= Coordinates.X) &&
                        (character.X <= Size.Width) &&
                        (character.Y >= Coordinates.Y) &&
                        (character.Y <= Size.Height))
                        Canvas.DrawImage(character.Image, new Point(character.X * Parent.settings.ChunkSize.Width,
                            character.Y * Parent.settings.ChunkSize.Height));
                }
            }
            Monitor.Exit(Parent.actors);
        }
    }
}
