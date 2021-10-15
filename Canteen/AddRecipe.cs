using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class AddRecipe : MetroForm
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateProducts = "select name, category as Продукт from ProductsList order by name";
        private readonly string QueryInsertRecipe = "insert into RecipeList (dish, product, norm) values (" +
            "@dish, " +
            "(select top 1 Id from ProductsList where name like @product), " +
            "@norm)";
        private readonly string QueryInsertDish = "insert into DishList (name, [commit]) values (@name, @commit)";
        private readonly string QueryFindDish = "select top 1 Id from DishList where name like @dish order by name";
        private readonly string QuerySearchProduct=
                                "select name as Продукт from ProductsList where name like @name order by name";
        private readonly DataTable DataTableProducts = new DataTable();
        private readonly DataTable DataTableAddProducts = new DataTable();
        private SqlDataAdapter DataAdapterProducts;
        public AddRecipe()
        {
            InitializeComponent();
        }
        private void UpdateDataTableProducts()
        {
            DataTableProducts.Clear();
            DataAdapterProducts = SqlConnection.QueryForDataAdapter(QueryUpdateProducts);
            DataAdapterProducts.Fill(DataTableProducts);
            GridViewProductList.Columns[0].Width = 190;
        }

        private void AddRecipe_Load(object sender, EventArgs e)
        {
            GridViewProductList.DataSource = new BindingSource(DataTableProducts, null);
            dataGridView1.DataSource = new BindingSource(DataTableAddProducts, null);
            DataTableAddProducts.Columns.Add("Продукт");
            DataTableAddProducts.Columns.Add("Количество");
            UpdateDataTableProducts();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            AddProductFromRightGrid();

        }

        private void AddProductFromRightGrid()
        {
            using (var selectQuantityForm = new SelectQuantity("кг / 1 порция"))
            {
                var result = selectQuantityForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if(GridViewProductList.SelectedRows.Count > 0)
                    {
                        var Value = selectQuantityForm.ReturnValue;
                        var Name = GridViewProductList.Rows[GridViewProductList.SelectedRows[0].Index].Cells[0].Value;

                        var _row = DataTableAddProducts.NewRow();
                        _row["Продукт"] = Name;
                        _row["Количество"] = Value;
                        DataTableAddProducts.Rows.Add(_row);
                    }
                    else
                    {
                        var Value = selectQuantityForm.ReturnValue;
                        var Name = GridViewProductList.SelectedCells[0].Value;

                        var _row = DataTableAddProducts.NewRow();
                        _row["Продукт"] = Name;
                        _row["Количество"] = Value;
                        DataTableAddProducts.Rows.Add(_row);
                    }
                }
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            DataTableAddProducts.Rows[dataGridView1.SelectedRows[0].Index].Delete();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            var dishName = metroTextBox1.Text;
            var Id = FindDish(dishName);
            if (Id == 0)
            {
                try
                {
                    AddDish(dishName);
                    Id = FindDish(dishName);
                    AddRecipeMethod(Id);
                }
                catch(Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            else
            {
                MessageBox.Show("Данный рецепт уже существует");
            }
        }

        private void AddDish(string str)
        {
            SqlConnection.SetSqlParameters(new List<SqlParameter> {
                    new SqlParameter("@name", $@"{str}"),
                    new SqlParameter("@commit", $@"{metroTextBox2.Text}")
                });
            var count = SqlConnection.ExecuteNonQuery(QueryInsertDish);
        }

        private int FindDish(string str)
        {
            SqlConnection.SetSqlParameters(new List<SqlParameter> {
                    new SqlParameter("@dish", $@"{str}")
                });
            using (var reader = SqlConnection.ExecuteQuery(QueryFindDish))
                if (reader.HasRows)
                {
                    reader.Read();
                    var Id = reader.GetInt32(0);
                    reader.Close();
                    return Id;
                }
                else
                {
                    return 0;
                }
        }

        private void AddRecipeMethod(int idDish)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                SqlConnection.SetSqlParameters(new List<SqlParameter> {
                    new SqlParameter("@dish", idDish),
                    new SqlParameter("@product", dataGridView1.Rows[i].Cells[0].Value),
                    new SqlParameter("@norm", Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value))
                });
                SqlConnection.ExecuteNonQuery(QueryInsertRecipe);
            }
            MessageBox.Show("Успешно");
        }

        private void GridViewProductList_DoubleClick(object sender, EventArgs e)
        {
            AddProductFromRightGrid();
        }

        private void metroTextBox2_Click(object sender, EventArgs e)
        {
            if(metroTextBox2.Text == "Введите технологию приготовления сюда") metroTextBox2.Text = "";
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            var addProduct = new AddProduct();
            addProduct.ShowDialog();
            UpdateDataTableProducts();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            DataTableProducts.Clear();
            DataAdapterProducts = SqlConnection.QueryForDataAdapter(QuerySearchProduct);
            DataAdapterProducts.SelectCommand.Parameters.AddWithValue("@name", $@"%{toolStripTextBox1.Text}%");
            DataAdapterProducts.Fill(DataTableProducts);
            GridViewProductList.Columns[0].Width = 190;
        }
    }
}
