using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner.Actors.Fractions
{
    public abstract class Fraction
    {
        public abstract string Name { get; }
        public abstract Color Color { get; }
        /// <summary>
        /// Dictionary of fractions' names and relations with these fractions.
        /// </summary>
        public abstract Dictionary<string, Relations> RelationsList { get; }
    }
}
