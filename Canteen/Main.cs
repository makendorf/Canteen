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
                                "select typeOper.name as Операция, date as Дата, Dish.name as Блюдо, Round(sum(Movement.quantity), 3) as КГ from Movement " +
                                "left join ProductsList as prod on prod.Id = Movement.product " +
                                "left join DishList as Dish on Dish.Id = Movement.dish " +
                                "left join TypeOperation as typeOper on typeOper.Id = Movement.type " +
                                "group by typeOper.name, Dish.name, date " +
                                "order by date desc";
        private readonly string QueryUpdateMovementProductionProduct =
                                "select prod.name as Продукт, Round(sum(Movement.quantity), 3) as КГ from Movement " +
                                "left join ProductsList as prod on prod.Id = Movement.product " +
                                "left join DishList as Dish on Dish.Id = Movement.dish " +
                                "where type = (select top 1 Id from TypeOperation where name like @type) and date = @date and Dish.name like @dishName " +
                                "group by prod.name " +
                                "order by prod.name";


        private DataTable DataTableMovement = new DataTable();
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

            UpdatePanel();

        }
        public void UpdatePanel()
        {
            ProductAndRecipe productAndRecipeForm = new ProductAndRecipe()
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            tabControl1.TabPages[1].Controls.Add(productAndRecipeForm);
            productAndRecipeForm.Show();

            ProductionSale productionSaleForm = new ProductionSale()
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            tabControl1.TabPages[2].Controls.Add(productionSaleForm);
            productionSaleForm.Show();

            Report reportForm = new Report()
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent
            };
            tabControl1.TabPages[3].Controls.Add(reportForm);
            reportForm.Show();
        }
        private void UpdateDataTableMovement()
        {
            DataTableMovement.Clear();
            DataAdapterMovement = SqlConnection.QueryForDataAdapter(QueryUpdateMovement);
            DataAdapterMovement.Fill(DataTableMovement);
            movementDataGrid.Columns[0].Width = movementDataGrid.Columns[2].Width = 200;
        }
        private void UpdateDataTableMovementProductionProduct()
        {
            DataTableMovementProductionProduct.Clear();
            DataAdapterMovementProductionProduct = SqlConnection.QueryForDataAdapter(QueryUpdateMovementProductionProduct);
            DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{movementDataGrid.SelectedRows[0].Cells[2].Value.ToString().Trim()}%");
            DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@type", $@"%{movementDataGrid.SelectedRows[0].Cells[0].Value.ToString().Trim()}%");
            DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@date", movementDataGrid.SelectedRows[0].Cells[1].Value);
            DataAdapterMovementProductionProduct.Fill(DataTableMovementProductionProduct);
            DataTableMovementProductionProduct.Rows.Add("Итого:", SummWeighOnePerson());
            //switch (movementDataGrid.SelectedRows[0].Cells[0].Value.ToString().Trim())
            //{
            //    case "Без операции": { break; }
            //    case "Производство": {
            //            DataAdapterMovementProductionProduct = SqlConnection.QueryForDataAdapter(QueryUpdateMovementProductionProduct);
            //            DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{movementDataGrid.SelectedRows[0].Cells[2].Value.ToString().Trim()}%");
            //            DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@type", $@"%{movementDataGrid.SelectedRows[0].Cells[0].Value.ToString().Trim()}%");
            //            DataAdapterMovementProductionProduct.SelectCommand.Parameters.AddWithValue("@date", $@"%{movementDataGrid.SelectedRows[0].Cells[1].Value.ToString().Trim()}%");
            //            DataAdapterMovementProductionProduct.Fill(DataTableMovementProductionProduct);
            //            DataTableMovementProductionProduct.Rows.Add("Итого:", SummWeighOnePerson());
            //            break;
            //        }
            //    case "Продажа": { break; }
            //    case "Приход": { break; }
            //    case "Инвентаризация": { break; }
            //}
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
            var AddProduct = new AddProduct();
            AddProduct.ShowDialog();
        }

        private void metroButton1_Click(object sender, System.EventArgs e)
        {
            var AddRecipe = new ProductAndRecipe();
            AddRecipe.ShowDialog();
        }

        private void metroButton2_Click(object sender, System.EventArgs e)
        {
            var AddRecipe = new ProductAndRecipe();
            AddRecipe.ShowDialog();
            UpdateDataTableMovement();
        }

        private void metroButton3_Click(object sender, System.EventArgs e)
        {
            var addProductionSaleForm = new ProductionSale();
            addProductionSaleForm.ShowDialog();
            UpdateDataTableMovement();
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                UpdateDataTableMovement();
            }
        }

        private void movementDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            movementDataGrid.Rows[movementDataGrid.CurrentCell.RowIndex].Selected = true;
            UpdateDataTableMovementProductionProduct();
        }

    }
}
