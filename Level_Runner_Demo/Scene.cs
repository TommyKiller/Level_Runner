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
        private static Graphics Canvas { get; set; }
        private bool Saved { get; set; }
        public Point Coordinates { get; set; }
        public Size Size { get; set; }

        public Scene(Point coordinates, Size size, Form parentForm)
        {
            Coordinates = coordinates;
            Size = size;
            Canvas = parentForm.CreateGraphics();
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
                    Canvas.DrawImage(GameClient.terrainImageList[GameClient.map.Terrain[j, i]], new Point((i * GameClient.settings.chunkSize.Width), (j * GameClient.settings.chunkSize.Height)));
                }
            }
        }

        private void RepaintOldChunks()
        {
            Monitor.Enter(oldChunks);
            foreach (Point point in oldChunks)
            {
                if ((point.X < GameClient.map.Width) && (point.Y < GameClient.map.Height) &&
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
            Canvas.DrawImage(GameClient.terrainImageList[GameClient.map.Terrain[point.Y, point.X]],
                new Point(point.X * GameClient.settings.chunkSize.Width, point.Y * GameClient.settings.chunkSize.Height));
        }

        public void AddOldChunk(Point point)
        {
            Monitor.Enter(oldChunks);
            if (!oldChunks.Contains(point)) oldChunks.Add(point);
            Monitor.Exit(oldChunks);
        }

        private void RepaintActors()
        {
            Monitor.Enter(GameClient.actors);
            if (GameClient.actors.Count > 0)
            {
                foreach (Character character in GameClient.actors)
                {
                    if ((character.X >= Coordinates.X) &&
                        (character.X <= Size.Width) &&
                        (character.Y >= Coordinates.Y) &&
                        (character.Y <= Size.Height))
                        Canvas.DrawImage(character.image, new Point(character.X * GameClient.settings.chunkSize.Width,
                            character.Y * GameClient.settings.chunkSize.Height));
                }
            }
            Monitor.Exit(GameClient.actors);
        }
    }
}
