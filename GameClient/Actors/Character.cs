using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.GameWorld.Map;

namespace LevelRunner.Actors
{
    public abstract class Character : IDisposable
    {
        // Fields
        private bool disposed = false;
        private int _health;
        private Point _coordinates;

        // Propereties
        protected World Parent { get; }
        public UnitTypes UnitType { get; }
        public Fraction Fraction { get; }
        public int Health
        {
            get => _health;
            set
            {
                _health += value;
                if ((Alive) && (_health <= 0))
                {
                    #region Debugging
                    if (Program.World.debug)
                    {
                        Program.World.wounded++;
                        Console.WriteLine("{0} deadly wounded", Name);
                        Console.WriteLine(Thread.CurrentThread.Name);
                    }
                    #endregion

                    Dispose();
                }
            }
        }
        protected int Speed { get; }
        public UnitAttack UnitAttack { get; }
        protected int SightRange { get; }
        public virtual Point Coordinates
        {
            get => _coordinates;
            protected set
            {
                Monitor.Enter(Parent.Scene);
                Parent.Scene.AddOldChunk(Coordinates);
                Monitor.Exit(Parent.Scene);

                Monitor.Enter(Parent.Map);
                Parent.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Free;
                _coordinates = value;
                Parent.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Occupied;
                Monitor.Exit(Parent.Map);
            }
        }
        public Bitmap Image { get; }
        public abstract string Name { get; }
        protected bool Alive { get; set; }
        public Thread CharacterThread { get; }
        protected Stack<Delegates.ActDelegate> ActionStack { get; set; }

        public Character(World parent, Fraction fraction, UnitTypes unitType, UnitAttack unitAttack,
            int health, int speed, int sightRange, Point coordinates, Bitmap image)
        {
            Parent = parent;
            Fraction = fraction;
            UnitType = unitType;
            Health = health;
            Speed = speed;
            UnitAttack = unitAttack;
            SightRange = sightRange;
            Coordinates = coordinates;

            Image = image;
            Image.MakeTransparent(Color.White);

            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    Console.WriteLine(Image.GetPixel(i, j));
                    if (Image.GetPixel(i, j) == Color.FromArgb(255, 255, 0, 0))
                    {
                        Image.SetPixel(i, j, Fraction.Color);
                    }
                }
                
            }

            CharacterThread = new Thread(Action_Execute)
            {
                Name = Name,
                IsBackground = false
            };
            ActionStack = new Stack<Delegates.ActDelegate>();
        }

        protected abstract void Action_Execute();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Alive = false;

                Monitor.Enter(Parent.Actors);
                Parent.Actors.Remove(this);
                Monitor.Exit(Parent.Actors);

                Monitor.Enter(Parent.Scene);
                Parent.Scene.AddOldChunk(new Point(Coordinates.X, Coordinates.Y));
                Monitor.Exit(Parent.Scene);

                Monitor.Enter(Parent.Map);
                Parent.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Free;
                Monitor.Exit(Parent.Map);

                OnDeath();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            disposed = true;
        }

        protected abstract void OnDeath();
    }
}
