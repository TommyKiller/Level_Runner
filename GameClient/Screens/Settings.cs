using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LevelRunner.GameWorld;

namespace LevelRunner
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            // Form settings
            FormBorderStyle = GameSettings.FormBorderStyle;

            volumeTracker.Value = (int)(GameSettings.VolumeLevel * 100);
            screenModePicker.DropDownStyle = ComboBoxStyle.DropDownList;
            screenModePicker.Text = screenModePicker.Items[0].ToString();

            // Events
            GameSettings.SettingsChanged += GameSettings_OnChange;
        }

        private void GameSettings_OnChange()
        {
            FormBorderStyle = GameSettings.FormBorderStyle;
        }

        private void playerColorPickButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                GameSettings.PlayerColor = colorDialog.Color;
            }
        }

        private void volumeTracker_Scroll(object sender, EventArgs e)
        {
            GameSettings.VolumeLevel = volumeTracker.Value;
        }

        private void screenModePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (screenModePicker.Text)
            {
                case "FULLSCREEN":
                    GameSettings.FormBorderStyle = FormBorderStyle.None;
                    break;
                case "WINDOWED":
                    GameSettings.FormBorderStyle = FormBorderStyle.Sizable;
                    break;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            Program.Context.MainForm = MainMenu;
            Close();
            Program.Context.MainForm.Show();
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Events
            GameSettings.SettingsChanged -= GameSettings_OnChange;
        }
    }
}
