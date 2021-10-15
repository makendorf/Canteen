using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class AddProductionSale : MetroForm
    {
        private int TypeOperation;
        public AddProductionSale()
        {
            InitializeComponent();
        }
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateDishProduction = "select name as Блюда from DishList order by name";
        private readonly string QuerySearchDishProduction = "select name as Блюда from DishList where name like @dishName order by name";
        private readonly string QueryTypeOperation =
            "select * from TypeOperation";
        private readonly string QueryUpdateDishSale =
            "select _dish.Name as Блюда from ProductionSale " +
            "left join DishList as _dish on _dish.Id = ProductionSale.dish " +
            "where date = @date and type = @type " +
            "order by name";
        private readonly string QuerySearchDishSale =
            "select _dish.Name as Блюда from ProductionSale " +
            "left join DishList as _dish on _dish.Id = ProductionSale.dish " +
            "where date = @date and _dish.name like @dishName and type = @type " +
            "order by name";
        private readonly string QueryInsertProductionSale =
            "insert into ProductionSale (type, date, dish, quantity) values " +
            "(@typeOperation, " +
            "@date, " +
            "(select top 1 Id from DishList where name like @dishName), " +
            "@quantity)";
        private readonly string QueryInsertMovement =
            "insert into Movement (type, date, dish, product, quantity) values " +
            "(@typeOperation, " +
            "@date, " +
            "(select top 1 Id from DishList where name like @dishName), " +
            "@productId, " +
            "@quantity)";
        private readonly string QueryFindProductFromDish =
            "select product, norm, prod.category from RecipeList " +
            "left join ProductsList as prod on prod.Id = RecipeList.product " +
            "where dish = (select TOP 1 Id from DishList where name like @dishName) order by RecipeList.product";

        private DataTable DataTableDish = new DataTable();
        private DataTable DataTableAddDish = new DataTable();


        private SqlDataAdapter DataAdapterDish;
        private void AddProduction_Load(object sender, System.EventArgs e)
        {
            metroDateTime1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            
            dataGridView1.DataSource = new BindingSource(DataTableAddDish, null);
            GridViewDishList.DataSource = new BindingSource(DataTableDish, null);
            DataTableAddDish.Columns.Add("Блюдо");
            DataTableAddDish.Columns.Add("Количество порций");
            UpdateCBTypeOperation();
            UpdateDishGrid();
        }

        private void UpdateCBTypeOperation()
        {
            using(var reader = SqlConnection.ExecuteQuery(QueryTypeOperation))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        metroComboBox1.Items.Add(reader.GetString(1));
                    }
                }
            }
            metroComboBox1.SelectedIndex = TypeOperation = 1;
        }
        private void InsertProductionInDataTable()
        {
            DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryUpdateDishProduction);
        }
        private void UpdateDishGrid()
        {

            DataTableDish.Clear();
            switch (TypeOperation)
            {
                case 0:
                    {
                        InsertProductionInDataTable();
                        break;
                    }
                case 1:
                    {
                        InsertProductionInDataTable();
                        break;
                    }
                case 2:
                    {
                        InsertProductionInDataTable();
                        break;
                    }
                case 3:
                    {
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryUpdateDishSale);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@date", metroDateTime1.Value);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@type", 1);
                        break;
                    }
                case 4:
                    {
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryUpdateDishSale);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@date", metroDateTime1.Value);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@type", 2);
                        break;
                    }
            }
            DataAdapterDish.Fill(DataTableDish);
        }

        private void GridViewDishList_DoubleClick(object sender, EventArgs e)
        {
            using (var selectQuantityForm = new SelectQuantity("Колличество порций"))
            {
                selectQuantityForm.Text = GridViewDishList.SelectedCells[0].Value.ToString();
                if (selectQuantityForm.ShowDialog() == DialogResult.OK)
                {
                    if (GridViewDishList.SelectedRows.Count > 0)
                    {
                        var Value = selectQuantityForm.ReturnValue;
                        var Name = GridViewDishList.Rows[GridViewDishList.SelectedRows[0].Index].Cells[0].Value;

                        var _row = DataTableAddDish.NewRow();
                        _row["Блюдо"] = Name;
                        _row["Количество порций"] = Value;
                        DataTableAddDish.Rows.Add(_row);
                    }
                    else
                    {
                        var Value = selectQuantityForm.ReturnValue;
                        var Name = GridViewDishList.SelectedCells[0].Value;

                        var _row = DataTableAddDish.NewRow();
                        _row["Блюдо"] = Name;
                        _row["Количество порций"] = Value;
                        DataTableAddDish.Rows.Add(_row);
                    }
                }
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            try
            {
                var date = metroDateTime1.Value;
                for (int i = 0; i < DataTableAddDish.Rows.Count; i++)
                {
                    var dishName = DataTableAddDish.Rows[i].ItemArray[0];
                    double quantity = Convert.ToDouble(DataTableAddDish.Rows[i].ItemArray[1]);
                    SqlConnection.SetSqlParameters(new List<SqlParameter>
                    {
                        new SqlParameter("@typeOperation", TypeOperation),
                        new SqlParameter("@date",  date),
                        new SqlParameter("@dishName",  dishName),
                        new SqlParameter("@quantity",  quantity),
                    });
                    SqlConnection.ExecuteNonQuery(QueryInsertProductionSale);

                    var SqlConnProduct = new SQL();
                    SqlConnProduct.SetSqlParameters(new List<SqlParameter>
                    {
                        new SqlParameter("@dishName", dishName)
                    });
                    using (var reader = SqlConnProduct.ExecuteQuery(QueryFindProductFromDish))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var SqlConnMove = new SQL();
                                var productId = reader.GetInt32(0);
                                var category = reader.GetInt32(2);
                                dynamic quantityKg;
                                switch (category)
                                {
                                    case 10008:
                                        {
                                            quantityKg = Math.Round(quantity, 3);
                                            break;
                                        }
                                    default:
                                        {
                                            quantityKg = Math.Round(reader.GetDouble(1) * quantity, 3);
                                            break;
                                        }
                                }
                                switch (category)
                                {
                                    case 10008:
                                        {
                                            quantityKg = Math.Round(quantity, 3);
                                            break;
                                        }
                                    default:
                                        {
                                            switch (TypeOperation)
                                            {
                                                case 1:
                                                case 2:
                                                    {
                                                        quantityKg = Math.Round(reader.GetDouble(1) * quantity, 3);
                                                        break;
                                                    }
                                                case 3:
                                                case 4:
                                                    {
                                                        int quantityProductionLastDay = 0;
                                                        int quantityProductionCurrentDay = 0;
                                                        using (var readerProductionCurrentDay = new SQL().ExecuteQuery(
                                                                $@"select sum(quantity) from ProductionSale " +
                                                                $@"where date = '{date}' and " +
                                                                $@"dish = (select top 1 Id from DishList where name like {dishName}) and " +
                                                                $@"type = 2"))
                                                        {
                                                            readerProductionCurrentDay.Read();
                                                            quantityProductionCurrentDay = readerProductionCurrentDay.GetInt32(0);
                                                            using (var readerProductionLastDay = new SQL().ExecuteQuery(
                                                                $@"select top 1 " +
                                                                $@"(select sum(quantity) from ProductionSale " +
                                                                $@"where date = '{date.AddDays(-1)}' and " +
                                                                $@"dish = (select top 1 Id from DishList where name like {dishName}) and " +
                                                                $@"type = 2) - " +
                                                                $@"(select sum(quantity) from ProductionSale " +
                                                                $@"where date = '13.10.2021' and " +
                                                                $@"dish = 10031 and " +
                                                                $@"type = 4) " +
                                                                $@"from ProductionSale  "))
                                                            {
                                                                if (readerProductionLastDay.HasRows)
                                                                {
                                                                    quantityProductionLastDay = readerProductionLastDay.GetInt32(0);
                                                                    if (quantityProductionLastDay < 0) quantityProductionLastDay = 0;
                                                                }
                                                                else quantityProductionLastDay = 0;
                                                            }
                                                        }
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                }

                                SqlConnMove.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@typeOperation", TypeOperation),
                                    new SqlParameter("@date", date),
                                    new SqlParameter("@dishName", dishName),
                                    new SqlParameter("@productId", productId),
                                    new SqlParameter("@quantity", quantityKg)
                                });
                                SqlConnMove.ExecuteNonQuery(QueryInsertMovement);
                            }
                        }
                    }
                }
            }
            finally
            {
                MessageBox.Show("Успешно");
            }
            
        }

        private void metroDateTime1_ValueChanged(object sender, EventArgs e)
        {
            if(TypeOperation == 2)
            {
                UpdateDishGrid();
            }
        }
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            switch (TypeOperation)
            {
                case 1:
                    {
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QuerySearchDishProduction);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
                case 2:
                    {
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QuerySearchDishProduction);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
                case 3:
                    {
                        var date = metroDateTime1.Value.ToShortDateString();
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QuerySearchDishSale);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@date", date);
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
                case 4:
                    {
                        var date = metroDateTime1.Value.ToShortDateString();
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QuerySearchDishSale);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@date", date);
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
            }
            
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TypeOperation = metroComboBox1.SelectedIndex;
            UpdateDishGrid();
        }
    }
}
