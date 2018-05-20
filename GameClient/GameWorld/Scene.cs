using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LevelRunner.Actors;

namespace LevelRunner.GameWorld
{
    public class Scene
    {
        // Components
        private List<Point> oldChunks;

        // Propereties
        public World Parent { get; }
        private bool Saved { get; set; }
        public Point Coordinates { get; private set; }
        public Size Size { get; private set; }
        private GraphicsContainer CanvasBackgroundSave { get; set; }

        public Scene(Point coordinates, Size size, World parent)
        {
            Coordinates = coordinates;
            Size = size;
            Parent = parent;
            Parent.Canvas = Parent.CreateGraphics();
            oldChunks = new List<Point>();
            Saved = false;
        }

        public void Repaint()
        {
            if (!Saved)
            {
                CanvasBackgroundSave = Parent.Canvas.BeginContainer();
                RepaintBackGround();
                Saved = true;
            }
            RepaintOldChunks();
            Parent.Canvas.EndContainer(CanvasBackgroundSave);
            RepaintActors();
        }

        private void RepaintBackGround()
        {
            for (int i = Coordinates.X; i < (Coordinates.X + Size.Width); i++)
            {
                for (int j = Coordinates.Y; j < (Coordinates.Y + Size.Height); j++)
                {
                    Parent.Canvas.DrawImage(Parent.Map.Terrain[j, i].Image, new Point((i * Parent.Settings.ChunkSize.Width), (j * Parent.Settings.ChunkSize.Height)));
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
                    if ((character.X >= Coordinates.X) &&
                        (character.X <= Size.Width) &&
                        (character.Y >= Coordinates.Y) &&
                        (character.Y <= Size.Height))
                        Parent.Canvas.DrawImage(character.Image, new Point(character.X * Parent.Settings.ChunkSize.Width,
                            character.Y * Parent.Settings.ChunkSize.Height));
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
            Parent.Canvas.DrawImage(Parent.Map.Terrain[point.Y, point.X].Image,
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
