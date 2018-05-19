
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
                Monitor.Enter(Program.GameClient.scene);
                Program.GameClient.scene.AddOldChunk(Coordinates);
                Monitor.Exit(Program.GameClient.scene);

                Monitor.Enter(Program.GameClient.map);
                Program.GameClient.map.Patency[coordinates.Y, coordinates.X] = 0;
                coordinates = value;
                Program.GameClient.map.Patency[coordinates.Y, coordinates.X] = 1;
                Monitor.Exit(Program.GameClient.map);

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
                    if (Program.GameClient.debug)
                    {
                        Program.GameClient.wounded++;
                        Console.WriteLine("{0} deadly wounded", Name);
                        Console.WriteLine(Thread.CurrentThread.Name);
                    }

                    Dispose();
                }
            }
        }
        protected int Damage { get; }
        protected double AttackSpeed { get; }
        protected string AttackType { get; }
        protected double AttackRange { get; }
        protected int Speed { get; }
        protected int SightRange { get; }

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
            if (Program.GameClient.debug)
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

                Monitor.Enter(Program.GameClient.actors);
                Program.GameClient.actors.Remove(this);
                Monitor.Exit(Program.GameClient.actors);

                DeleteTargetEvent(this);

                Monitor.Enter(Program.GameClient.scene);
                Program.GameClient.scene.AddOldChunk(new Point(X, Y));
                Monitor.Exit(Program.GameClient.scene);

                Monitor.Enter(Program.GameClient.map);
                Program.GameClient.map.Patency[Y, X] = 0;
                Monitor.Exit(Program.GameClient.map);

                OnDeath();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            disposed = true;
        }

        protected virtual void OnDeath()
        {
            // Debugging
            if (Program.GameClient.debug)
            {
                Program.GameClient.died++;
                Console.WriteLine("{0} died", Name);
            }

            CoolDownTimer.Elapsed -= CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed -= MovementSpeedTimer_Elapsed;
            DeleteTargetEvent -= DeleteTarget;
        }

        ~Character()
        {
            Dispose(false);
        }
    }
}
