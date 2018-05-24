using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Actors.NPC
{
    class AIWarrior : NPC
    {
        // Propereties
        public override string Name { get; }

        public AIWarrior(World parent, Fraction fraction, Point coordinates)
            : base(parent, fraction, UnitTypes.GroundUnit, new GroundOnly(9, 1.1, 1.5), 45, 1, 80, coordinates, Resources.AIWarrior)
        {
            Name = Fraction.Name + " Warrior";
        }

        protected override void DealDamage()
        {
            if (Target != null)
            {
                Target.Health = -UnitAttack.Damage;
            }
        }

        protected override void RespawnCharacter()
        {
            #region Debugging
            if (Parent.debug)
            {
                Parent.respawned++;
                Console.WriteLine("{0} respawned", Name);
            }
            #endregion

            Monitor.Enter(Parent.Actors);
            Parent.Actors.Add(new AIWarrior(Parent, Fraction, Mathematics.GetRandomFreePoint(UnitType)));
            Monitor.Exit(Parent.Actors);
        }
    }
}
