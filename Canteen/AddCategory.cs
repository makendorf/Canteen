using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class AddCategory : MetroForm
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateCategory = "select name as Категория from CategoryList order by name";
        private readonly string QueryInsertCategory = "insert into CategoryList (name) " +
            "values (@name)";
        private readonly DataTable DataTableCategory = new DataTable();
        private SqlDataAdapter DataAdapterCategory;

        private void UpdateDataTableCategory()
        {
            DataTableCategory.Clear();
            DataAdapterCategory = SqlConnection.QueryForDataAdapter(QueryUpdateCategory);
            DataAdapterCategory.Fill(DataTableCategory);
            GridViewProductList.Columns[0].Width = 190;
        }
        public AddCategory()
        {
            InitializeComponent();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@name", $"{metroTextBox1.Text}"),
            };
            SqlConnection.SetSqlParameters(sqlParameters);
            int insert = 0;
            try
            {
                insert = SqlConnection.ExecuteNonQuery(QueryInsertCategory);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            if (insert > 0)
            {
                MessageBox.Show("Успешно");
            }
            UpdateDataTableCategory();
        }

        private void AddCategory_Load(object sender, EventArgs e)
        {
            GridViewProductList.DataSource = new BindingSource(DataTableCategory, null);
            UpdateDataTableCategory();
        }

        private void GridViewProductList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (metroCheckBox1.Checked)
            {
                try
                {
                    string product = GridViewProductList.SelectedCells[0].Value.ToString();
                    var ChangeProduct = new SelectQuantity(product);
                    ChangeProduct.Text = product;
                    ChangeProduct.metroLabel1.Text = "Новое имя:";
                    ChangeProduct.ShowDialog();
                    if (ChangeProduct.DialogResult == DialogResult.OK)
                    {
                        SqlConnection.BeginTransaction();
                        SqlConnection.SetSqlParameters(new List<SqlParameter>()
                                    {
                                        new SqlParameter("@name", ChangeProduct.ReturnValue),
                                        new SqlParameter("@oldName", product)
                                    });
                        int countChange = SqlConnection.ExecuteNonQuery("update CategoryList set name = @name where name like @oldName");
                        if (countChange <= 0)
                        {
                            throw new Exception("Ошибка. Изменено 0 записей. Обратитесь к администратору");
                        }
                        SqlConnection.Commit();
                    }
                    UpdateDataTableCategory();
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
