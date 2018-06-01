using LevelRunner.GameWorld;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LevelRunner
{
    public partial class MainMenu : Form
    {
        // Events
        public static event Delegates.EventDelegate MainMenuLoaded;

        public MainMenu()
        {
            InitializeComponent();

            // Settings
            FormBorderStyle = Program.Settings.FormBorderStyle;
            WindowState = FormWindowState.Maximized;
            BackgroundImage = new Bitmap(Image.FromFile(@"Resources\Assets\Backgrounds\MainMenuBackground.jpg"), ClientSize);

            // Events
            GameSettings.FormBorderStyleChanged += GameSettings_OnChange;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            MainMenuLoaded();
        }

        private void GameSettings_OnChange(FormBorderStyle value)
        {
            FormBorderStyle = value;
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.CloseWaveOut();
            Program.World = new World();
            Program.Context.MainForm = Program.World;
            Close();
            Program.Context.MainForm.Show();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            About About = new About();
            Program.Context.MainForm = About;
            Close();
            Program.Context.MainForm.Show();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm Settings = new SettingsForm();
            Program.Context.MainForm = Settings;
            Close();
            Program.Context.MainForm.Show();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Events
            GameSettings.FormBorderStyleChanged -= GameSettings_OnChange;
        }
    }
}
