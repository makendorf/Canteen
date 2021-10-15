using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Canteen
{
    public partial class ProductAndRecipe : Form
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateRecipe =
                                "select name as Блюдо from DishList order by name";
        private readonly string QuerySearchRecipe =
                                "select name as Блюдо from DishList where name like @name order by name";
        private readonly string QueryUpdateProductOfRecipe =
                                "select products.name as Продукт, RecipeList.norm as 'Норма (кг/1 порция)' from RecipeList " +
                                "left join DishList as dishs on dishs.Id = RecipeList.dish " +
                                "left join ProductsList as products on products.Id = RecipeList.product " +
                                "where RecipeList.dish = (select top 1 Id from DishList where name like @nameDish) " +
                                "order by products.name";
        private readonly string QueryUpdateTehnicalCard =
                                "select DishList.[commit] from DishList where name like @nameDish";
        private readonly string QueryUpdateValueProductName =
                                "update ProductsList set name = @newName where name like @oldName";
        private readonly string QueryUpdateValueDishName =
                                "update DishList set name = @newName where name like @oldName";
        private readonly string QueryUpdateValueProductNorm =
                                "update RecipeList set norm = @newNorm where " +
                                "product like (select Id from ProductsList where name like @nameProduct) and " +
                                "dish like (select Id from DishList where name like @nameDish)";


        private DataTable DataTableRecipe = new DataTable();
        private DataTable DataTableProductOfRecipe = new DataTable();
        private DataTable DataTableProductLast = new DataTable();
        private DataTable DataTableRecipeLast = new DataTable();

        private SqlDataAdapter DataAdapterRecipe;
        private SqlDataAdapter DataAdapterProductOfRecipe;
        public ProductAndRecipe()
        {
            InitializeComponent();
            metroTextBox1.Dock = DockStyle.Fill;
        }

        private void AddRecipe_Load(object sender, EventArgs e)
        {
            DataGridProduct.DataSource = new BindingSource(DataTableProductOfRecipe, null);
            DataGridRecipe.DataSource = new BindingSource(DataTableRecipe, null);
            UpdateDataTableRecipe();
        }

        private void UpdateDataTableRecipe()
        {
            DataTableRecipe.Clear();
            DataAdapterRecipe = SqlConnection.QueryForDataAdapter(QueryUpdateRecipe);
            DataAdapterRecipe.Fill(DataTableRecipe);
            DataAdapterRecipe.Fill(DataTableRecipeLast);
            //DataTableRecipeLast = DataTableRecipe;
            DataGridRecipe.Columns[0].Width = 200;
        }

        private void RecipeDataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            UpdateDataTableProduct();
        }

        private void UpdateDataTableProduct()
        {
            var nameDish = DataGridRecipe.SelectedRows[0].Cells[0].Value.ToString().Trim();
            DataTableProductOfRecipe.Clear();
            DataAdapterProductOfRecipe = SqlConnection.QueryForDataAdapter(QueryUpdateProductOfRecipe);
            DataAdapterProductOfRecipe.SelectCommand.Parameters.AddWithValue("@nameDish", nameDish);
            DataAdapterProductOfRecipe.Fill(DataTableProductOfRecipe);

            SqlConnection.SetSqlParameters(new List<SqlParameter>
            {
                new SqlParameter("@nameDish", nameDish)
            });
            using(var reader = SqlConnection.ExecuteQuery(QueryUpdateTehnicalCard))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    try
                    {
                        metroTextBox1.Text = reader.GetString(0);
                    }
                    catch
                    {
                        metroTextBox1.Text = "Технология приготовления:";
                    }
                }
            }

            DataTableProductLast = DataTableProductOfRecipe;
            toolStripLabel2.Text = $"Ингридиенты для '{DataGridRecipe.SelectedRows[0].Cells[0].Value}'";
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            DataTableRecipe.Clear();
            DataAdapterRecipe = SqlConnection.QueryForDataAdapter(QuerySearchRecipe);
            DataAdapterRecipe.SelectCommand.Parameters.AddWithValue("@name", $@"%{toolStripTextBox1.Text}%");
            DataAdapterRecipe.Fill(DataTableRecipe);
        }

        private void ProductDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var cell = DataGridProduct.CurrentCell;
            if (cell.ColumnIndex == 0)
            {
                SqlConnection.SetSqlParameters(new List<SqlParameter> {
                    new SqlParameter("@oldName", $@"{DataTableProductLast.Rows[cell.RowIndex].ItemArray[0]}%"),
                    new SqlParameter("@newName", $@"{cell.Value}")
                });
                SqlConnection.ExecuteNonQuery(QueryUpdateValueProductName);
                UpdateDataTableProduct();
                DataTableProductLast = DataTableProductOfRecipe;
            }
            else if (cell.ColumnIndex == 1)
            {
                SqlConnection.SetSqlParameters(new List<SqlParameter> {
                    new SqlParameter("@newNorm", DataTableProductLast.Rows[cell.RowIndex].ItemArray[1]),
                    new SqlParameter("@nameProduct", $@"{DataGridProduct.Rows[cell.RowIndex].Cells[0].Value}"),
                    new SqlParameter("@nameDish", $@"{DataGridRecipe.SelectedRows[0].Cells[0].Value}")
                });
                int q = SqlConnection.ExecuteNonQuery(QueryUpdateValueProductNorm);
                UpdateDataTableProduct();
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            var AddProduct = new AddProduct();
            AddProduct.ShowDialog();
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            
            var AddRecipe = new AddRecipe();
            AddRecipe.ShowDialog();
            UpdateDataTableRecipe();
        }

        private void splitContainer4_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width == (sender as Panel).Width && e.ClipRectangle.Height == (sender as Panel).Height)
            {
                Pen p = new Pen(Brushes.Black, 2);
                e.Graphics.DrawRectangle(p, e.ClipRectangle);
            }
        }

        private void splitContainer5_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width == (sender as Panel).Width && e.ClipRectangle.Height == (sender as Panel).Height)
            {
                Pen p = new Pen(Brushes.Black, 2);
                e.Graphics.DrawRectangle(p, e.ClipRectangle);
            }
        }

        private void splitContainer5_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width == (sender as Panel).Width && e.ClipRectangle.Height == (sender as Panel).Height)
            {
                Pen p = new Pen(Brushes.Black, 2);
                e.Graphics.DrawRectangle(p, e.ClipRectangle);
            }
        }

        private void DataGridRecipe_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridRecipe.Rows[DataGridRecipe.CurrentCell.RowIndex].Selected = true;
            UpdateDataTableProduct();
        }

        private void DataGridRecipe_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var cell = DataGridRecipe.SelectedRows[0].Cells[0];
            
            SqlConnection.SetSqlParameters(new List<SqlParameter> {
                new SqlParameter("@oldName", $@"{DataTableRecipeLast.Rows[cell.RowIndex].ItemArray[0]}%"),
                new SqlParameter("@newName", $@"{cell.Value}")
            });
            SqlConnection.ExecuteNonQuery(QueryUpdateValueDishName);
            UpdateDataTableRecipe();
        }
    }
}
