
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

        // Data
        public string name;
        public string fraction;
        private Point coordinates;
        public Point Coordinates
        {
            get
            {
                return coordinates;
            }
            set
            {
                Monitor.Enter(GameClient.scene);
                GameClient.scene.AddOldChunk(Coordinates);
                Monitor.Exit(GameClient.scene);

                Monitor.Enter(GameClient.map);
                GameClient.map.Patency[coordinates.Y, coordinates.X] = 0;
                coordinates = value;
                GameClient.map.Patency[coordinates.Y, coordinates.X] = 1;
                Monitor.Exit(GameClient.map);

                if (destinationReached) OnMove();
            }
        }
        protected Point destination;
        protected bool destinationReached;
        protected bool canAttack;
        protected bool canMove;
        private bool alive;
        public readonly Bitmap image;
        protected Character target;
        protected Queue<Point> path;
        protected Stack<Delegates.ActDelegate> actionStack;
        protected System.Timers.Timer coolDownTimer;
        protected System.Timers.Timer movementSpeedTimer;
        public Thread characterThread;

        // Characteristics
        public int X
        {
            get
            {
                return Coordinates.X;
            }
        }
        public int Y
        {
            get
            {
                return Coordinates.Y;
            }
        }
        protected readonly Characteristics characteristics;
        private int health;
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health += value;
                if ((alive) && (health <= 0))
                {
                    // Debugging
                    if (GameClient.debug)
                    {
                        GameClient.wounded++;
                        Console.WriteLine("{0} deadly wounded", name);
                        Console.WriteLine(Thread.CurrentThread.Name);
                    }

                    Dispose();
                }
            }
        }
        protected int Damage
        {
            get
            {
                return characteristics.damage;
            }
        }
        protected double AttackSpeed
        {
            get
            {
                return characteristics.attackSpeed;
            }
        }
        protected string AttackType
        {
            get
            {
                return characteristics.attackType;
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

            this.name = name;
            this.fraction = fraction;
            this.image = image;
            this.image.MakeTransparent(Color.White);
            Coordinates = coordinates;
            canAttack = true;
            canMove = true;
            alive = true;
            path = new Queue<Point>();
            actionStack = new Stack<Delegates.ActDelegate>();
            characterThread = new Thread(Action_Execute)
            {
                Name = name,
                IsBackground = false
            };
            coolDownTimer = new System.Timers.Timer
            {
                Interval = AttackSpeed * 1000,
                Enabled = false,
                AutoReset = false
            };
            movementSpeedTimer = new System.Timers.Timer
            {
                Interval = Convert.ToInt32(Math.Floor((double)(1000 / Speed))),
                Enabled = false,
                AutoReset = false
            };

            // Events
            coolDownTimer.Elapsed += CoolDownTimer_Elapsed;
            movementSpeedTimer.Elapsed += MovementSpeedTimer_Elapsed;
            DeleteTargetEvent += DeleteTarget;
        }

        protected void CoolDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            coolDownTimer.Stop();
            canAttack = true;
        }

        protected void MovementSpeedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            movementSpeedTimer.Stop();
            canMove = true;
        }

        protected void Action_Execute()
        {
            while (alive)
            {
                if (actionStack.Count == 0) actionStack.Push(Delegates.CurrentAct = Action_Guard);
                actionStack.Pop()();
            }
        }

        protected abstract void Action_Guard();

        protected abstract void Action_Attack();

        protected abstract void Action_Move();

        protected void OnAttack()
        {
            canAttack = false;
            coolDownTimer.Start();
        }

        protected void OnMove()
        {
            canMove = false;
            destinationReached = false;
            movementSpeedTimer.Start();
        }

        protected void DealDamage()
        {
            if (target != null) target.Health = -Damage;
        }

        public void DeleteTarget(Character target)
        {
            // Debug
            if (GameClient.debug)
            {
                Console.WriteLine("Delete target for {0} is managed by {1}", name, Thread.CurrentThread.Name);
            }

            if (this.target == target) this.target = null;
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
                characterThread.Abort();
                alive = false;

                Monitor.Enter(GameClient.actors);
                GameClient.actors.Remove(this);
                Monitor.Exit(GameClient.actors);

                DeleteTargetEvent(this);

                Monitor.Enter(GameClient.scene);
                GameClient.scene.AddOldChunk(new Point(X, Y));
                Monitor.Exit(GameClient.scene);

                Monitor.Enter(GameClient.map);
                GameClient.map.Patency[Y, X] = 0;
                Monitor.Exit(GameClient.map);

                OnDeath();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            disposed = true;
        }

        protected virtual void OnDeath()
        {
            // Debugging
            if (GameClient.debug)
            {
                GameClient.died++;
                Console.WriteLine("{0} died", name);
            }

            coolDownTimer.Elapsed -= CoolDownTimer_Elapsed;
            movementSpeedTimer.Elapsed -= MovementSpeedTimer_Elapsed;
            DeleteTargetEvent -= DeleteTarget;
        }

        ~Character()
        {
            Dispose(false);
        }
    }
}
