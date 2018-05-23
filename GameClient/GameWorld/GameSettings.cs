using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.Actors;

namespace LevelRunner.GameWorld
{
    public struct GameSettings
    {
        public int TimerInterval { get; }
        public Size ChunkSize { get; }

        public GameSettings(Size chunkSize, int timerInterval)
        {
            TimerInterval = timerInterval;
            ChunkSize = chunkSize;
        }
    }
}
