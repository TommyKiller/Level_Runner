
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Level_Runner_Demo
{
    public abstract class Character : IDisposable
    {
        // Events
        public static event Delegates.DeleteTargetDelegate DeleteTargetEvent;

        // Propereties
        public string Name { get; }
        public string Fraction { get; }
        private Point coordinates;
        public Point Coordinates
        {
            get => coordinates;
            set
            {
                Monitor.Enter(Program.World.scene);
                Program.World.scene.AddOldChunk(Coordinates);
                Monitor.Exit(Program.World.scene);

                Monitor.Enter(Program.World.map);
                Program.World.map.Patency[coordinates.Y, coordinates.X] = 0;
                coordinates = value;
                Program.World.map.Patency[coordinates.Y, coordinates.X] = 1;
                Monitor.Exit(Program.World.map);

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
        public int X { get; }
        public int Y { get; }
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
        protected int Damage { get; }
        protected double AttackSpeed
        {
            get
            {
                return characteristics.attackSpeed;
            }
        }
        protected double AttackRange
        {
            get
            {
                return characteristics.attackRange;
            }
        }
        protected int Speed
        {
            get
            {
                return characteristics.speed;
            }
        }
        protected int SightRange
        {
            get
            {
                return characteristics.sightRange;
            }
        }

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
            this.characteristics = characteristics;
            health = characteristics.fullHealth;

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
                Interval = Convert.ToInt32(Math.Floor((double)(1000 / Speed))),
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
                CharacterThread.Abort();
                Alive = false;

                Monitor.Enter(Program.World.actors);
                Program.World.actors.Remove(this);
                Monitor.Exit(Program.World.actors);

                DeleteTargetEvent(this);

                Monitor.Enter(Program.World.scene);
                Program.World.scene.AddOldChunk(new Point(X, Y));
                Monitor.Exit(Program.World.scene);

                Monitor.Enter(Program.World.map);
                Program.World.map.Patency[Y, X] = 0;
                Monitor.Exit(Program.World.map);

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
