using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using CanteenMonitorAndroid.Model;

namespace CanteenMonitorAndroid.ViewModels
{
    public class RecipeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Recipe> _recipes;
        public ObservableCollection<Recipe> Recipes
        {
            get { return _recipes; }
            set { _recipes = value; }
        }
        public RecipeViewModel()
        {
            Recipes = new ObservableCollection<Recipe>();
            string QueryUpdateProductList = "select DishList.Name as Блюдо from ProductionSale " +
            "left join DishList on DishList.Id = ProductionSale.dish where date = @date group by DishList.Name, date";
            var SqlConnection = new SQL();
            SqlConnection.SetSqlParameters(new List<SqlParameter>()
            {
                new SqlParameter("@date", DateTime.Now)
            });
            using (var reader = SqlConnection.ExecuteQuery(QueryUpdateProductList))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Recipes.Add(new Recipe { NameDish = reader.GetString(0) });
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ((INotifyPropertyChanged)_recipes).PropertyChanged += value;
            }

            remove
            {
                ((INotifyPropertyChanged)_recipes).PropertyChanged -= value;
            }
        }
    }
}
