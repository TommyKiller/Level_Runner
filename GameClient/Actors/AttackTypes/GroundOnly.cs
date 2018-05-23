using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Actors.AttackTypes
{
    public class GroundOnly : UnitAttack
    {
        public override List<UnitTypes> EligibleTargets { get; }

        public GroundOnly(int damage, double attackSpeed, double attackRange) : base(damage, attackSpeed, attackRange)
        {
            EligibleTargets = new List<UnitTypes>
            {
                UnitTypes.GroundUnit
            };
        }
    }
}
