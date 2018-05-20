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
        public static World World;
        public static MainMenu MainMenu;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Context = new ApplicationContext();
            MainMenu = new MainMenu();
            Context.MainForm = MainMenu;
            Application.Run(Context);
        }
    }
}
