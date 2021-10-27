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
            "insert into ProductionSale (type, date, dish, quantity, remains) values " +
            "(@typeOperation, " +
            "@date, " +
            "(select top 1 Id from DishList where name like @dishName), " +
            "@quantity," +
            "@remains)";
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
        private readonly string QueryFindSummRemains = "select sum(remains) from ProductionSale " +
            $@"where (date = @date or date = @dateLast) and " +
            $@"dish = (select top 1 Id from DishList where name like @dishName) and " +
            $@"type = @type";
        private readonly string QueryFindRemains = 
            "select Id, remains, date from ProductionSale " +
            $@"where (date = @date or date = @dateLast) and " +
            $@"dish = (select top 1 Id from DishList where name like @dishName) and " +
            $@"type = @type order by date asc";
        private readonly string QueryUpdateRemainsForProduction = 
            "update ProductionSale set remains = @remains where Id = @id";
        private readonly string QueryUpdateProductQuantity =
            "update ProductsQuantity set quantity = quantity + @quantity " +
            "where product_id = (select top 1 Id from ProductsList where name like @name)";
        private readonly string QueryInsertMovementСomingProduct =
            "insert into Movement (type, date, product, quantity) values " +
            "(@typeOperation, " +
            "@date, " +
            "(select top 1 Id from ProductsList where name like @name), " +
            "@quantity)";
        private readonly string QueryUpdateProductComming =
            "select name as Продукт from ProductsList";
        private readonly string QueryFindProductComming =
            "select name as Продукт from ProductsList where name like @name";

        private DataTable DataTableDish = new DataTable();
        private DataTable DataTableAddDish = new DataTable();


        private SqlDataAdapter DataAdapterDish;
        private void AddProduction_Load(object sender, EventArgs e)
        {
            metroDateTime1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            UpdateCBTypeOperation();
            dataGridView1.DataSource = new BindingSource(DataTableAddDish, null);
            GridViewDishList.DataSource = new BindingSource(DataTableDish, null);
            UpdateHeaderAddDishTable();

            UpdateDishGrid();
        }

        private void UpdateHeaderAddDishTable()
        {
            DataTableAddDish = new DataTable();
            dataGridView1.DataSource = new BindingSource(DataTableAddDish, null);
            switch (TypeOperation)
            {
                case 5:
                    {
                        DataTableAddDish.Columns.Add("Продукт");
                        DataTableAddDish.Columns.Add("Количество");
                        break;
                    }
                case 6: goto case 5;
                default:
                    {
                        DataTableAddDish.Columns.Add("Блюдо");
                        DataTableAddDish.Columns.Add("Количество порций");
                        break;
                    }
            }
        }

        private void UpdateCBTypeOperation()
        {
            using (SqlDataReader reader = SqlConnection.ExecuteQuery(QueryTypeOperation))
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
            switch (TypeOperation)
            {
                case 0: goto case 2;
                case 1: goto case 2;
                case 2:
                    {
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryUpdateDishProduction);
                        break;
                    }
                case 3: goto case 4;
                case 4:
                    {
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryUpdateDishSale);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@date", metroDateTime1.Value);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@type", SelectTypeProductionInTypeSale());
                        break;
                    }
                case 5:
                    {
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryUpdateProductComming);
                        break;
                    }
                case 6: goto case 5;

            }
           
        }
        private void UpdateDishGrid()
        {
            DataTableDish = new DataTable();
            GridViewDishList.DataSource = new BindingSource(DataTableDish, null);
            DataTableDish.Clear();
            InsertProductionInDataTable();
            DataAdapterDish.Fill(DataTableDish);
            GridViewDishList.Columns[0].Width = 200;
        }

        private void GridViewDishList_DoubleClick(object sender, EventArgs e)
        {
            using (SelectQuantity selectQuantityForm = new SelectQuantity("Колличество порций"))
            {
                selectQuantityForm.Text = GridViewDishList.SelectedCells[0].Value.ToString();
                if (selectQuantityForm.ShowDialog() == DialogResult.OK)
                {
                    DataRow _row = DataTableAddDish.NewRow();
                    string Value = selectQuantityForm.ReturnValue;
                    object Name = GridViewDishList.SelectedCells[0].Value;
                    switch (TypeOperation)
                    {
                        case 5:
                            {
                                _row["Продукт"] = Name;
                                _row["Количество"] = Value;
                                break;
                            }
                        case 6: goto case 5;
                        default:
                            {
                                _row["Блюдо"] = Name;
                                _row["Количество порций"] = Value;
                                break;
                            }
                    }
                    DataTableAddDish.Rows.Add(_row);
                }
            }
        }
        private int SelectTypeProductionInTypeSale()
        {
            if (TypeOperation == 3)
            {
                return 1;
            }
            else if (TypeOperation == 4)
            {
                return 2;
            }
            return 0;
        }
        private void metroButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime date = metroDateTime1.Value;
                double summRemains = 0;
                SqlConnection.BeginTransaction();
                for (int i = 0; i < DataTableAddDish.Rows.Count; i++)
                {
                    object name = DataTableAddDish.Rows[i].ItemArray[0];
                    double quantity = Convert.ToDouble(DataTableAddDish.Rows[i].ItemArray[1].ToString().Replace(".", ","));
                    double remainsFact = quantity;
                    double remainsLastDay = 0;
                    switch (TypeOperation)
                    {
                        case 1: goto case 2;
                        case 2:
                            {
                                SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@typeOperation", TypeOperation),
                                    new SqlParameter("@date",  date),
                                    new SqlParameter("@dishName",  name),
                                    new SqlParameter("@quantity",  quantity),
                                    new SqlParameter("@remains", quantity)
                                });
                                SqlConnection.ExecuteNonQuery(QueryInsertProductionSale);
                                InsertMovement(date, name, quantity, remainsLastDay);
                                break;
                            }
                        case 3: goto case 4;
                        case 4:
                            {
                                int id = 0;
                                double remains = 0;
                                summRemains = 0;

                                SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@type", SelectTypeProductionInTypeSale()),
                                    new SqlParameter("@date",  date),
                                    new SqlParameter("@dishName",  name),
                                    new SqlParameter("@dateLast",  date.AddDays(-1))
                                });
                                using (SqlDataReader readerRemains = SqlConnection.ExecuteQuery(QueryFindSummRemains))
                                {
                                    readerRemains.Read();
                                    summRemains = readerRemains.GetDouble(0);
                                }
                                using (SqlDataReader readerRemains = SqlConnection.ExecuteQuery(QueryFindRemains))
                                {
                                    if (readerRemains.HasRows)
                                    {
                                        while (readerRemains.Read())
                                        {
                                            if (summRemains != 0)
                                            {
                                                if (remainsFact > summRemains)
                                                {
                                                    remainsFact = summRemains;
                                                }
                                                id = readerRemains.GetInt32(0);
                                                remains = readerRemains.GetDouble(1);

                                                remainsFact -= remains;
                                                if (readerRemains.GetDateTime(2).ToShortDateString() == date.AddDays(-1).ToShortDateString())
                                                {
                                                    remainsLastDay += remains;
                                                }
                                                SQL SqlChangeRemains = new SQL();
                                                if (remainsFact > 0)
                                                {

                                                    SqlChangeRemains.SetSqlParameters(new List<SqlParameter>
                                                    {
                                                        new SqlParameter("@id", id),
                                                        new SqlParameter
                                                        {
                                                            IsNullable = false,
                                                            DbType = DbType.Int64,
                                                            ParameterName = "@remains",
                                                            Value = 0
                                                        }
                                                    });

                                                }
                                                else if (remainsFact <= remains)
                                                {
                                                    SqlChangeRemains.SetSqlParameters(new List<SqlParameter>
                                                    {
                                                        new SqlParameter("@id", id),
                                                        new SqlParameter
                                                        {
                                                            IsNullable = false,
                                                            DbType = DbType.Int64,
                                                            ParameterName = "@remains",
                                                            Value = remains
                                                        }
                                                    });
                                                }
                                                SqlChangeRemains.ExecuteNonQuery(QueryUpdateRemainsForProduction);
                                                SqlChangeRemains.Close();
                                            }
                                        }
                                    }
                                }
                                remainsFact = Math.Abs(remainsFact);
                                SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@id", id),
                                    new SqlParameter
                                    {
                                        IsNullable = false,
                                        DbType = DbType.Int64,
                                        ParameterName = "@remains",
                                        Value = remainsFact
                                    }
                                });
                                SqlConnection.ExecuteNonQuery(QueryUpdateRemainsForProduction);
                                SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@typeOperation", TypeOperation),
                                    new SqlParameter("@date",  date),
                                    new SqlParameter("@dishName",  name),
                                    new SqlParameter("@quantity",  quantity),
                                    new SqlParameter("@remains", remainsFact)
                                });
                                SqlConnection.ExecuteNonQuery(QueryInsertProductionSale);
                                if (quantity > summRemains)
                                {
                                    quantity = summRemains;
                                }
                                InsertMovement(date, name, quantity, remainsLastDay);
                                break;
                            }
                        case 5:
                            {
                                SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@name",  name),
                                    new SqlParameter("@quantity",  quantity)
                                });
                                SqlConnection.ExecuteNonQuery(QueryUpdateProductQuantity);

                                SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@typeOperation", TypeOperation),
                                    new SqlParameter("@date",  date),
                                    new SqlParameter("@name",  name),
                                    new SqlParameter("@quantity",  quantity)
                                });
                                SqlConnection.ExecuteNonQuery(QueryInsertMovementСomingProduct);
                                break;
                            }
                        case 6:
                            {
                                InsertMovementReweighing(date, name, quantity);

                                break;
                            }
                    }
                    
                }
                SqlConnection.Commit();
                MessageBox.Show("Успешно");

            }
            catch (Exception exc)
            {
                SqlConnection.RollBack();
                MessageBox.Show(exc.Message);
            }
        }

        private void InsertMovementReweighing(DateTime date, object name, double remains)
        {
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@prodName", name)
            });
            using (var reader = SqlConnection.ExecuteQuery($"select top 1 Id from ProductsList where name like @prodName"))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    name = reader.GetInt32(0);
                }
            }
            double calcRemains = CalcDifference(date, name);
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@difference", remains - calcRemains),
                new SqlParameter("@remains", remains),
                new SqlParameter("@calcRemains", calcRemains),
                new SqlParameter("@date", date),
                new SqlParameter("@type", TypeOperation),
                new SqlParameter("@name", name)
            });
            SqlConnection.ExecuteNonQuery($"insert into Movement (type, date, product, quantity, difference, calcremains) values (@type, @date, @name, @remains, @difference, @calcRemains)");
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@quantity", remains),
                new SqlParameter("@name", name)
            });
            SqlConnection.ExecuteNonQuery("update ProductsQuantity set quantity = @quantity where product_id = @name");
        }

        private double CalcDifference(DateTime date, object name)
        {
            double lastReweighing = 0;
            double summComming = 0;
            double summProduction = 0;
            
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@prodName", name),
                new SqlParameter("@type", 6)
            });
            using (var reader = SqlConnection.ExecuteQuery($"select top 1 quantity from Movement where product = @prodName and type = @type order by date desc"))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    lastReweighing = reader.GetDouble(0);
                }
            }
            string _date = date.AddMonths(-1).ToString("yyyy-MM-") + "%";
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@date", _date),
                new SqlParameter("@prodName", name),
                new SqlParameter("@type", 5)
            });
            using (var reader = SqlConnection.ExecuteQuery($"select sum(quantity) from Movement where date like @date and product = @prodName and type = @type"))
            {
                try
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        summComming = reader.GetDouble(0);
                    }
                }
                catch { }
            }
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@date", _date),
                new SqlParameter("@prodName", name)
            });
            using (var reader = SqlConnection.ExecuteQuery($"select sum(quantity) from Movement where date like @date and product = @prodName and type in (1, 2)"))
            {
                try
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        summProduction = reader.GetDouble(0);
                    }
                }
                catch { }
            }
            return lastReweighing + summComming - summProduction;
        }

        private void InsertMovement(DateTime date, object dishName, double quantity, double remainsLastDay)
        {
            SQL SqlConnProduct = new SQL();
            SqlConnProduct.SetSqlParameters(new List<SqlParameter>
                    {
                        new SqlParameter("@dishName", dishName)
                    });
            using (SqlDataReader reader = SqlConnProduct.ExecuteQuery(QueryFindProductFromDish))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int productId = reader.GetInt32(0);
                        int category = reader.GetInt32(2);
                        dynamic quantityKg = 0;
                        
                        switch (category)
                        {
                            case 10008:
                                {
                                    quantityKg = quantity;
                                    SqlConnection.SetSqlParameters(new List<SqlParameter>()
                                    {
                                        new SqlParameter("@quantity", quantityKg),
                                        new SqlParameter("@name", productId)
                                    });
                                    SqlConnection.ExecuteNonQuery($"update ProductsQuantity set quantity = quantity - @quantity where product_id = @name");
                                    break;
                                }
                            default:
                                {
                                    switch (TypeOperation)
                                    {
                                        case 1: goto case 2;
                                        case 2:
                                            {
                                                quantityKg = Math.Round(reader.GetDouble(1) * quantity, 3);
                                                SqlConnection.SetSqlParameters(new List<SqlParameter>()
                                                {
                                                    new SqlParameter("@quantity", quantityKg),
                                                    new SqlParameter("@name", productId)
                                                });
                                                SqlConnection.ExecuteNonQuery($"update ProductsQuantity set quantity = quantity - @quantity where product_id = @name ");
                                                break;
                                            }
                                        case 3: goto case 4;
                                        case 4:
                                            {
                                                quantityKg = Math.Round(reader.GetDouble(1) * (quantity - remainsLastDay), 3);
                                                break;
                                            }
                                    }
                                    break;
                                }

                        }
                        SqlConnection.SetSqlParameters(new List<SqlParameter>
                                {
                                    new SqlParameter("@typeOperation", TypeOperation),
                                    new SqlParameter("@date", date),
                                    new SqlParameter("@dishName", dishName),
                                    new SqlParameter("@productId", productId),
                                    new SqlParameter("@quantity", quantityKg)
                                });
                        SqlConnection.ExecuteNonQuery(QueryInsertMovement);
                    }
                }
            }
        }

        private void metroDateTime1_ValueChanged(object sender, EventArgs e)
        {
            if (TypeOperation == 2)
            {
                UpdateDishGrid();
            }
        }
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            DataTableDish = new DataTable();
            GridViewDishList.DataSource = new BindingSource(DataTableDish, null);
            switch (TypeOperation)
            {
                case 1: goto case 2;
                case 2:
                    {
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QuerySearchDishProduction);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
                case 3: goto case 4;
                case 4:
                    {
                        string date = metroDateTime1.Value.ToShortDateString();
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QuerySearchDishSale);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@dishName", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@date", date);
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
                case 5:
                    {
                        DataTableDish.Clear();
                        DataAdapterDish = SqlConnection.QueryForDataAdapter(QueryFindProductComming);
                        DataAdapterDish.SelectCommand.Parameters.AddWithValue("@name", $@"%{toolStripTextBox1.Text}%");
                        DataAdapterDish.Fill(DataTableDish);
                        break;
                    }
            }
            GridViewDishList.Columns[0].Width = 200;

        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TypeOperation = metroComboBox1.SelectedIndex;
            UpdateHeaderAddDishTable();
            UpdateDishGrid();
        }
    }
}
