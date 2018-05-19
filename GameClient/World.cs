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
    public partial class World : Form
    {
        // Events
        public event Delegates.ActDelegate OnMoveKeyUp;
        public event Delegates.ActDelegate OnMoveKeyDown;
        public event Delegates.OnMoveKeyPressedDelegate OnMoveKeyPressed;

        // Objects
        public System.Windows.Forms.Timer timer;
        public Map map;
        public Scene scene;
        public Settings settings;

        // Lists
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

        public World()
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

        private void World_Load(object sender, EventArgs e)
        {// All that is connected to game world
            #region SceneRegion
            int height = Convert.ToInt32(Math.Floor((double)(ClientSize.Height / settings.ChunkSize.Height)));
            int width = Convert.ToInt32(Math.Floor((double)(ClientSize.Width / settings.ChunkSize.Width)));
            scene = new Scene(new Point(0, 0), new Size(width, height), this);
            #endregion

            #region MapRegion
            map = new Map(
                Convert.ToInt32(Math.Floor((double)ClientSize.Width / settings.ChunkSize.Width)),
                Convert.ToInt32(Math.Floor((double)ClientSize.Height / settings.ChunkSize.Height)),
                this);
            #endregion

            //AddActors(50); // Actors adding
            scene.Repaint();
            SetTimer(settings.TimerInterval);
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
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.MeleeDefChars, "A"));
                        break;
                    case 1:
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.RangeDefChars, "A"));
                        break;
                    case 2:
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.MeleeDefChars, "B"));
                        break;
                    case 3:
                        actors.Add(new NPC(String.Format("AI.NPC {0}", i), Mathematics.GetRandomFreePoint(), (Bitmap)characterImageList[res], settings.RangeDefChars, "B"));
                        break;
                }
            }
        }

        private void StartActors()
        {
            foreach (Character character in actors)
            {
                character.CharacterThread.Start();
            }
        }

        private void StopActors()
        {
            Monitor.Enter(actors);
            foreach (Character character in actors)
            {
                character.CharacterThread.Abort();
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

        private void World_Paint(object sender, PaintEventArgs e)
        {
            scene.Repaint();
        }

        private void World_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopActors();
        }
    }
}
