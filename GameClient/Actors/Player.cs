using LevelRunner.Actors.AttackTypes;
using LevelRunner.Mathematics;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LevelRunner.Actors
{
    public class Player : Character, IDisposable
    {
        // Propereties
        public Vector Direction { get; set; }

        public Player(World parent, Fraction.Fractions fraction, Point coordinates, string name)
            : base(parent, fraction, coordinates, Resources.PWarrior)
        {
            #region Characteristics
            Name = name;
            UnitType = UnitTypes.GroundUnit;
            Health = 120;
            Speed = 5;
            UnitAttack = new GroundOnly(10, 1, 1.5);
            #endregion

            SetUpTimers(UnitAttack.AttackSpeed * 1000, 1000 / Speed);
            Parent.Camera.Bind(this);

            #region Events
            Parent.OnMoveKeyDown += Player_StartMoving;
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

        protected void Player_StartMoving()
        {
            if (ActionStack.Count > 0)
            {
                ActionStack.Clear();
            }
            ActionStack.Push(Action_Move);
        }

        protected override void Character_OnDeath()
        {
            #region Unsubscribe events
            Parent.OnMoveKeyDown -= Player_StartMoving;
            #endregion

            base.Character_OnDeath();

            MessageBox.Show("You died!");
        }

        protected override void RespawnCharacter()
        {
            #region Debugging
            #if DEBUG
                Parent.respawned++;
                Console.WriteLine("{0} respawned", Name);
            #endif
            #endregion

            Monitor.Enter(Parent.Actors);
            Parent.Actors.Add(new Player(Parent, FractionName, Calculate.GetRandomFreePoint(UnitType), Name));
            Monitor.Exit(Parent.Actors);
        }
    }
}
