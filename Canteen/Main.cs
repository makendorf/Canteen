using MetroFramework.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class Main : MetroForm
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateMovement =
                                "select typeOper.name as Операция, date as Дата, coalesce(Dish.name, prod.name) as Блюдо, Round(sum(Movement.quantity), 3) as КГ from Movement " +
                                "left join ProductsList as prod on prod.Id = Movement.product " +
                                "left join DishList as Dish on Dish.Id = Movement.dish " +
                                "left join TypeOperation as typeOper on typeOper.Id = Movement.type " +
                                "group by typeOper.name, coalesce(Dish.name, prod.name), date " +
                                "order by date desc";
        private readonly string QueryUpdateMovementProductionProduct =
                                "select prod.name as Продукт, Round(sum(Movement.quantity), 3) as КГ from Movement " +
                                "left join ProductsList as prod on prod.Id = Movement.product " +
                                "left join DishList as Dish on Dish.Id = Movement.dish " +
                                "where type = (select top 1 Id from TypeOperation where name like @type) and date = @date and Dish.name like @dishName " +
                                "group by prod.name " +
                                "order by prod.name";
        private readonly string QueryUpdateMovementProductionProductAsComming =
                                "select prod.name as Продукт, quantity as КГ from ProductsQuantity " +
                                "left join ProductsList as prod on prod.Id = ProductsQuantity.product_id " +
            "where ProductsQuantity.product_id = (select top 1 Id from ProductsList where name like @dishName)";
        private readonly string QueryUpdateInventirization =
                                "select quantity as 'Фактический остаток', calcremains 'Расчетный остаток', difference as 'Разница' from Movement " +
            "left join ProductsList as prod on prod.Id = Movement.product " +
            "left join TypeOperation as typeOper on typeOper.Id = Movement.type " +
            "where type = 6 and Movement.date = @date and Movement.product = (select top 1 Id from ProductsList where name like @dishName) " +
            "order by date desc";


        private readonly DataTable DataTableMovement = new DataTable();
        private DataTable DataTableMovementProductionProduct = new DataTable();

        private SqlDataAdapter DataAdapterMovement;
        private SqlDataAdapter DataAdapterMovementProductionProduct;
        public Main()
        {
            InitializeComponent();
            //using (DbDataReader reader = SqlConnection.ExecuteQuery("select name from CategoryList"))
            //{
            //    while (reader.Read())
            //    {
            //        MessageBox.Show(reader.GetString(0));
            //    }
            //}

        }

        private void Main_Load(object sender, System.EventArgs e)
        {
            movementProductionProductDataGrid.DataSource = new BindingSource(DataTableMovementProductionProduct, null);
            movementDataGrid.DataSource = new BindingSource(DataTableMovement, null);
            UpdateDataTableMovement();
        }
        public void UpdatePanel()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        UpdateDataTableMovement();
                        break;
                    }
                case 1:
                    {
                        ProductAndRecipe productAndRecipeForm = new ProductAndRecipe()
                        {
                            Dock = DockStyle.Fill,
                            TopLevel = false,
                            FormBorderStyle = FormBorderStyle.None
                        };
                        tabControl1.TabPages[1].Controls.Add(productAndRecipeForm);
                        productAndRecipeForm.Show();
                        break;
                    }
                case 2:
                    {
                        ProductionSale productionSaleForm = new ProductionSale()
                        {
                            Dock = DockStyle.Fill,
                            TopLevel = false,
                            FormBorderStyle = FormBorderStyle.None
                        };
                        tabControl1.TabPages[2].Controls.Add(productionSaleForm);
                        productionSaleForm.Show();
                        break;
                    }
                case 3:
                    {
                        Report reportForm = new Report()
                        {
                            Dock = DockStyle.Fill,
                            TopLevel = false,
                            FormBorderStyle = FormBorderStyle.None,
                            StartPosition = FormStartPosition.CenterParent
                        };
                        tabControl1.TabPages[3].Controls.Add(reportForm);
                        reportForm.Show();
                        break;
                    }
                case 4:
                    {
                        Inventarization inventarizationForm = new Inventarization()
                        {
                            Dock = DockStyle.Fill,
                            TopLevel = false,
                            FormBorderStyle = FormBorderStyle.None,
                            StartPosition = FormStartPosition.CenterParent
                        };
                        tabControl1.TabPages[4].Controls.Add(inventarizationForm);
                        inventarizationForm.Show();
                        break;
                    }
            }
        }
        private void UpdateDataTableMovement()
        {
            DataTableMovement.Clear();
            //DataAdapterMovement.SelectCommand.Parameters.AddWithValue("@headName", "prod.name, ");
            //DataAdapterMovement.SelectCommand.Parameters.AddWithValue("@name", ", prod.name ");
            DataAdapterMovement = SqlConnection.QueryForDataAdapter(QueryUpdateMovement);
            DataAdapterMovement.Fill(DataTableMovement);
            movementDataGrid.Columns[0].Width = movementDataGrid.Columns[2].Width = 200;
        }
        private void UpdateDataTableMovementProductionProduct()
        {
            DataTableMovementProductionProduct = new DataTable();
            movementProductionProductDataGrid.DataSource = new BindingSource(DataTableMovementProductionProduct, null);
            string type = movementDataGrid.SelectedRows[0].Cells[0].Value.ToString().Trim();
            switch (type)
            {
                case "Без операции": { break; }
                case "Производство (малый зал)":
                    {
                        DataAdapterMovementProductionProduct = SqlConnection.QueryForDataAdapter(QueryUpdateMovementProductionProduct);
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{movementDataGrid.SelectedRows[0].Cells[2].Value.ToString().Trim()}%");
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@type", $@"%{movementDataGrid.SelectedRows[0].Cells[0].Value.ToString().Trim()}%");
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@date", movementDataGrid.SelectedRows[0].Cells[1].Value);
                        DataAdapterMovementProductionProduct.Fill(DataTableMovementProductionProduct);
                        DataTableMovementProductionProduct.Rows.Add("Итого:", SummWeighOnePerson());
                        break;
                    }
                case "Производство (большой зал)": goto case "Производство (малый зал)";
                case "Продажа (малый зал)": goto case "Производство (малый зал)";
                case "Продажа (большой зал)": goto case "Производство (малый зал)";
                case "Приход":
                    {
                        DataAdapterMovementProductionProduct = SqlConnection.QueryForDataAdapter(QueryUpdateMovementProductionProductAsComming);
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{movementDataGrid.SelectedRows[0].Cells[2].Value.ToString().Trim()}%");
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@date", movementDataGrid.SelectedRows[0].Cells[1].Value);
                        DataAdapterMovementProductionProduct.Fill(DataTableMovementProductionProduct);
                        break;
                    }
                case "Перевеска": 
                    {
                        DataAdapterMovementProductionProduct = SqlConnection.QueryForDataAdapter(QueryUpdateInventirization);
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{movementDataGrid.SelectedRows[0].Cells[2].Value.ToString().Trim()}%");
                        DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@date", System.Convert.ToDateTime(movementDataGrid.SelectedRows[0].Cells[1].Value));
                        DataAdapterMovementProductionProduct.Fill(DataTableMovementProductionProduct);
                        break; 
                    }
            }
            
            
            
           
        }

        private double SummWeighOnePerson()
        {
            double summ = 0;
            for (int i = 0; i < DataTableMovementProductionProduct.Rows.Count; i++)
            {
                summ += (double)DataTableMovementProductionProduct.Rows[i].ItemArray[1];
            }

            return System.Math.Round(summ, 3);
        }

        private void movementDataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            UpdateDataTableMovementProductionProduct();
        }

        private void ButtonAddProduct_Click(object sender, System.EventArgs e)
        {
            AddProduct AddProduct = new AddProduct();
            AddProduct.ShowDialog();
        }

        private void metroButton1_Click(object sender, System.EventArgs e)
        {
            ProductAndRecipe AddRecipe = new ProductAndRecipe();
            AddRecipe.ShowDialog();
        }

        private void metroButton2_Click(object sender, System.EventArgs e)
        {
            ProductAndRecipe AddRecipe = new ProductAndRecipe();
            AddRecipe.ShowDialog();
            UpdateDataTableMovement();
        }

        private void metroButton3_Click(object sender, System.EventArgs e)
        {
            ProductionSale addProductionSaleForm = new ProductionSale();
            addProductionSaleForm.ShowDialog();
            UpdateDataTableMovement();
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdatePanel();
        }

        private void movementDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            movementDataGrid.Rows[movementDataGrid.CurrentCell.RowIndex].Selected = true;
            UpdateDataTableMovementProductionProduct();
        }
    }
}
