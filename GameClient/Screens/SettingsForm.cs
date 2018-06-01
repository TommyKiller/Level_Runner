using LevelRunner.GameWorld;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LevelRunner
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            // Form settings
            FormBorderStyle = Program.Settings.FormBorderStyle;
            WindowState = FormWindowState.Maximized;
            BackgroundImage = new Bitmap(Image.FromFile(@"Resources\Assets\Backgrounds\SettingsBackground.jpg"), ClientSize);

            // Form elements
            volumeTracker.Value = (int)(Program.Settings.VolumeLevel * 100);
            screenModePicker.DropDownStyle = ComboBoxStyle.DropDownList;
            screenModePicker.Text = screenModePicker.Items[0].ToString();

            // Events
            GameSettings.FormBorderStyleChanged += GameSettings_OnChange;
        }

        private void GameSettings_OnChange(FormBorderStyle value)
        {
            FormBorderStyle = value;
        }

        private void playerColorPickButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Program.Settings.PlayerColor = colorDialog.Color;
            }
        }

        private void volumeTracker_Scroll(object sender, EventArgs e)
        {
            Program.Settings.VolumeLevel = (float)(volumeTracker.Value / 100.100);
        }

        private void screenModePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (screenModePicker.Text)
            {
                case "FULLSCREEN":
                    Program.Settings.FormBorderStyle = FormBorderStyle.None;
                    break;
                case "WINDOWED":
                    Program.Settings.FormBorderStyle = FormBorderStyle.Sizable;
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
            Program.Settings.Save("settings.json");

            // Events
            GameSettings.FormBorderStyleChanged -= GameSettings_OnChange;
        }
    }
}
