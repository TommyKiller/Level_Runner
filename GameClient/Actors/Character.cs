﻿using LevelRunner.Actors.AttackTypes;
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
        // Events
        public Delegates.EventDelegate CharacterChangedPosition;
        public Delegates.EventDelegate CharacterDied;

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
                    #if DEBUG
                        Program.World.wounded++;
                        Console.WriteLine("{0} deadly wounded", Name);
                        Console.WriteLine(Thread.CurrentThread.Name);
                    #endif
                    #endregion

                    CharacterDied();
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
                if (CanMove)
                {
                    Monitor.Enter(Parent.Scene);
                    Parent.Scene.AddOldChunk(Coordinates);
                    Monitor.Exit(Parent.Scene);

                    Monitor.Enter(Parent.Map);
                    Parent.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Free;
                    _coordinates = value;
                    Parent.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Occupied;
                    Monitor.Exit(Parent.Map);

                    CharacterChangedPosition?.Invoke();
                }
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

            #region Flags
            CanAttack = true;
            CanMove = true;
            Alive = true;
            #endregion

            #region Events
            CharacterDied += Character_OnDeath;
            CharacterChangedPosition += Character_OnMove;
            CoolDownTimer.Elapsed += CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed += MovementSpeedTimer_Elapsed;
            Parent.OnTimer += Action_Execute;
            #endregion
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

        protected abstract void RespawnCharacter();

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
                Monitor.Enter(Parent.Actors);
                Parent.Actors.Remove(this);
                Monitor.Exit(Parent.Actors);

                Monitor.Enter(Parent.Scene);
                Parent.Scene.AddOldChunk(new Point(Coordinates.X, Coordinates.Y));
                Monitor.Exit(Parent.Scene);

                Monitor.Enter(Parent.Map);
                Parent.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Free;
                Monitor.Exit(Parent.Map);
                
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

        protected void Character_OnAttack()
        {
            CanAttack = false;
            CoolDownTimer.Start();
        }

        protected void Character_OnMove()
        {
            CanMove = false;
            MovementSpeedTimer.Start();
        }

        protected virtual void Character_OnDeath()
        {
            #region Debugging
            #if DEBUG
                Parent.died++;
                Console.WriteLine("{0} died", Name);
            #endif
            #endregion

            #region Unsubscribe events
            CharacterDied -= Character_OnDeath;
            CharacterChangedPosition -= Character_OnMove;
            CoolDownTimer.Elapsed -= CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed -= MovementSpeedTimer_Elapsed;
            Parent.OnTimer -= Action_Execute;
            #endregion

            Alive = false;
            Dispose();
            RespawnCharacter();
        }
        #endregion
    }
}
