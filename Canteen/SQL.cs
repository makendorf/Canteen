﻿using System.Collections.Generic;
using System.Data.SqlClient;

namespace Canteen
{
    public class SQL
    {
        private SqlConnection SqlConnection { get; set; }
        private SqlCommand SqlCommand { get; set; }
        public SQL()
        {
            SqlConnection = new SqlConnection(GetConnectionString());
            SqlCommand = new SqlCommand("", SqlConnection);
            if (SqlConnection.State == System.Data.ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }
        private string GetConnectionString()
        {
            var connString = Properties.Settings.Default.canteenConnectionString;
            return connString;
        }
        public SqlDataReader ExecuteQuery(string cmd = "")
        {
            if (cmd != "")
            {
                SqlCommand.CommandText = cmd;
            }
            else
            {
                SqlCommand = new SqlCommand(cmd, SqlConnection);
            }

            return SqlCommand.ExecuteReader();
        }
        public int ExecuteNonQuery(string cmd = "")
        {
            if(cmd != "")
            {
                SqlCommand.CommandText = cmd;
            }
            return SqlCommand.ExecuteNonQuery();
        }
        public void SetSqlCommand(string cmd)
        {
            SqlCommand.CommandText = cmd;
        }
        public SqlDataAdapter QueryForDataAdapter(string cmd = "")
        {
            SqlCommand = new SqlCommand(cmd, SqlConnection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(SqlCommand);
            return dataAdapter;
        }
        public void SetSqlParameters(List<SqlParameter> sqlParameterCollection)
        {
            SqlCommand.Parameters.Clear();
            SqlCommand.Parameters.AddRange(sqlParameterCollection.ToArray());
            for(int i = 0; i < SqlCommand.Parameters.Count; i++)
            {
                if(SqlCommand.Parameters[i].SqlDbType == System.Data.SqlDbType.DateTime)
                {
                    SqlCommand.Parameters[i].SqlDbType = System.Data.SqlDbType.Date;
                }
            }
        }
        public void Close()
        {
            SqlConnection.Close();
        }


        
    }
}