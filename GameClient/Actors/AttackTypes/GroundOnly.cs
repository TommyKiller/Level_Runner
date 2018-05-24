using System.Collections.Generic;

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
