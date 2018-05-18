using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Level_Runner_Demo
{
    static class Program
    {
        // Forms
        public static ApplicationContext Context { get; set; }
        public static GameClient GameClient { get; set; }
        public static MainMenu MainMenu { get; set; }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainMenu = new MainMenu();
            Application.Run(MainMenu);
        }
    }
}
