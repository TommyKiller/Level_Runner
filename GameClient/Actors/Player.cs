using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LevelRunner.Actors
{
    class Player : Character, IDisposable
    {
        // Propereties
        public override string Name { get; }

        public Player(World parent, Fraction fraction, Point coordinates)
            : base(parent, fraction, UnitTypes.GroundUnit, new GroundOnly(5, 1, 1.5), 40, 1, 80, coordinates, Resources.PWarrior)
        {
            Name = Fraction.Name + " Player";
        }

        protected override void Action_Execute()
        {
            ActionThread = new Thread(new ThreadStart(ActionStack.Pop()))
            {
                Name = Name,
                IsBackground = false
            };
            ActionThread.Start();
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
            Parent.Actors.Add(new Player(Parent, Fraction, Mathematics.GetRandomFreePoint(UnitType)));
            Monitor.Exit(Parent.Actors);
        }
    }
}
