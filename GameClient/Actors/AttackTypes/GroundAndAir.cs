using System.Collections.Generic;

namespace LevelRunner.Actors.AttackTypes
{
    public class GroundAndAir : UnitAttack
    {
        public override List<UnitTypes> EligibleTargets { get; }

        public GroundAndAir(int damage, double attackSpeed, double attackRange) : base(damage, attackSpeed, attackRange)
        {
            EligibleTargets = new List<UnitTypes>
            {
                UnitTypes.GroundUnit,
                UnitTypes.AirUnit
            };
        }
    }
}
