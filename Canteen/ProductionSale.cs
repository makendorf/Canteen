using MetroFramework.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class ProductionSale : Form
    {
        public ProductionSale()
        {
            InitializeComponent();
        }
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateProductionSale = "select typeOperation.name as Тип, date as Дата, dishList.name as Блюдо, quantity as Колличество from ProductionSale " +
            "left join TypeOperation as typeOperation on typeOperation.Id = ProductionSale.type " +
            "left join DishList as dishList on dishList.Id = ProductionSale.dish " +
            "Order by date desc";


        private DataTable DataTableProductionSale = new DataTable();

        private SqlDataAdapter DataAdapterProductionSale;

        private void AddProductionSale_Load(object sender, System.EventArgs e)
        {
            dataGridView1.DataSource = new BindingSource(DataTableProductionSale, null);
            UpdateDGProductionSale();
        }

        private void UpdateDGProductionSale()
        {
            DataTableProductionSale.Clear();
            DataAdapterProductionSale = SqlConnection.QueryForDataAdapter(QueryUpdateProductionSale);
            DataAdapterProductionSale.Fill(DataTableProductionSale);
            dataGridView1.Columns[0].Width = dataGridView1.Columns[2].Width = 200;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            var productionsForm = new AddProductionSale();
            productionsForm.ShowDialog();
            UpdateDGProductionSale();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            var productionsForm = new AddProductionSale();
            productionsForm.ShowDialog();
            UpdateDGProductionSale();
        }
    }
}
