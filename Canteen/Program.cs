using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Canteen
{
    public class Program
    {

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>

        public static DataTable ReWeighTable = new DataTable();
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.Source);
            }
        }
    }
}
