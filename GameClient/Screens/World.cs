using System;
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
using LevelRunner.Actors.NPC;
using LevelRunner.Actors.Fractions;

namespace LevelRunner
{
    public partial class World : Form
    {
        // Events
        public event Delegates.EventDelegate OnTimer;

        #region Debugging
        public bool debug = false;
        public long summ = 0;
        public long counter = 0;
        public int wounded = 0;
        public int died = 0;
        public int respawned = 0;
        #endregion

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
            Settings = new GameSettings(new Size(18, 24), // Standard chunk size
                16); // Timer interval

            // Lists
            Actors = new List<Character>();
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

            AddActors(100);
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
                        Actors.Add(new AIWarrior(this, new FMern(), Mathematics.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 1:
                        Actors.Add(new AIArcher(this, new FMern(), Mathematics.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 2:
                        Actors.Add(new AIWarrior(this, new FRivia(), Mathematics.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 3:
                        Actors.Add(new AIArcher(this, new FRivia(), Mathematics.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                }
            }
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
            Scene.Repaint();
            OnTimer();

            #region Debugging
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
            #endregion
        }

        private void World_Paint(object sender, PaintEventArgs e)
        {
            Scene.Repaint();
        }
    }
}
