using LevelRunner.Actors.AttackTypes;
using LevelRunner.Mathematics;
using LevelRunner.Properties;
using System;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Actors.NPC
{
    class AIArcher : NPC
    {
        public AIArcher(World parent, Fraction.Fractions fraction, Point coordinates)
            : base(parent, fraction, coordinates, Resources.AIArcher)
        {
            #region Characteristics
            Name = FractionName.ToString() + " Archer";
            UnitType = UnitTypes.GroundUnit;
            Health = 80;
            Speed = 3;
            UnitAttack = new GroundAndAir(9, 0.8, 8);
            SightRange = 80;
            #endregion

            SetUpTimers(UnitAttack.AttackSpeed * 1000, 1000 / Speed);
        }

        public AIArcher(World parent, Fraction.Fractions fraction, Point coordinates, Bitmap image)
            : base(parent, fraction, coordinates, image)
        {
            #region Characteristics
            Name = FractionName.ToString() + " Archer";
            UnitType = UnitTypes.GroundUnit;
            Health = 80;
            Speed = 3;
            UnitAttack = new GroundAndAir(9, 0.8, 8);
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
            Parent.Actors.Add(new AIArcher(Parent, FractionName, Calculate.GetRandomFreePoint(UnitType)));
            Monitor.Exit(Parent.Actors);
        }
    }
}
