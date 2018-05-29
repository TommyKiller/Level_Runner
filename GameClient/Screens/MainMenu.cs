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
        // Events
        public static event Delegates.EventDelegate MainMenuLoaded;

        public MainMenu()
        {
            InitializeComponent();

            // Settings
            FormBorderStyle = GameSettings.FormBorderStyle;

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
            Settings Settings = new Settings();
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
