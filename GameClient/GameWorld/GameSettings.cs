using System.Drawing;

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
