using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace LevelRunner.GameWorld
{
    [DataContract]
    public class GameSettings
    {
        // Events
        public static Delegates.FormBorderStyleChangeEventDelegate FormBorderStyleChanged;
        public static Delegates.VolumeChangeEventDelegate VolumeLevelChanged;

        // Fields
        private FormBorderStyle s_formBorderStyle;
        private float s_volumeLevel;

        // Properties
        [DataMember]
        public int TimerInterval { get; set; }
        [DataMember]
        public Size ChunkSize { get; set; }
        [DataMember]
        public float VolumeLevel
        {
            get => s_volumeLevel;
            set
            {
                if ((value >= 0) && (value <= 1))
                {
                    s_volumeLevel = value;
                    VolumeLevelChanged?.Invoke(value);
                }
            }
        }
        [DataMember]
        public Color PlayerColor { get; set; }
        [DataMember]
        public FormBorderStyle FormBorderStyle
        {
            get => s_formBorderStyle;
            set
            {
                s_formBorderStyle = value;
                FormBorderStyleChanged?.Invoke(value);
            }
        }

        public GameSettings()
        {

        }

        public void SetDefault()
        {
            TimerInterval = 1;
            ChunkSize = new Size(18, 24);
            VolumeLevel = 0.05f;
            PlayerColor = Color.LightPink;
            FormBorderStyle = FormBorderStyle.None;
        }

        public void Save(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(GameSettings));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, this);
            }
        }

        public bool Load(string path)
        {
            if (File.Exists(path))
            {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(GameSettings));

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    GameSettings settings = (GameSettings)jsonFormatter.ReadObject(fs);
                    TimerInterval = settings.TimerInterval;
                    ChunkSize = settings.ChunkSize;
                    VolumeLevel = settings.VolumeLevel;
                    PlayerColor = settings.PlayerColor;
                    FormBorderStyle = settings.FormBorderStyle;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
