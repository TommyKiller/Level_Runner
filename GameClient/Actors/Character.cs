using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.GameWorld.Map;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

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
        public string Name { get; protected set; }
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
        public UnitTypes UnitType { get; protected set; }
        public Fraction Fraction { get; }
        public UnitAttack UnitAttack { get; protected set; }
        protected double Speed { get; set; }
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
        protected bool Alive { get; set; }
        protected bool CanAttack { get; set; }
        protected bool CanMove { get; set; }
        protected System.Timers.Timer CoolDownTimer { get; }
        protected System.Timers.Timer MovementSpeedTimer { get; }
        protected Stack<Delegates.ActDelegate> ActionStack { get; }
        protected Thread ActionThread { get; set; }

        public Character(World parent, Fraction fraction, Point coordinates, Bitmap image)
        {
            Parent = parent;
            Fraction = fraction;
            _coordinates = coordinates; // !!!
            
            CoolDownTimer = new System.Timers.Timer();
            MovementSpeedTimer = new System.Timers.Timer();
            ActionStack = new Stack<Delegates.ActDelegate>();

            #region Image editing
            Image = image;
            Image.MakeTransparent(Color.White);
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    if (Image.GetPixel(i, j) == Color.FromArgb(255, 255, 0, 0))
                    {
                        Image.SetPixel(i, j, Fraction.Color);
                    }
                }
            }
            #endregion

            CanAttack = true;
            CanMove = true;
            Alive = true;
        }

        protected void SetUpTimers(double coolDownInterval, double movementSpeedInterval)
        {
            #region Attack cool down timer
            CoolDownTimer.Interval = coolDownInterval;
            CoolDownTimer.Enabled = false;
            CoolDownTimer.AutoReset = false;
            #endregion

            #region Movement cool down timer
            MovementSpeedTimer.Interval = movementSpeedInterval;
            MovementSpeedTimer.Enabled = false;
            MovementSpeedTimer.AutoReset = false;
            #endregion
        }

        protected abstract void Action_Execute();

        #region Dispose
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
        #endregion

        #region Events
        protected void CoolDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CoolDownTimer.Stop();
            CanAttack = true;
        }

        protected void MovementSpeedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MovementSpeedTimer.Stop();
            CanMove = true;
        }

        protected void OnAttack()
        {
            CanAttack = false;
            CoolDownTimer.Start();
        }

        protected void OnMove()
        {
            CanMove = false;
            MovementSpeedTimer.Start();
        }

        protected abstract void OnDeath();
        #endregion
    }
}
