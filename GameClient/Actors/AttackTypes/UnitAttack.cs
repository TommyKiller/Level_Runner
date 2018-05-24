using System.Collections.Generic;

namespace LevelRunner.Actors.AttackTypes
{
    public abstract class UnitAttack
    {
        public int Damage { get; }
        public double AttackSpeed { get; }
        public double AttackRange { get; }
        public abstract List<UnitTypes> EligibleTargets { get; }

        public UnitAttack(int damage, double attackSpeed, double attackRange)
        {
            Damage = damage;
            AttackSpeed = attackSpeed;
            AttackRange = attackRange;
        }
    }
}
