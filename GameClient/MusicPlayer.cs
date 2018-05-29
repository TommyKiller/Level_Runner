using LevelRunner.GameWorld;
using LevelRunner.Properties;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelRunner
{
    public static class MusicPlayer
    {
        private static bool Started { get; set; }
        public static WaveOut WaveOut { get; set; }
        public static List<UnmanagedMemoryStream> TrackList { get; set; }
        public static List<UnmanagedMemoryStream> ListenedTracks { get; set; }

        public static void Initialize()
        {
            WaveOut = new WaveOut();
            WaveOut.Volume = GameSettings.VolumeLevel;
            Started = false;

            // Events
            MainMenu.MainMenuLoaded += LoadMenuPlaylist;
            MainMenu.MainMenuLoaded += StartWaveOut;
            World.WorldLoaded += LoadGamePlaylist;
            World.WorldLoaded += StartWaveOut;
            WaveOut.PlaybackStopped += WaveOut_OnPlaybackStopped;
            GameSettings.VolumeLevelChanged += ChangeVolumeLevel;
        }

        private static void StartWaveOut()
        {
            if (WaveOut == null)
            {
                WaveOut = new WaveOut();
            }
            if (!Started)
            {
                NextTrack();
                Started = true;
            }
        }

        public static void NextTrack()
        {
            try
            {
                ChooseTrack(out UnmanagedMemoryStream track);
                WaveFileReader WaveFileReader = new WaveFileReader(track);
                WaveOut.Init(WaveFileReader);
                WaveOut.Play();
            }
            catch(Exception)
            {

            }
        }

        public static void CloseWaveOut()
        {
            if (WaveOut != null)
            {
                WaveOut.Stop();
                WaveOut.Dispose();
                WaveOut = null;
                Started = false;
            }
        }

        private static void ChooseTrack(out UnmanagedMemoryStream Track)
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
                foreach (UnmanagedMemoryStream track in TrackList)
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

        public static void LoadMenuPlaylist()
        {
            if (!Started)
            {
                TrackList = new List<UnmanagedMemoryStream>
                {
                    Resources.MainTheme,
                    Resources.Forever,
                    Resources.Divinitus,
                    Resources.Epic
                };
                ListenedTracks = new List<UnmanagedMemoryStream>();
            }
        }

        public static void LoadGamePlaylist()
        {
            if (!Started)
            {
                TrackList = new List<UnmanagedMemoryStream>
                {
                    Resources.Track1,
                    Resources.Track2,
                    Resources.Track3,
                    Resources.Track4,
                    Resources.Track5
                };
                ListenedTracks = new List<UnmanagedMemoryStream>();
            }
        }

        public static void ChangeVolumeLevel(float value)
        {
            if (WaveOut != null)
            {
                WaveOut.Volume = value;
            }
        }

        private static void WaveOut_OnPlaybackStopped(object sender, EventArgs e)
        {
            NextTrack();
        }
    };
}
