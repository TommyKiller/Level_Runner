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
        private Dictionary<UnmanagedMemoryStream, bool> TrackList { get; set; }

        public MainMenu()
        {
            InitializeComponent();

            // Form settings
            FormBorderStyle = GameSettings.FormBorderStyle;

            TrackList = new Dictionary<UnmanagedMemoryStream, bool>
            {
                { Resources.MainTheme, true },
                { Resources.Forever, true },
                { Resources.Divinitus, true },
                { Resources.Epic, true }
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
            int listenedTracks = 0;
            UnmanagedMemoryStream chosenTrack = null;

            foreach (UnmanagedMemoryStream track in TrackList.Keys)
            {
                if (TrackList[track])
                {
                    TrackList[track] = false;
                    chosenTrack = track;
                    break;
                }
                else
                {
                    listenedTracks++;
                }
            }
            if (listenedTracks == TrackList.Count)
            {
                ReshuffleTracks();
                ChooseTrack(out Track);
            }
            else
            {
                Track = chosenTrack;
            }
        }

        private void ReshuffleTracks()
        {
            Dictionary<UnmanagedMemoryStream, bool> TempTrackList = new Dictionary<UnmanagedMemoryStream, bool>();
            foreach (UnmanagedMemoryStream track in TrackList.Keys)
            {
                TempTrackList.Add(track, true);
            }
            TrackList = TempTrackList;
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
