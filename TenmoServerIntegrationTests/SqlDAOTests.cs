using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace TenmoServerIntegrationTests
{
    public abstract class SqlDAOTests
    {
        const string connectionString = "Server=.\\SQLEXPRESS;Database=tenmo_test;Trusted_Connection=True;";


        public void SetupDB()
        {
            string path = "SetupTestData.sql";
            string setupScript = File.ReadAllText(path);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(setupScript, conn);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
