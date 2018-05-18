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
using Level_Runner_Demo.Properties;

namespace Level_Runner_Demo
{
    public partial class GameClient : Form
    {
        // Events
        public event Delegates.ActDelegate OnMoveKeyUp;
        public event Delegates.ActDelegate OnMoveKeyDown;
        public event Delegates.OnMoveKeyPressedDelegate OnMoveKeyPressed;

        // Objects
        private System.Windows.Forms.Timer timer;
        public Map map;
        public Scene scene;
        public Settings settings;
        public List<Character> actors;
        public List<Image> terrainImageList;
        public List<Image> characterImageList;

        // Debugging
        public bool debug = false;
        public long summ = 0;
        public long counter = 0;
        public int wounded = 0;
        public int died = 0;
        public int respawned = 0;

        public GameClient()
        {// All that is connected to form
            InitializeComponent();

            // Settings
            ClientSize = Screen.PrimaryScreen.Bounds.Size;

            // Thread
            Thread.CurrentThread.IsBackground = true;

            // Objects
            settings = new Settings(new Size(18, 24), // Standard chunk size
                new Character.Characteristics(40, 5, 1, "melee", 1.5, 2, 80), // Melee template
                new Character.Characteristics(40, 5, 1, "range", 9, 3, 80), // Range template
                16); // Timer interval

            // Lists
            actors = new List<Character>();
            terrainImageList = new List<Image>
            {
                Resources.Grass,
                Resources.RoadLeft_Right,
                Resources.RoadTop_Bottom,
                Resources.WoodenBridge,
                Resources.Stone,
                Resources.Water
            };
            characterImageList = new List<Image>
            {
                Resources.MeleeWarA,
                Resources.RangeWarA,
                Resources.MeleeWarB,
                Resources.RangeWarB,
                Resources.MeleePlayer
            };
        }

        private void GameClient_Load(object sender, EventArgs e)
        {// All that is connected to game world
            #region SceneRegion
            int height = Convert.ToInt32(Math.Floor((double)(ClientSize.Height / settings.chunkSize.Height)));
            int width = Convert.ToInt32(Math.Floor((double)(ClientSize.Width / settings.chunkSize.Width)));
            scene = new Scene(new Point(0, 0), new Size(width, height), this);
            #endregion

            #region MapRegion
            map = new Map(
                Convert.ToInt32(Math.Floor((double)ClientSize.Width / settings.chunkSize.Width)),
                Convert.ToInt32(Math.Floor((double)ClientSize.Height / settings.chunkSize.Height)),
                this);
            #endregion

            AddActors(50); // Actors adding
            scene.Repaint();
            SetTimer(settings.timerInterval);
            StartActors();
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
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mechanics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.MeleeDefChars, "A"));
                        break;
                    case 1:
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mechanics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.RangeDefChars, "A"));
                        break;
                    case 2:
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mechanics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.MeleeDefChars, "B"));
                        break;
                    case 3:
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mechanics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.RangeDefChars, "B"));
                        break;
                }
            }
        }

        private void StartActors()
        {
            foreach (Character character in actors)
            {
                character.characterThread.Start();
            }
        }

        private void StopActors()
        {
            Monitor.Enter(actors);
            foreach (Character character in actors)
            {
                character.characterThread.Abort();
            }
            Monitor.Exit(actors);
        }

        private void SetTimer(int interval)
        {

            timer = new System.Windows.Forms.Timer
            {
                Interval = interval,
                Enabled = true
            };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            scene.Repaint();

            // Debugging
            if (debug)
            {
                if ((wounded > 0) || (died > 0) || (respawned > 0))
                {
                    Console.WriteLine("____________________________________");
                    Console.WriteLine("Actors count: {0}", actors.Count);
                    Console.WriteLine("Were wounded: {0}. Died: {1}. Respawned: {2}", wounded, died, respawned);
                    wounded = 0;
                    died = 0;
                    respawned = 0;
                }
            }
        }

        public struct Settings
        {
            public int timerInterval;
            public Character.Characteristics MeleeDefChars;
            public Character.Characteristics RangeDefChars;
            public readonly Size chunkSize;
            public readonly Dictionary<string, Dictionary<string, string>> relationsTable;
            public Settings(Size chunkSize, Character.Characteristics MeleeDefChars, Character.Characteristics RangeDefChars, int timerInterval)
            {
                relationsTable = new Dictionary<string, Dictionary<string, string>>
                {
                    { "A", new Dictionary<string, string>() },
                    { "B", new Dictionary<string, string>() }
                };
                relationsTable["A"].Add("A", "friendly");
                relationsTable["A"].Add("B", "hostile");
                relationsTable["B"].Add("A", "hostile");
                relationsTable["B"].Add("B", "friendly");
                this.timerInterval = timerInterval;
                this.chunkSize = chunkSize;
                this.MeleeDefChars = MeleeDefChars;
                this.RangeDefChars = RangeDefChars;
            }
        }

        private void GameClient_KeyDown(object sender, KeyEventArgs e)
        {
            Keys[] moveKeys = new Keys[]
            {
                Keys.W, Keys.A, Keys.S, Keys.D
            };
            if (moveKeys.Contains(e.KeyCode)) OnMoveKeyDown?.Invoke();
        }

        private void GameClient_KeyPress(object sender, KeyPressEventArgs e) // !!!!
        {
            char[] moveKeys = new char[]
            {
                'W', 'A', 'S', 'D'
            };
            if (moveKeys.Contains(e.KeyChar))
            {
                switch (e.KeyChar)
                {
                    case 'W':
                        OnMoveKeyPressed?.Invoke(0, 1);
                        break;
                    case 'A':
                        OnMoveKeyPressed?.Invoke(-1, 0);
                        break;
                    case 'S':
                        OnMoveKeyPressed?.Invoke(0, -1);
                        break;
                    case 'D':
                        OnMoveKeyPressed?.Invoke(1, 0);
                        break;
                }
            }
        }

        private void GameClient_KeyUp(object sender, KeyEventArgs e)
        {
            Keys[] moveKeys = new Keys[]
            {
                Keys.W, Keys.A, Keys.S, Keys.D
            };
            if (moveKeys.Contains(e.KeyCode)) OnMoveKeyUp?.Invoke();
        }

        private void GameClient_Paint(object sender, PaintEventArgs e)
        {
            scene.Repaint();
        }

        private void GameClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopActors();
        }
    }
}
