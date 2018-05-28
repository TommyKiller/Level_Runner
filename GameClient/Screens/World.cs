#define DEBUG

using LevelRunner.Actors;
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
        #if DEBUG
            public long summ = 0;
            public long counter = 0;
            public int wounded = 0;
            public int died = 0;
            public int respawned = 0;
        #endif
        #endregion

        public List<Character> Actors { get; set; }
        public Map Map { get; set; }
        public Scene Scene { get; set; }
        public Camera Camera { get; set; }
        public Graphics Canvas { get; set; }
        public System.Windows.Forms.Timer Timer { get; set; }

        public World()
        {// All that is connected to form
            InitializeComponent();

            // Form settings
            FormBorderStyle = GameSettings.FormBorderStyle;
            WindowState = FormWindowState.Maximized;
            Thread.CurrentThread.IsBackground = true;

            // Components
            Actors = new List<Character>();
            Scene = new Scene(this);
            Camera = new Camera(this);
            Map = new Map(150, 150);
        }

        private void World_Load(object sender, EventArgs e)
        {// All that is connected to game world
            Actors.Add(new Player(this, Fraction.Fractions.Player, new Point(0, 0), "Tommy"));
            AddActors(30);
            SetTimer(GameSettings.TimerInterval);
        }

        private void AddActors(int number)
        {
            Random random = new Random();
            for (int i = 0; i < number; i++)
            {
                int index = random.Next(4);
                switch (index)
                {
                    case 0:
                        Actors.Add(new AIWarrior(this, Fraction.Fractions.Mern, Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 1:
                        Actors.Add(new AIArcher(this, Fraction.Fractions.Mern, Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 2:
                        Actors.Add(new AIWarrior(this, Fraction.Fractions.Rivia, Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
                        break;
                    case 3:
                        Actors.Add(new AIArcher(this, Fraction.Fractions.Rivia, Calculate.GetRandomFreePoint(UnitTypes.GroundUnit)));
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
            #if DEBUG
                if ((wounded > 0) || (died > 0) || (respawned > 0))
                {
                    Console.WriteLine("____________________________________");
                    Console.WriteLine("Actors count: {0}", Actors.Count);
                    Console.WriteLine("Were wounded: {0}. Died: {1}. Respawned: {2}", wounded, died, respawned);
                    wounded = 0;
                    died = 0;
                    respawned = 0;
                }
            #endif
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
            if (UserControl.KeyPressed(Keys.Escape))
            {
                Timer.Stop();
                DialogResult result = MessageBox.Show("Do you really want to quit?", "Exit to main menu?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    MainMenu MainMenu = new MainMenu();
                    Program.Context.MainForm = MainMenu;
                    Close();
                    Program.Context.MainForm.Show();
                }
                else
                {
                    Timer.Start();
                }
                UserControl.ChangeState(Keys.Escape, false);
            }
        }

        private void World_KeyUp(object sender, KeyEventArgs e)
        {
            UserControl.ChangeState(e.KeyCode, false);
        }
    }
}
