using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;
using static TenmoServer.Models.Enums;

namespace TenmoServerIntegrationTests
{
    [TestClass]
    public class TransferSqlDAOTests : SqlDAOTests
    {
        private const string connectionString = "Server=.\\SQLEXPRESS;Database=tenmo_test;Trusted_Connection=True;";
        private TransferSqlDAO dao;

        [TestInitialize]
        public void ArrangeForEachTest()
        {
            this.dao = new TransferSqlDAO(connectionString);

            SetupDB();
        }

        [TestMethod]
        public void GetTransfersByUserTest()
        {
            // Arrange
            int test1UserId = GetTest1UserId();
            // Act
            List<Transfer> actualResult = dao.GetTransfersByUser(test1UserId);

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(7, actualResult.Count);
            Assert.AreEqual(385, GetSumOfAmountsInList(actualResult));
        }

        [TestMethod]
        public void GetPendingByUserTest()
        {
            // Arrange
            int test1UserId = GetTest1UserId();

            // Act
            List<Transfer> actualResult = dao.GetPendingByUser(test1UserId);

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(2, actualResult.Count);
            Assert.AreEqual(110, GetSumOfAmountsInList(actualResult));
            Assert.IsTrue(actualResult[0].AccountFrom == test1UserId);
            Assert.IsTrue(actualResult[1].AccountFrom == test1UserId);

        }

        [TestMethod]
        public void AuthorizeTransferTest()
        {
            // (1, 1, 1, 3, 50),
            Transfer transferBeforeUpdate = null;
            // Arrange
            Transfer transfer = new Transfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string Sql = "SELECT * FROM transfers WHERE amount = 10;";
                    SqlCommand cmd = new SqlCommand(Sql, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    
                    if (rdr.Read())
                    {
                        transferBeforeUpdate = RowToObject(rdr);
                    }
                }
            }
            catch (SqlException ex)
            {

                throw;
            }

            transfer.TransferId = transferBeforeUpdate.TransferId;
            transfer.TransferStatus = (TransferStatus)2;
            transfer.TransferType = (TransferType)1;
            transfer.AccountFrom = transferBeforeUpdate.AccountFrom;
            transfer.AccountTo = transferBeforeUpdate.AccountTo;
            transfer.Amount = transferBeforeUpdate.Amount;

            // Act
            bool actualResult = dao.AuthorizeTransfer(transfer);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(990, GetBalanceByAccount(transfer.AccountFrom));
            Assert.AreEqual(1010, GetBalanceByAccount(transfer.AccountTo));
            
        }
        private decimal GetBalanceByAccount(int accountId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE account_id = @accountId;", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    decimal balance = Convert.ToDecimal(cmd.ExecuteScalar());

                    return balance;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }
        private static Transfer RowToObject(SqlDataReader rdr)
        {
            Transfer transferBeforeUpdate = new Transfer();
            transferBeforeUpdate.TransferId = Convert.ToInt32(rdr["transfer_id"]);
            transferBeforeUpdate.TransferType = (TransferType)Convert.ToInt32(rdr["transfer_type_id"]);
            transferBeforeUpdate.TransferStatus = (TransferStatus)Convert.ToInt32(rdr["transfer_status_id"]);
            transferBeforeUpdate.AccountFrom = Convert.ToInt32(rdr["account_from"]);
            transferBeforeUpdate.AccountTo = Convert.ToInt32(rdr["account_to"]);
            transferBeforeUpdate.Amount = Convert.ToDecimal(rdr["amount"]);
            return transferBeforeUpdate;
        }

        private decimal GetSumOfAmountsInList(List<Transfer> transfers)
        {
            decimal total = 0;

            foreach (Transfer transfer in transfers)
            {
                total += transfer.Amount;
            }

            return total;
        }
        private int GetTest1UserId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id FROM users WHERE username = 'test1';", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    return userId;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }


    }
}
