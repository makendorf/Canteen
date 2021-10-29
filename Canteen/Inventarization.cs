using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Canteen
{
    public partial class Inventarization : Form
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateInventirization =
                                "select * from dbo.Reweigh order by Reweigh.Продукт";
        private readonly DataTable DataTableInventirization = new DataTable();

        private SqlDataAdapter DataAdapterInventirization;
        public Inventarization()
        {
            InitializeComponent();
        }

        private void Inventarization_Load(object sender, EventArgs e)
        {
            dataGridInventarization.DataSource = new BindingSource(DataTableInventirization, null);
            dataGridInventarization.ReadOnly = true;
            UpdateDataTableInventarization();
        }
        private void UpdateDataTableInventarization()
        {
            DataTableInventirization.Clear();
            DataAdapterInventirization = SqlConnection.QueryForDataAdapter(QueryUpdateInventirization);
            DataAdapterInventirization.Fill(DataTableInventirization);
            Program.ReWeighTable = DataTableInventirization;
            if (DataTableInventirization.Rows.Count > 0)
            {
                dataGridInventarization.Rows[DataTableInventirization.Rows.Count - 1].Selected = true;
            }
        }
    }
}
