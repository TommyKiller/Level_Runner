using System.Drawing;
using System.Windows.Forms;

namespace LevelRunner.GameWorld
{
    public static class GameSettings
    {
        // Events
        public static Delegates.EventDelegate SettingsChanged;

        // Fields
        private static FormBorderStyle s_formBorderStyle = FormBorderStyle.None;

        // Properties
        public static int TimerInterval { get; set; } = 1;
        public static Size ChunkSize { get; set; } = new Size(18, 24);
        public static float VolumeLevel { get; set; } = (float)0.05;
        public static Color PlayerColor { get; set; } = Color.LightPink;
        public static FormBorderStyle FormBorderStyle
        {
            get => s_formBorderStyle;
            set
            {
                s_formBorderStyle = value;
                SettingsChanged();
            }
        }
    }
}
