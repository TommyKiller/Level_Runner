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
        public GameClient Parent { get; }
        private Graphics Canvas { get; }
        private bool Saved { get; set; }
        public Point Coordinates { get; private set; }
        public Size Size { get; private set; }

        public Scene(Point coordinates, Size size, GameClient parent)
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
            GameClient.Settings settings = Parent.settings;
            for (int i = Coordinates.X; i < (Coordinates.X + Size.Width); i++)
            {
                for (int j = Coordinates.Y; j < (Coordinates.Y + Size.Height); j++)
                {
                    Canvas.DrawImage(Parent.terrainImageList[Parent.map.Terrain[j, i]], new Point((i * settings.chunkSize.Width), (j * settings.chunkSize.Height)));
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
            GameClient.Settings settings = Parent.settings;
            Canvas.DrawImage(Parent.terrainImageList[Parent.map.Terrain[point.Y, point.X]],
                new Point(point.X * settings.chunkSize.Width, point.Y * settings.chunkSize.Height));
        }

        public void AddOldChunk(Point point)
        {
            Monitor.Enter(oldChunks);
            if (!oldChunks.Contains(point)) oldChunks.Add(point);
            Monitor.Exit(oldChunks);
        }

        private void RepaintActors()
        {
            GameClient.Settings settings = Parent.settings;
            Monitor.Enter(Parent.actors);
            if (Parent.actors.Count > 0)
            {
                foreach (Character character in Parent.actors)
                {
                    if ((character.X >= Coordinates.X) &&
                        (character.X <= Size.Width) &&
                        (character.Y >= Coordinates.Y) &&
                        (character.Y <= Size.Height))
                        Canvas.DrawImage(character.image, new Point(character.X * settings.chunkSize.Width,
                            character.Y * settings.chunkSize.Height));
                }
            }
            Monitor.Exit(Parent.actors);
        }
    }
}
