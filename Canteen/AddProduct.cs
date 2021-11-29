using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class AddProduct : MetroForm
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateProductList = "select ProductsList.name as Продукт, CategoryList.name as Категория from ProductsList left join CategoryList on CategoryList.Id = ProductsList.category order by ProductsList.name";
        private readonly string QueryInsertProductList = "insert into ProductsList (name, category) " +
            "values (@name, (select top 1 Id from CategoryList where name like @category)); " +
            "insert into ProductsQuantity (product_id) " +
            "values ((select top 1 Id from ProductsList where name like @name))";
        private readonly DataTable DataTableProductList = new DataTable();
        private SqlDataAdapter DataAdapterProductList;
        private bool ReturnBack;
        public string ProductValue = "";
        public AddProduct(bool returnBack = false)
        {
            InitializeComponent();
            ReturnBack = returnBack;
        }
        private void UpdateDataTableProductList()
        {
            DataTableProductList.Clear();
            DataAdapterProductList = SqlConnection.QueryForDataAdapter(QueryUpdateProductList);
            DataAdapterProductList.Fill(DataTableProductList);
            GridViewProductList.Columns[0].Width = 190;
            GridViewProductList.Columns[1].Width = 120;
        }
        private void AddProduct_Load(object sender, EventArgs e)
        {
            SetCommit();
            GridViewProductList.DataSource = new BindingSource(DataTableProductList, null);
            UpdateDataTableProductList();
            using (DbDataReader reader = SqlConnection.ExecuteQuery("select name from CategoryList"))
            {
                while (reader.Read())
                {
                    metroComboBox1.Items.Add(reader.GetString(0).Trim());
                }
            }

        }

        private void SetCommit()
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(metroButton1, "Изменение списка категория продуктов");
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@name", $"{metroTextBox1.Text}"),
                new SqlParameter("@category", $"%{metroComboBox1.Text}%"),
            };
            SqlConnection.SetSqlParameters(sqlParameters);
            int insert = 0;
            try
            {
                //SqlConnection.BeginTransaction();
                insert = SqlConnection.ExecuteNonQuery(QueryInsertProductList);
                //SqlConnection.Commit();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                //SqlConnection.RollBack();
            }
            if (insert > 0)
            {
                MessageBox.Show("Успешно");
            }
            UpdateDataTableProductList();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            AddCategory addCategoryForm = new AddCategory();
            addCategoryForm.ShowDialog();
        }

        private void GridViewProductList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (ReturnBack)
            {
                ProductValue = GridViewProductList.CurrentCell.Value.ToString();
                Close();
            }
            if (metroCheckBox1.Checked)
            {
                switch (GridViewProductList.SelectedCells[0].ColumnIndex)
                {
                    case 0:
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
                                    int countChange = SqlConnection.ExecuteNonQuery("update ProductsList set name = @name where name like @oldName");
                                    if (countChange <= 0)
                                    {
                                        throw new Exception("Ошибка. Изменено 0 записей. Обратитесь к администратору");
                                    }
                                    SqlConnection.Commit();
                                }
                                UpdateDataTableProductList();
                            }
                            catch(Exception exc)
                            {
                                SqlConnection.RollBack();
                                MessageBox.Show(exc.Message);
                            }
                            
                            break;
                        }
                    case 1:
                        {
                            string category = GridViewProductList.SelectedCells[0].Value.ToString();
                            string product = GridViewProductList.Rows[GridViewProductList.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                            var selectCategory = new SelectCategory(product, category);
                            selectCategory.Text = product;
                            selectCategory.ShowDialog();
                            UpdateDataTableProductList();
                            break;
                        }
                }
            }
        }
    }
}
