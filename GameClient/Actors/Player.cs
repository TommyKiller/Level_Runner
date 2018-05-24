using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Actors
{
    class Player : Character, IDisposable
    {
        // Propereties
        public Delegates.ActDelegate CurrentAct { get; set; }

        public Player(World parent, Fraction fraction, Point coordinates, string name)
            : base(parent, fraction, coordinates, Resources.PWarrior)
        {
            Name = name;
            UnitType = UnitTypes.GroundUnit;
            Health = 120;
            Speed = 1;
            UnitAttack = new GroundOnly(10, 1, 1.5);

            SetUpTimers(UnitAttack.AttackSpeed * 1000, 1000 / Speed);
        }

        protected override void Action_Execute()
        {
            if (CurrentAct != null)
            {
                ActionThread = new Thread(new ThreadStart(CurrentAct))
                {
                    Name = Name,
                    IsBackground = false
                };
                ActionThread.Start();
            }
        }

        protected override void OnDeath()
        {
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
            Parent.Actors.Add(new Player(Parent, Fraction, Mathematics.GetRandomFreePoint(UnitType), Name));
            Monitor.Exit(Parent.Actors);
        }
    }
}
