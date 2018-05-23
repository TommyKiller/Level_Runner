using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Actors.Fractions
{
    class FRivia : Fraction
    {
        public override string Name { get; }
        public override Color Color { get; }
        public override Dictionary<string, Relations> RelationsList { get; }

        public FRivia()
        {
            Name = "Rivia";
            Color = Color.Cyan;
            RelationsList = new Dictionary<string, Relations>
            {
                { "Mern", Relations.Hostile }
            };
        }
    }
}
