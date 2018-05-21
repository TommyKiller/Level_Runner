﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LevelRunner.Properties;
using LevelRunner.Actors;
using LevelRunner.GameWorld;
using LevelRunner.GameWorld.Map;

namespace LevelRunner
{
    public partial class World : Form
    {
        // Temporary
        public List<Image> characterImageList;
        
        // Debugging
        public bool debug = false;
        public long summ = 0;
        public long counter = 0;
        public int wounded = 0;
        public int died = 0;
        public int respawned = 0;

        public bool ActorsActive { get; private set; }
        public List<Character> Actors { get; set; }
        public Map Map { get; set; }
        public Scene Scene { get; set; }
        public GameSettings Settings { get; set; }
        public Graphics Canvas { get; set; }
        public System.Windows.Forms.Timer Timer { get; set; }

        public World()
        {// All that is connected to form
            InitializeComponent();

            // Form settings
            WindowState = FormWindowState.Maximized;
            Thread.CurrentThread.IsBackground = true;

            // Propereties
            ActorsActive = false;
            Settings = new GameSettings(new Size(18, 24), // Standard chunk size
                new Character.Characteristics(40, 5, 1, "melee", 1.5, 2, 80), // Melee template
                new Character.Characteristics(40, 5, 1, "range", 9, 3, 80), // Range template
                16); // Timer interval

            // Lists
            Actors = new List<Character>();
            characterImageList = new List<Image>
            {
                Resources.MeleeWarA,
                Resources.RangeWarA,
                Resources.MeleeWarB,
                Resources.RangeWarB,
                Resources.MeleePlayer
            };
        }

        private void World_Load(object sender, EventArgs e)
        {// All that is connected to game world

            int height = ClientSize.Height / Settings.ChunkSize.Height;
            int width = ClientSize.Width / Settings.ChunkSize.Width;
            
            #region Scene
            Scene = new Scene(new Point(0, 0), new Size(width, height), this);
            #endregion

            #region Map
            Map = new Map(width, height);
            #endregion

            AddActors(10);
            SetTimer(Settings.TimerInterval);
        }

        private void AddActors(int number)
        {
            Random rnd = new Random();
            for (int i = 0; i < number; i++)
            {
                int res = rnd.Next(4);
                switch (res)
                {
                    case 0:
                        Actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(UnitType.Ground), (Bitmap)characterImageList[res], Settings.MeleeDefChars, "A"));
                        break;
                    case 1:
                        Actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(UnitType.Ground), (Bitmap)characterImageList[res], Settings.RangeDefChars, "A"));
                        break;
                    case 2:
                        Actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(UnitType.Ground), (Bitmap)characterImageList[res], Settings.MeleeDefChars, "B"));
                        break;
                    case 3:
                        Actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(UnitType.Ground), (Bitmap)characterImageList[res], Settings.RangeDefChars, "B"));
                        break;
                }
            }
        }

        public void StartActors()
        {
            foreach (Character character in Actors)
            {
                character.CharacterThread.Start();
            }
        }

        private void StopActors()
        {
            Monitor.Enter(Actors);
            foreach (Character character in Actors)
            {
                character.CharacterThread.Abort();
            }
            Monitor.Exit(Actors);
        }

        private void SetTimer(int interval)
        {
            Timer = new System.Windows.Forms.Timer
            {
                Interval = interval,
                Enabled = true
            };
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!ActorsActive)
            {
                Scene.Repaint(true);
                StartActors();
                ActorsActive = true;
            }
            else
            {
                Scene.Repaint();
            }

            // Debugging
            if (debug)
            {
                if ((wounded > 0) || (died > 0) || (respawned > 0))
                {
                    Console.WriteLine("____________________________________");
                    Console.WriteLine("Actors count: {0}", Actors.Count);
                    Console.WriteLine("Were wounded: {0}. Died: {1}. Respawned: {2}", wounded, died, respawned);
                    wounded = 0;
                    died = 0;
                    respawned = 0;
                }
            }
        }

        private void World_Paint(object sender, PaintEventArgs e)
        {
            Scene.Repaint(true);
        }

        private void World_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopActors();
        }
    }
}
