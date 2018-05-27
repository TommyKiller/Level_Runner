using LevelRunner.Actors;
using LevelRunner.Actors.Fractions;
using LevelRunner.Actors.NPC;
using LevelRunner.GameWorld;
using LevelRunner.GameWorld.Map;
using LevelRunner.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LevelRunner
{
    public partial class World : Form
    {
        // Events
        public event Delegates.EventDelegate OnTimer;
        public event Delegates.EventDelegate OnMoveKeyDown;

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
        public Camera Camera { get; set; }
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
                1); // Timer interval

            // Lists
            Actors = new List<Character>();
        }

        private void World_Load(object sender, EventArgs e)
        {// All that is connected to game world
            Scene = new Scene(this);
            Camera = new Camera(this);
            Map = new Map(150, 120);

            Actors.Add(new Player(this, new FTerronia(), Calculate.GetRandomFreePoint(UnitTypes.GroundUnit), "Tommy"));
            AddActors(25);
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
                        Actors.Add(new AIWarrior(this, new FMern(), Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 1:
                        Actors.Add(new AIArcher(this, new FMern(), Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 2:
                        Actors.Add(new AIWarrior(this, new FRivia(), Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 3:
                        Actors.Add(new AIArcher(this, new FRivia(), Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                }
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemTimeAdjustment(out long lpTimeAdjustment,
            out long lpTimeIncrement, out bool lpTimeAdjustmentDisabled);

        private void SetTimer(int interval)
        {
            if (GetSystemTimeAdjustment(out long timeAdjustment, out long timeIncrement, out bool timeAdjustmentDisabled))
            {
                interval = !timeAdjustmentDisabled ? (int)(timeIncrement / 10000) : interval;
            }
            Console.WriteLine(interval);
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
            OnTimer?.Invoke();

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

        private void World_KeyDown(object sender, KeyEventArgs e)
        {
            UserControl.ChangeState(e.KeyCode, true);
            if ((UserControl.KeyPressed(Keys.Right)) || (UserControl.KeyPressed(Keys.Left)) ||
                (UserControl.KeyPressed(Keys.Up)) || (UserControl.KeyPressed(Keys.Down)))
            {
                OnMoveKeyDown?.Invoke();
            }
        }

        private void World_KeyUp(object sender, KeyEventArgs e)
        {
            UserControl.ChangeState(e.KeyCode, false);
        }
    }
}
