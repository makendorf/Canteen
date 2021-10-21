using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Canteen
{
    public class Program
    {

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        public static readonly List<TypeOperation> TypeOperationsList = new List<TypeOperation>();
        private static void InsertTypeOperation()
        {
            SQL sql = new SQL();
            string query = "select name from TypeOperation";
            using (System.Data.Common.DbDataReader reader = sql.ExecuteQuery(query))
            {
                try
                {
                    reader.Read();
                    TypeOperationsList.Add(new TypeOperation { Type = Type.None, TypeStr = reader.GetString(0).Trim() });
                    reader.Read();
                    TypeOperationsList.Add(new TypeOperation { Type = Type.Production, TypeStr = reader.GetString(0).Trim() });
                    reader.Read();
                    TypeOperationsList.Add(new TypeOperation { Type = Type.Sale, TypeStr = reader.GetString(0).Trim() });
                    reader.Read();
                    TypeOperationsList.Add(new TypeOperation { Type = Type.Incoming, TypeStr = reader.GetString(0).Trim() });
                    reader.Read();
                    TypeOperationsList.Add(new TypeOperation { Type = Type.Inventarization, TypeStr = reader.GetString(0).Trim() });
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                finally
                {
                    sql.Close();
                }
            }
        }
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                InsertTypeOperation();
                Application.Run(new Main());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.Source);
            }
        }
    }
}
