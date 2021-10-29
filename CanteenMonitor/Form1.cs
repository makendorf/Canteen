﻿using MetroFramework.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CanteenMonitor
{
    public partial class Form1 : MetroForm
    {
        private readonly SQL SqlConnection = new SQL();
        private readonly string QueryUpdateProductList = "select date, DishList.Name from ProductionSale " +
            "left join DishList on DishList.Id = ProductionSale.dish where date = @date";
        private readonly DataTable DataTableProductList = new DataTable();
        private SqlDataAdapter DataAdapterProductList;
        System.Timers.Timer timer = new System.Timers.Timer(3600000);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            metroGrid1.DataSource = new BindingSource(DataTableProductList, null);
            timer.Elapsed += UpdateDataTableProductList;
            timer.Start();
            UpdateDataTableProductList();
        }

        private void UpdateDataTableProductList(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                DataTableProductList.Clear();
                DataAdapterProductList = SqlConnection.QueryForDataAdapter(QueryUpdateProductList);
                DataAdapterProductList.SelectCommand.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                DataAdapterProductList.Fill(DataTableProductList);
                metroGrid1.Columns[1].Width = 800;
                metroGrid1.Columns[0].Width = 200;
                foreach (DataGridViewRow row in metroGrid1.Rows)
                {
                    row.Height = 50;
                }
            }
            catch { }
        }

        private void UpdateDataTableProductList()
        {
            try
            {
                DataTableProductList.Clear();
                DataAdapterProductList = SqlConnection.QueryForDataAdapter(QueryUpdateProductList);
                DataAdapterProductList.SelectCommand.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                DataAdapterProductList.Fill(DataTableProductList);
                metroGrid1.Columns[1].Width = 800;
                metroGrid1.Columns[0].Width = 200;
                foreach (DataGridViewRow row in metroGrid1.Rows)
                {
                    row.Height = 50;
                }
            }
            catch { }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            UpdateDataTableProductList();
        }
    }
}
