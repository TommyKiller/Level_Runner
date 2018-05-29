using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelRunner
{
    static class Program
    {
        // Forms
        public static ApplicationContext Context { get; set; }
        public static World World { get; set; }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
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
