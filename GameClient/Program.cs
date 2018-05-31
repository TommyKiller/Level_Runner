using System;
using System.Windows.Forms;

namespace LevelRunner
{
    static class Program
    {
        // Forms
        public static ApplicationContext Context { get; set; }
        public static World World { get; set; }

        // Properties
        public static GameWorld.GameSettings Settings { get; private set; }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Settings = new GameWorld.GameSettings();
            if (!Settings.Load("settings.json"))
            {
                Settings.SetDefault();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Context = new ApplicationContext();
            MainMenu MainMenu = new MainMenu();
            Context.MainForm = MainMenu;
            MusicPlayer.Initialize();
            Application.Run(Context);
        }
    }
}
