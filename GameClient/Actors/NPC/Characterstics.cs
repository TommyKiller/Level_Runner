using LevelRunner.Actors.AttackTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Actors.NPC
{
    public static class Archer
    {
        public static Bitmap Image { get; } = new Bitmap(@"Resources\Assets\Characters\AIArcher.png");
        public static UnitTypes UnitType { get; } = UnitTypes.GroundUnit;
        public static int Health { get; } = 80;
        public static double Speed { get; } = 3;
        public static UnitAttack UnitAttack { get; } = new GroundAndAir(9, 0.8, 8);
        public static int SightRange { get; } = 80;
    }

    public static class Warrior
    {
        public static Bitmap Image { get; } = new Bitmap(@"Resources\Assets\Characters\AIWarrior.png");
        public static UnitTypes UnitType { get; } = UnitTypes.GroundUnit;
        public static int Health { get; } = 120;
        public static double Speed { get; } = 1;
        public static UnitAttack UnitAttack { get; } = new GroundOnly(7, 1.1, 1.5);
        public static int SightRange { get; } = 200;
    }
}
