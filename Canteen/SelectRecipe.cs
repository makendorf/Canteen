using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canteen
{
    public partial class SelectRecipe : MetroForm
    {
        SQL SqlConnection = new SQL();
        private readonly string QueryUpdateProductOfRecipe =
                               "select products.name as Продукт, RecipeList.norm as 'Норма (кг/1 порция)' from RecipeList " +
                               "left join DishList as dishs on dishs.Id = RecipeList.dish " +
                               "left join ProductsList as products on products.Id = RecipeList.product " +
                               "where RecipeList.dish = (select top 1 Id from DishList where name like @nameDish) " +
                               "order by products.name";
        private DataTable DataTableRecipe = new DataTable();
        private SqlDataAdapter DataAdapterProductOfRecipe;
        public SelectRecipe()
        {
            InitializeComponent();
            
        }

        private void SelectRecipe_Load(object sender, EventArgs e)
        {
            dataGridView1.DataError += new DataGridViewDataErrorEventHandler(DataGridView1_DataError);
            dataGridView1.DataSource = new BindingSource(DataTableRecipe, null);
            UpdateDataTableRecipe();
            metroTextBox1.Text = Text;
        }
        private void UpdateDataTableRecipe()
        {
            DataTableRecipe.Clear();
            DataAdapterProductOfRecipe = SqlConnection.QueryForDataAdapter(QueryUpdateProductOfRecipe);
            DataAdapterProductOfRecipe.SelectCommand.Parameters.AddWithValue("@nameDish", Text);
            DataAdapterProductOfRecipe.Fill(DataTableRecipe);
            dataGridView1.Columns[0].Width = 200;
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 0:
                    {
                        AddProduct addProductForm = new AddProduct(true);
                        addProductForm.ShowDialog();
                        dataGridView1.CurrentCell.Value = addProductForm.ProductValue == "" ? dataGridView1.CurrentCell.Value : addProductForm.ProductValue;
                        break;
                    }
                    case 1:
                    {
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = true;
                        break;
                    }
            }
            if(dataGridView1.CurrentCell.ColumnIndex == 0)
            {
               
            }
        }
        public void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            anError.ThrowException = false;
            MessageBox.Show("Ошибка ввода! Убедитесь, что стоит запятая, а не точка.");
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            int dishID = 0;
            try
            {
                SqlConnection.BeginTransaction();
                SqlConnection.SetSqlParameters(new List<SqlParameter>()
                {
                    new SqlParameter("@dishName", Text)
                });
                using (var reader = SqlConnection.ExecuteQuery("select top 1 id from DishList where name like @dishName"))
                {
                    if (reader.Read())
                    {
                        dishID = reader.GetInt32(0);
                    }
                }
                SqlConnection.SetSqlParameters(new List<SqlParameter>()
                {
                    new SqlParameter("@dishId", dishID)
                });
                SqlConnection.ExecuteNonQuery("delete from RecipeList where dish = @dishId");

                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    SqlConnection.SetSqlParameters(new List<SqlParameter>()
                    {
                        new SqlParameter("@dishId", dishID),
                        new SqlParameter("@prodName", dataGridView1.Rows[i].Cells[0].Value.ToString()),
                        new SqlParameter("@norm", Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value))
                    });
                    SqlConnection.ExecuteNonQuery("insert into RecipeList (dish, product, norm) values (@dishId, (select top 1 Id from ProductsList where name like @prodName), @norm)");
                }
                if(metroTextBox1.Text != Text)
                {
                    SqlConnection.SetSqlParameters(new List<SqlParameter>()
                    {
                        new SqlParameter("dishId", dishID),
                        new SqlParameter("newDishName", metroTextBox1.Text)
                    });
                    SqlConnection.ExecuteNonQuery("update DishList set name = @newDishName where id = @dishId");
                }
                
                SqlConnection.Commit();
            }
            catch
            {
                SqlConnection.RollBack();
            }
            
        }
    }
}
