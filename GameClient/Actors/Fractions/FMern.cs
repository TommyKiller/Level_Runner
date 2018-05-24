using System.Collections.Generic;
using System.Drawing;

namespace LevelRunner.Actors.Fractions
{
    public class FMern : Fraction
    {
        public override string Name { get; }
        public override Color Color { get; }
        public override Dictionary<string, Relations> RelationsList { get; }

        public FMern()
        {
            Name = "Mern";
            Color = Color.AliceBlue;
            RelationsList = new Dictionary<string, Relations>
            {
                { "Rivia", Relations.Hostile }
            };
        }
    }
}
