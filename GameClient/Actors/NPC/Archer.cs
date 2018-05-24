using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Actors.NPC
{
    class AIArcher : NPC
    {
        // Propereties
        public override string Name { get; }

        public AIArcher(World parent, Fraction fraction, Point coordinates)
            : base(parent, fraction, UnitTypes.GroundUnit, new GroundAndAir(8, 0.8, 8), 35, 2, 80, coordinates, Resources.AIArcher)
        {
            Name = Fraction.Name + " Archer";
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
            Parent.Actors.Add(new AIArcher(Parent, Fraction, Mathematics.GetRandomFreePoint(UnitType)));
            Monitor.Exit(Parent.Actors);
        }
    }
}
