using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.Actors;

namespace LevelRunner.GameClient
{
    public class GameSettings
    {
        public int TimerInterval { get; }
        public Size ChunkSize { get; }
        public readonly Dictionary<string, Dictionary<string, string>> relationsTable;
        public Character.Characteristics MeleeDefChars;
        public Character.Characteristics RangeDefChars;

        public GameSettings(Size chunkSize, Character.Characteristics MeleeDefChars, Character.Characteristics RangeDefChars, int timerInterval)
        {
            relationsTable = new Dictionary<string, Dictionary<string, string>>
                {
                    { "A", new Dictionary<string, string>() },
                    { "B", new Dictionary<string, string>() }
                };
            relationsTable["A"].Add("A", "friendly");
            relationsTable["A"].Add("B", "hostile");
            relationsTable["B"].Add("A", "hostile");
            relationsTable["B"].Add("B", "friendly");
            TimerInterval = timerInterval;
            ChunkSize = chunkSize;
            this.MeleeDefChars = MeleeDefChars;
            this.RangeDefChars = RangeDefChars;
        }
    }
}
