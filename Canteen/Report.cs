using System;
using System.Windows.Forms;

namespace Canteen
{
    public partial class Report : Form
    {
        private readonly SQL SqlConnection = new SQL();
        private int TypeReport;
        private readonly string QueryTypeOperation =
            "select * from TypeReports";
        public Report()
        {
            InitializeComponent();
        }
        private void UpdateCBTypeOperation()
        {
            using (System.Data.SqlClient.SqlDataReader reader = SqlConnection.ExecuteQuery(QueryTypeOperation))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        metroComboBox1.Items.Add(reader.GetString(1));
                    }
                }
            }
            metroComboBox1.SelectedIndex = TypeReport = 1;
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            
            switch (TypeReport)
            {
                case 1:
                    {
                        ReportXLMS report = new ReportXLMS(TypeReport, metroDateTime1.Value);
                        report.BookkeepingReport();
                        break;
                    }
                case 2: goto case 1;
                case 3: goto case 1;
                case 4: goto case 1;
                case 6:
                    {
                        ReportXLMS report = new ReportXLMS(TypeReport, metroDateTime1.Value);
                        report.Reweigh();
                        break;
                    }
            }
        }
        private void Report_Load(object sender, EventArgs e)
        {
            UpdateCBTypeOperation();
            metroComboBox1.SelectedIndex = 1;
        }
        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TypeReport = metroComboBox1.SelectedIndex;
        }
    }
}
