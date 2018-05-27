using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Actors.Fractions
{
    public class FTerronia : Fraction
    {
        public override string Name { get; }
        public override Color Color { get; }
        public override Dictionary<string, Relations> RelationsList { get; }

        public FTerronia()
        {
            Name = "Terronia";
            Color = Color.LightPink;
            RelationsList = new Dictionary<string, Relations>
            {
                { "Rivia", Relations.Hostile },
                { "Mern", Relations.Friendly }
            };
        }
    }
}
