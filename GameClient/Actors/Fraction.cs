using System;
using System.Collections.Generic;
using System.Drawing;

namespace LevelRunner.Actors
{
    public static class Fraction
    {
        public enum Fractions
        {
            Mern,
            Rivia,
            Player
        }

        public static Dictionary<Fractions, Color> FractionColor = new Dictionary<Fractions, Color>
        {
            { Fractions.Mern, Color.AliceBlue },
            { Fractions.Rivia, Color.Cyan },
            { Fractions.Player, Program.Settings.PlayerColor }
        };

        public static Dictionary<Fractions, Dictionary<Fractions, Relations>> RelationsList { get; set; } = new Dictionary<Fractions, Dictionary<Fractions, Relations>>
        {
            { Fractions.Mern,
                new Dictionary<Fractions, Relations>
                {
                    { Fractions.Mern, Relations.Friendly},
                    { Fractions.Rivia, Relations.Hostile},
                    { Fractions.Player, Relations.Hostile}
                }
            },
            { Fractions.Rivia,
                new Dictionary<Fractions, Relations>
                {
                    { Fractions.Mern, Relations.Hostile},
                    { Fractions.Rivia, Relations.Friendly},
                    { Fractions.Player, Relations.Friendly}
                }
            },
            { Fractions.Player,
                new Dictionary<Fractions, Relations>
                {
                    { Fractions.Mern, Relations.Hostile},
                    { Fractions.Rivia, Relations.Friendly},
                    { Fractions.Player, Relations.Friendly}
                }
            }
        };

        public enum Relations
        {
            Friendly,
            Hostile,
            Neutral
        }
    }
}
