using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.Mathematics;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LevelRunner.Actors
{
    class Player : Character, IDisposable
    {
        // Propereties
        public Vector Direction { get; set; }
        public override Point Coordinates
        {
            get => base.Coordinates;
            protected set
            {
                if (CanMove)
                {
                    base.Coordinates = value;
                    OnMove();
                }
            }
        }

        public Player(World parent, Fraction fraction, Point coordinates, string name)
            : base(parent, fraction, coordinates, Resources.PWarrior)
        {
            #region Characteristics
            Name = name;
            UnitType = UnitTypes.GroundUnit;
            Health = 120;
            Speed = 3.5;
            UnitAttack = new GroundOnly(10, 1, 1.5);
            #endregion

            SetUpTimers(UnitAttack.AttackSpeed * 1000, 1000 / Speed);

            #region Events
            CoolDownTimer.Elapsed += CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed += MovementSpeedTimer_Elapsed;
            Parent.OnTimer += Action_Execute;
            Parent.KeyDown += Player_StartMoving;
            #endregion
        }

        protected override void Action_Execute()
        {
            if (ActionStack.Count > 0)
            {
                ActionThread = new Thread(new ThreadStart(ActionStack.Pop()))
                {
                    Name = Name,
                    IsBackground = false
                };
                ActionThread.Start();
            }
        }

        protected void Action_Move()
        {
            #region Set direction vector
            Direction = new Vector(0, 0);
            if (UserControl.KeyPressed(Keys.Right))
            {
                Direction.X += 1;
            }
            if (UserControl.KeyPressed(Keys.Left))
            {
                Direction.X -= 1;
            }
            if (UserControl.KeyPressed(Keys.Down))
            {
                Direction.Y += 1;
            }
            if (UserControl.KeyPressed(Keys.Up))
            {
                Direction.Y -= 1;
            }
            #endregion

            Point newPoint = Direction.GetEndPoint(Coordinates);
            if (Calculate.CheckPoint(newPoint, UnitType))
            {
                Coordinates = newPoint;
            }
        }

        protected void Player_StartMoving(object sender, KeyEventArgs e)
        {
            if ((UserControl.KeyPressed(Keys.Right)) || (UserControl.KeyPressed(Keys.Left)) ||
                (UserControl.KeyPressed(Keys.Up)) || (UserControl.KeyPressed(Keys.Down)))
            {
                if (ActionStack.Count > 0)
                {
                    ActionStack.Clear();
                }
                ActionStack.Push(Action_Move);
            }
        }

        protected override void OnDeath()
        {
            #region Unsubscribe events
            CoolDownTimer.Elapsed -= CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed -= MovementSpeedTimer_Elapsed;
            Parent.OnTimer -= Action_Execute;
            Parent.KeyDown -= Player_StartMoving;
            #endregion

            RespawnCharacter();
        }

        protected void RespawnCharacter()
        {
            #region Debugging
            if (Parent.debug)
            {
                Parent.respawned++;
                Console.WriteLine("{0} respawned", Name);
            }
            #endregion

            Monitor.Enter(Parent.Actors);
            Parent.Actors.Add(new Player(Parent, Fraction, Calculate.GetRandomFreePoint(UnitType), Name));
            Monitor.Exit(Parent.Actors);
        }
    }
}
