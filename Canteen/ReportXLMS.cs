using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
namespace Canteen
{
    internal class ReportXLMS
    {
        private readonly int TypeDocument;
        private readonly DateTime Date;
        private Excel.Application Kniga;
        private Excel.Worksheet List;
        private Excel.Range Range;
        private readonly string QueryUpdateDishSale =
                                "select _dish.Name as Блюда, ProductionSale.dish, sum(ProductionSale.quantity) as TotalQuantity from ProductionSale  " +
                                "left join DishList as _dish on _dish.Id = ProductionSale.dish " +
                                "where date = @date and type = @type " +
                                "group by _dish.Name, ProductionSale.dish";
        private readonly string QueryUpdateMovementDishProductAll =
                                "select prod.name as Продукт, Movement.product from Movement " +
                                "left join ProductsList as prod on prod.Id = Movement.product " +
                                "where type = @type and date = @date " +
                                "group by prod.name, Movement.product ";
        private readonly string QueryFindValueProductInDish =
                                "select quantity from Movement " +
                                "where type = @type and " +
                                "product = @idProduct and " +
                                "dish = @idDish and " +
                                "date = @date";
        private readonly string QueryFullQuantityProduct =
                                "select Round(sum(Movement.quantity), 3) as КГ from Movement " +
                                "left join ProductsList as prod on prod.Id = Movement.product " +
                                "left join DishList as Dish on Dish.Id = Movement.dish " +
                                "where type = @type and date = @date " +
                                "group by prod.name ";
        private readonly SQL SqlConnection = new SQL();
        public ReportXLMS(int typeDocument, DateTime date)
        {
            TypeDocument = typeDocument;
            Date = date;
            Kniga = new Excel.Application()
            {
                Visible = false
            };
            Kniga.Workbooks.Open($@"{Directory.GetCurrentDirectory()}\Resources\Reports\report.xlsx");
            List = (Excel.Worksheet)Kniga.Worksheets.get_Item(1);
            List.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
        }

        public void BookkeepingReport()
        {
            List<List<string>> ProductList = new List<List<string>>();
            List<List<string>> DishList = new List<List<string>>();

            List.Cells[1, 2] = "АО 'Губкинский мясокомбинат'";
            switch (TypeDocument)
            {
                case 1:
                    {
                        List.Cells[2, 3] = "Расчет потребности продуктов в малом зале столовой";
                        break;
                    }
                case 2:
                    {
                        List.Cells[2, 3] = "Отчет по расходу продуктов в большом зале столовой";
                        break;
                    }
                case 3:
                    {
                        List.Cells[2, 3] = "Отчет по расходу продуктов в малом зале столовой";
                        break;
                    }
                case 4:
                    {
                        List.Cells[2, 3] = "Отчет по расходу продуктов в большом зале столовой";
                        break;
                    }
            }


            List.Cells[3, 1] = "№п/п";
            List.Cells[3, 2] = "Наименование продуктов";

            SqlConnection.SetSqlParameters(new List<SqlParameter>
            {
                new SqlParameter("@date", Date.ToShortDateString()),
                new SqlParameter("@type", TypeDocument)
            });
            using (SqlDataReader reader = SqlConnection.ExecuteQuery(QueryUpdateDishSale))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DishList.Add(new List<string> { reader.GetString(0), reader.GetInt32(1).ToString(), reader.GetDouble(2).ToString() });
                    }
                    Range = GetRange(List.Cells[4, 3], List.Cells[4, DishList.Count + 2]);
                    Range.Cells.WrapText = true;
                    Range.Value2 = ListToArrayHorisontal(DishList);
                    FormatHead(DishList, Date);
                }
            }

            using (SqlDataReader reader = SqlConnection.ExecuteQuery(QueryUpdateMovementDishProductAll))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProductList.Add(new List<string> { reader.GetString(0), reader.GetInt32(1).ToString() });
                    }
                    Range = GetRange(List.Cells[5, 2], List.Cells[ProductList.Count + 4, 2]);
                    Range.Value = ListToArrayVertical(ProductList);
                }
            }



            for (int i = 3; i < DishList.Count + 3; i++)
            {
                for (int j = 5; j < ProductList.Count + 5; j++)
                {
                    SqlConnection.SetSqlParameters(new List<SqlParameter>
                    {
                        new SqlParameter("@idDish", Convert.ToInt32(DishList[i - 3][1])),
                        new SqlParameter("@idProduct", Convert.ToInt32(ProductList[j - 5][1])),
                        new SqlParameter("@date", Date),
                        new SqlParameter("@type", TypeDocument)
                    });
                    using (SqlDataReader reader = SqlConnection.ExecuteQuery(QueryFindValueProductInDish))
                    {
                        Kniga.Visible = true;
                        if (reader.HasRows)
                        {
                            double summPerDay = 0;
                            while (reader.Read())
                            {
                                summPerDay += reader.GetDouble(0);
                            }
                            List.Cells[j, i] = summPerDay;
                        }
                    }
                }
            }
            List.Cells[4, DishList.Count + 3] = "Итого, кг";
            SqlConnection.SetSqlParameters(new List<SqlParameter>
            {
                new SqlParameter("@date", Date),
                new SqlParameter("@type", TypeDocument)
            });
            using (SqlDataReader reader = SqlConnection.ExecuteQuery(QueryFullQuantityProduct))
            {
                if (reader.HasRows)
                {
                    for (int i = 5; reader.Read(); i++)
                    {
                        List.Cells[i, DishList.Count + 3] = reader.GetDouble(0);
                    }
                }
            }


            Kniga.Visible = true;
            for (int i = 1; i <= ProductList.Count; i++)
            {
                List.Cells[i + 4, 1] = i;
            }
            Range = GetRange(List.Cells[2, 1], List.Cells[ProductList.Count + 4, DishList.Count + 3]);
            FullBorders();
            Range = GetRange(List.Cells[3, 3], List.Cells[ProductList.Count + 4, DishList.Count + 2]);
            Range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            List.Cells[ProductList.Count + 6, 2] = "Заведущий столовой";
            List.Cells[ProductList.Count + 6, 4] = "___________/____________";

            SaveDocument();
            ClearCOM();
        }

        private void FullBorders()
        {
            Range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;
        }

        private void FormatHead(List<List<string>> DishList, DateTime Date)
        {
            List.Cells[1, DishList.Count + 2] = Date.ToShortDateString();
            Range = GetRange(List.Cells[2, 2], List.Cells[2, DishList.Count + 2]);
            Range.Merge();
            Range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //сами блюда
            for (int i = 3; i <= DishList.Count + 2; i++)
            {
                Range = GetRange(List.Cells[4, i], List.Cells[4, i]);
                Range.EntireColumn.ColumnWidth = 100;
                Range.EntireColumn.AutoFit();
                Range.EntireRow.AutoFit();
                Range.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

            }

            //наименование блюда
            List.Cells[3, 3] = "Наименование блюда";
            Range = GetRange(List.Cells[3, 3], List.Cells[3, DishList.Count + 2]);
            Range.Merge();
            Range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //№
            Range = GetRange(List.Cells[3, 1], List.Cells[4, 1]);
            Range.Merge();
            Range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //наименование продукта
            Range = GetRange(List.Cells[3, 2], List.Cells[4, 2]);
            Range.Merge();
            Range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            Range.EntireRow.AutoFit();
            Range.EntireColumn.AutoFit();
        }

        private string[,] ListToArrayHorisontal(List<List<string>> productList)
        {
            string[,] array = new string[1, productList.Count];
            for (int i = 0; i < productList.Count; i++)
            {
                array[0, i] = $"{productList[i][0]} {Environment.NewLine}({productList[i][2]} порций), кг";
            }
            return array;
        }
        private string[,] ListToArrayVertical(List<List<string>> productList)
        {
            string[,] array = new string[productList.Count, 1];
            for (int i = 0; i < productList.Count; i++)
            {
                array[i, 0] = productList[i][0];
            }
            return array;
        }

        public Excel.Range GetRange(Excel.Range range1, Excel.Range range2)
        {
            return List.Range[range1, range2];
        }
        public void SaveDocument()
        {
            
            string TypeName = "";
            using (SqlDataReader reader = SqlConnection.ExecuteQuery($@"select name from TypeOperation where Id = {TypeDocument}"))
            {
                reader.Read();
                TypeName = reader.GetString(0);
            }
            string path = $@"{Directory.GetCurrentDirectory()}\Resources\Reports\{Date.ToShortDateString()} {TypeName}.xlsx";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Kniga.Application.ActiveWorkbook.SaveAs(
                path,
                System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing,
                System.Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, System.Type.Missing, System.Type.Missing,
                System.Type.Missing, System.Type.Missing, System.Type.Missing);
            ClearCOM();
        }
        public void ClearCOM()
        {
            if (Kniga != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Kniga);
                Kniga = null;
            }
            if (List != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(List);
                List = null;
            }
            if (Range != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Range);
                Range = null;
            }

            GC.Collect();
        }
    }
}
