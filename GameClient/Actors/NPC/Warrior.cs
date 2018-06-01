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
            : base(parent, fraction, coordinates, Warrior.Image)
        {
            #region Characteristics
            Name = FractionName.ToString() + " Warrior";
            UnitType = Warrior.UnitType;
            Health = Warrior.Health;
            Speed = Warrior.Speed;
            UnitAttack = Warrior.UnitAttack;
            SightRange = Warrior.SightRange;
            #endregion

            SetUpTimers(UnitAttack.AttackSpeed * 1000, 1000 / Speed);
        }

        public AIWarrior(World parent, Fraction.Fractions fraction, Point coordinates,
            string name, int health, double speed, int sightRange, Bitmap image)
            : base(parent, fraction, coordinates, image)
        {
            #region Characteristics
            Name = name;
            UnitType = Warrior.UnitType;
            Health = health;
            Speed = speed;
            UnitAttack = Warrior.UnitAttack;
            SightRange = sightRange;
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
