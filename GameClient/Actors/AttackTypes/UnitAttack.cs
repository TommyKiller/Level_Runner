using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
