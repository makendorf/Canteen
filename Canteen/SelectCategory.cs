using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canteen
{
    public partial class SelectCategory : MetroForm
    {
        private SQL SqlConnection = new SQL();
        private string CurrentCategory;
        private string CurrentProduct;
        private bool Change;
        public SelectCategory(string currProd, string currCat)
        {
            InitializeComponent();
            CurrentCategory = currCat;
            CurrentProduct = currProd;
            Change = false;
        }

        private void SelectCategory_Load(object sender, EventArgs e)
        {
            using(var reader = SqlConnection.ExecuteQuery("select name from CategoryList"))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        metroComboBox1.Items.Add(reader.GetString(0));
                        if (CurrentCategory == (string)metroComboBox1.Items[metroComboBox1.Items.Count - 1])
                        {
                            metroComboBox1.SelectedIndex = metroComboBox1.Items.Count - 1;
                        }
                    }
                }
            }
            Change = true;
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Change)
            {
                if (MessageBox.Show("Изменить категорию продукта на выбранную?", "Изменение", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        SqlConnection.BeginTransaction();
                        SqlConnection.SetSqlParameters(new List<System.Data.SqlClient.SqlParameter>()
                        {
                            new System.Data.SqlClient.SqlParameter("@nameCat", metroComboBox1.SelectedItem),
                            new System.Data.SqlClient.SqlParameter("@nameProd", CurrentProduct)
                        });
                        int countUpdate = SqlConnection.ExecuteNonQuery("update ProductsList set category = (select top 1 Id from CategoryList where name like @nameCat) where name like @nameProd");
                        SqlConnection.Commit();
                        if (countUpdate > 0)
                        {
                            Close();
                        }
                        else
                        {
                            throw new Exception("Ошибка. Обработано 0 записей. Обратитесь к администратору. ");
                        }
                    }
                    catch (Exception exc)
                    {
                        SqlConnection.RollBack();
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }
    }
}
