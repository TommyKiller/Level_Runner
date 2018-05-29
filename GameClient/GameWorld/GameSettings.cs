using System.Drawing;
using System.Windows.Forms;

namespace LevelRunner.GameWorld
{
    public static class GameSettings
    {
        // Events
        public static Delegates.FormBorderStyleChangeEventDelegate FormBorderStyleChanged;
        public static Delegates.VolumeChangeEventDelegate VolumeLevelChanged;

        // Fields
        private static FormBorderStyle s_formBorderStyle = FormBorderStyle.None;
        private static float s_volumeLevel = (float)0.05;

        // Properties
        public static int TimerInterval { get; set; } = 1;
        public static Size ChunkSize { get; set; } = new Size(18, 24);
        public static float VolumeLevel
        {
            get => s_volumeLevel;
            set
            {
                if ((value >= 0) && (value <= 1))
                {
                    s_volumeLevel = value;
                    VolumeLevelChanged(value);
                }
            }
        }
        public static Color PlayerColor { get; set; } = Color.LightPink;
        public static FormBorderStyle FormBorderStyle
        {
            get => s_formBorderStyle;
            set
            {
                s_formBorderStyle = value;
                FormBorderStyleChanged(value);
            }
        }
    }
}
