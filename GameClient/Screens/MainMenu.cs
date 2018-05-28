using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LevelRunner.Properties;
using LevelRunner.GameWorld;

namespace LevelRunner
{
    public partial class MainMenu : Form
    {
        private WaveOut WaveOut { get; set; }
        private WaveFileReader WaveFileReader { get; set; }
        private List<UnmanagedMemoryStream> TrackList { get; set; }
        private List<UnmanagedMemoryStream> ListenedTracks { get; set; }

        public MainMenu()
        {
            InitializeComponent();

            // Form settings
            FormBorderStyle = GameSettings.FormBorderStyle;

            ListenedTracks = new List<UnmanagedMemoryStream>();
            TrackList = new List<UnmanagedMemoryStream>
            {
                Resources.MainTheme,
                Resources.Forever,
                Resources.Divinitus,
                Resources.Epic
            };

            // Events
            GameSettings.SettingsChanged += GameSettings_OnChange;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            ChooseTrack(out UnmanagedMemoryStream Track);
            StartWaveOut(Track);
        }

        private void GameSettings_OnChange()
        {
            FormBorderStyle = GameSettings.FormBorderStyle;
        }

        private void StartWaveOut(UnmanagedMemoryStream track)
        {
            if (WaveOut == null)
            {
                WaveOut = new WaveOut();
                WaveFileReader = new WaveFileReader(track);
                WaveOut.Init(WaveFileReader);
                WaveOut.Volume = (float)0.07;
                WaveOut.Play();
                WaveOut.PlaybackStopped += WaveOut_OnPlaybackStopped;
            }
            else
            {
                WaveFileReader = new WaveFileReader(track);
                WaveOut.Init(WaveFileReader);
                WaveOut.Play();
            }
        }

        private void ChooseTrack(out UnmanagedMemoryStream Track)
        {
            if (ListenedTracks.Count == TrackList.Count)
            {
                ListenedTracks.Clear();
                ChooseTrack(out Track);
            }
            else
            {
                Random random = new Random();
                List<UnmanagedMemoryStream> TempTrackList = new List<UnmanagedMemoryStream>();
                foreach(UnmanagedMemoryStream track in TrackList)
                {
                    if (!ListenedTracks.Contains(track))
                    {
                        TempTrackList.Add(track);
                    }
                }
                int TrackIndex = random.Next(TempTrackList.Count);
                Track = TempTrackList[TrackIndex];
            }
        }

        private void WaveOut_OnPlaybackStopped(object sender, EventArgs e)
        {
            ChooseTrack(out UnmanagedMemoryStream Track);
            StartWaveOut(Track);
        }

        private void CloseWaveOut()
        {
            if (WaveOut != null)
            {
                WaveOut.Stop();
            }
            if (WaveFileReader != null)
            {
                WaveFileReader.Dispose();
                WaveFileReader = null;
            }
            if (WaveOut != null)
            {
                WaveOut.Dispose();
                WaveOut = null;
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            Program.World = new World();
            Program.Context.MainForm = Program.World;
            Close();
            Program.Context.MainForm.Show();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            About About = new About();
            About.Show(this);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            Settings Settings = new Settings();
            Settings.Show(this);
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Events
            GameSettings.SettingsChanged -= GameSettings_OnChange;

            CloseWaveOut();
        }
    }
}
