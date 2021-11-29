using System;
using Xamarin.Forms;
using CanteenMonitorAndroid.ViewModels;

namespace CanteenMonitorAndroid
{


    public partial class MainPage : ContentPage
    {
        private ViewCell lastCell;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new RecipeViewModel();
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.White;
                lastCell = viewCell;
            }
        }
    }
    
}
