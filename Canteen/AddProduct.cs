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
        private readonly string QueryUpdateProductList = "select name as Продукт from ProductsList order by name";
        private readonly string QueryInsertProductList = "insert into ProductsList (name, category) " +
            "values (@name, (select top 1 Id from CategoryList where name like @category)); " +
            "insert into ProductsQuantity (product_id) " +
            "values ((select top 1 Id from ProductsList where name like @name))";
        private readonly DataTable DataTableProductList = new DataTable();
        private SqlDataAdapter DataAdapterProductList;
        public AddProduct()
        {
            InitializeComponent();
        }
        private void UpdateDataTableProductList()
        {
            DataTableProductList.Clear();
            DataAdapterProductList = SqlConnection.QueryForDataAdapter(QueryUpdateProductList);
            DataAdapterProductList.Fill(DataTableProductList);
            GridViewProductList.Columns[0].Width = 190;
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
    }
}
