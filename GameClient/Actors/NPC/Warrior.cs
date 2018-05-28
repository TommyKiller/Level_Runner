using LevelRunner.Actors.AttackTypes;
using LevelRunner.Mathematics;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Actors.NPC
{
    class AIWarrior : NPC
    {
        public AIWarrior(World parent, Fraction.Fractions fraction, Point coordinates)
            : base(parent, fraction, coordinates, Resources.AIWarrior)
        {
            #region Characteristics
            Name = FractionName.ToString() + " Warrior";
            UnitType = UnitTypes.GroundUnit;
            Health = 120;
            Speed = 1;
            UnitAttack = new GroundOnly(7, 1.1, 1.5);
            SightRange = 80;
            #endregion

            SetUpTimers(UnitAttack.AttackSpeed * 1000, 1000 / Speed);
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
            #if DEBUG
                Parent.respawned++;
                Console.WriteLine("{0} respawned", Name);
            #endif
            #endregion

            Monitor.Enter(Parent.Actors);
            Parent.Actors.Add(new AIWarrior(Parent, FractionName, Calculate.GetRandomFreePoint(UnitType)));
            Monitor.Exit(Parent.Actors);
        }
    }
}
