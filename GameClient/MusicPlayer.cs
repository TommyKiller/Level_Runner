using LevelRunner.GameWorld;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace LevelRunner
{
    public static class MusicPlayer
    {
        private static bool Started { get; set; }
        public static WaveOut WaveOut { get; set; }
        public static List<string> TrackList { get; set; }
        public static List<string> ListenedTracks { get; set; }

        public static void Initialize()
        {
            WaveOut = new WaveOut
            {
                Volume = Program.Settings.VolumeLevel
            };
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
            ChooseTrack(out string track);
            FileStream inputStream = File.OpenRead(track);
            WaveFileReader WaveFileReader = new WaveFileReader(inputStream);
            WaveOut.Init(WaveFileReader);
            WaveOut.Play();
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

        private static void ChooseTrack(out string Track)
        {
            if (ListenedTracks.Count == TrackList.Count)
            {
                ListenedTracks.Clear();
                ChooseTrack(out Track);
            }
            else
            {
                Random random = new Random();
                List<string> TempTrackList = new List<string>();
                foreach (string track in TrackList)
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
                TrackList = new List<string>
                {
                    @"Resources\Music\MainTheme.wav",
                    @"Resources\Music\Divinitus.wav",
                    @"Resources\Music\Epic.wav",
                    @"Resources\Music\Forever.wav"
                };
                ListenedTracks = new List<string>();
            }
        }

        public static void LoadGamePlaylist()
        {
            if (!Started)
            {
                TrackList = new List<string>
                {
                    @"Resources\Music\Track1.wav",
                    @"Resources\Music\Track2.wav",
                    @"Resources\Music\Track3.wav",
                    @"Resources\Music\Track4.wav",
                    @"Resources\Music\Track5.wav"
                };
                ListenedTracks = new List<string>();
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
