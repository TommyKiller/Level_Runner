using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LevelRunner.GameWorld.Map;

namespace LevelRunner.Actors
{
    public abstract class Character : IDisposable
    {
        // Events
        private static event Delegates.DeleteTargetDelegate DeleteTargetEvent;

        // Propereties
        public UnitType UnitType { get; }
        public string Name { get; }
        public string Fraction { get; }
        private Point coordinates;
        public Point Coordinates
        {
            get => coordinates;
            set
            {
                Monitor.Enter(Program.World.Scene);
                Program.World.Scene.AddOldChunk(Coordinates);
                Monitor.Exit(Program.World.Scene);

                Monitor.Enter(Program.World.Map);
                Program.World.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Free;
                coordinates = value;
                Program.World.Map.PatencyLayer[Coordinates.Y, Coordinates.X].GroundPatency = GroundPatencyMode.Occupied;
                Monitor.Exit(Program.World.Map);

                if (DestinationReached) OnMove();
            }
        }
        protected Point Destination { get; set; }
        protected bool DestinationReached { get; set; }
        protected bool CanAttack { get; set; }
        protected bool CanMove { get; set; }
        private bool Alive { get; set; }
        public Bitmap Image { get; }
        protected Character Target { get; set; }
        protected Stack<Delegates.ActDelegate> ActionStack { get; set; }
        protected System.Timers.Timer CoolDownTimer { get; }
        protected System.Timers.Timer MovementSpeedTimer { get; }
        public Thread CharacterThread { get; }

        // Characteristics
        public int X => Coordinates.X;
        public int Y => Coordinates.Y;
        protected Characteristics characteristics;
        private int health;
        public int Health
        {
            get => health;
            set
            {
                health += value;
                if ((Alive) && (health <= 0))
                {
                    // Debugging
                    if (Program.World.debug)
                    {
                        Program.World.wounded++;
                        Console.WriteLine("{0} deadly wounded", Name);
                        Console.WriteLine(Thread.CurrentThread.Name);
                    }

                    Dispose();
                }
            }
        }
        protected int Damage => characteristics.damage;
        protected double AttackSpeed => characteristics.attackSpeed;
        protected double AttackRange => characteristics.attackRange;
        protected int Speed => characteristics.speed;
        protected int SightRange => characteristics.sightRange;

        public struct Characteristics
        {
            public readonly int fullHealth;
            public readonly int damage;
            public readonly double attackSpeed;
            public readonly string attackType;
            public readonly double attackRange;
            public readonly int speed;
            public readonly int sightRange;

            public Characteristics(int fullHealth, int damage, double attackSpeed, string attackType, double attackRange, int speed, int sightRange)
            {
                this.fullHealth = fullHealth;
                this.damage = damage;
                this.attackSpeed = attackSpeed;
                this.attackType = attackType;
                this.attackRange = attackRange;
                this.speed = speed;
                this.sightRange = sightRange;
            }
        }

        public Character(string name, Point coordinates, Bitmap image, Characteristics characteristics, string fraction)
        {
            // Characteristics
            UnitType = UnitType.Ground;
            this.characteristics = characteristics;
            Health = characteristics.fullHealth;
            Name = name;
            Fraction = fraction;
            Image = image;
            Image.MakeTransparent(Color.White);
            Coordinates = coordinates;
            CanAttack = true;
            CanMove = true;
            Alive = true;
            ActionStack = new Stack<Delegates.ActDelegate>();
            CharacterThread = new Thread(Action_Execute)
            {
                Name = name,
                IsBackground = false
            };
            CoolDownTimer = new System.Timers.Timer
            {
                Interval = AttackSpeed * 1000,
                Enabled = false,
                AutoReset = false
            };
            MovementSpeedTimer = new System.Timers.Timer
            {
                Interval = 1000 / Speed,
                Enabled = false,
                AutoReset = false
            };

            // Events
            CoolDownTimer.Elapsed += CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed += MovementSpeedTimer_Elapsed;
            DeleteTargetEvent += DeleteTarget;
        }

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

        protected void Action_Execute()
        {
            while (Alive)
            {
                if (ActionStack.Count == 0)
                {
                    ActionStack.Push(Delegates.CurrentAct = Action_Guard);
                }

                ActionStack.Pop()();
            }
        }

        protected abstract void Action_Guard();

        protected abstract void Action_Attack();

        protected abstract void Action_Move();

        protected void OnAttack()
        {
            CanAttack = false;
            CoolDownTimer.Start();
        }

        protected void OnMove()
        {
            CanMove = false;
            DestinationReached = false;
            MovementSpeedTimer.Start();
        }

        protected void DealDamage()
        {
            if (Target != null)
            {
                Target.Health = -Damage;
            }
        }

        public void DeleteTarget(Character target)
        {
            // Debug
            if (Program.World.debug)
            {
                Console.WriteLine("Delete target for {0} is managed by {1}", Name, Thread.CurrentThread.Name);
            }

            if (Target == target)
            {
                Target = null;
            }
        }

        // IDisposable
        bool disposed = false;

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

                Monitor.Enter(Program.World.Actors);
                Program.World.Actors.Remove(this);
                Monitor.Exit(Program.World.Actors);

                DeleteTargetEvent(this);

                Monitor.Enter(Program.World.Scene);
                Program.World.Scene.AddOldChunk(new Point(X, Y));
                Monitor.Exit(Program.World.Scene);

                Monitor.Enter(Program.World.Map);
                Program.World.Map.PatencyLayer[Y, X].GroundPatency = GroundPatencyMode.Free;
                Monitor.Exit(Program.World.Map);

                OnDeath();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            disposed = true;
        }

        protected virtual void OnDeath()
        {
            // Debugging
            if (Program.World.debug)
            {
                Program.World.died++;
                Console.WriteLine("{0} died", Name);
            }

            CoolDownTimer.Elapsed -= CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed -= MovementSpeedTimer_Elapsed;
            DeleteTargetEvent -= DeleteTarget;
        }

        protected abstract void RespawnCharacter();

        ~Character()
        {
            Dispose(false);
        }
    }
}
