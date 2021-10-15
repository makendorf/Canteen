﻿using System;
using System.Windows.Forms;

namespace Canteen
{
    public partial class Report : Form
    {
        SQL SqlConnection = new SQL();
        private int TypeOperation;
        private readonly string QueryTypeOperation =
            "select * from TypeOperation";
        public Report()
        {
            InitializeComponent();
        }
        private void UpdateCBTypeOperation()
        {
            using (var reader = SqlConnection.ExecuteQuery(QueryTypeOperation))
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
        private void metroButton1_Click(object sender, EventArgs e)
        {
            ReportXLMS report = new ReportXLMS(TypeOperation, metroDateTime1.Value);
            report.BookkeepingReport();
        }
        private void Report_Load(object sender, EventArgs e)
        {
            UpdateCBTypeOperation();
            metroComboBox1.SelectedIndex = 1;
        }
        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TypeOperation = metroComboBox1.SelectedIndex;
        }
    }
}