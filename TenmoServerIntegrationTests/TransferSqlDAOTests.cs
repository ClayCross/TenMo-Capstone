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
            int test1UserId = GetUserIdByUsername("test1");
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
            int test1UserId = GetUserIdByUsername("test1");

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
                        transfer = RowToObject(rdr);
                        transfer.TransferStatus = TransferStatus.Approved;
                    }
                }
            }
            catch (SqlException ex)
            {

                throw;
            }

            // Act
            bool actualResult = dao.AuthorizeTransfer(transfer);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(990, GetBalanceByAccount(transfer.AccountFrom));
            Assert.AreEqual(1010, GetBalanceByAccount(transfer.AccountTo));
            Assert.AreEqual(2, GetTransferStatusByTransferId(transfer.TransferId));
            
        }

        [TestMethod]
        public void RejectTransferTest()
        {
            
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
                        transfer = RowToObject(rdr);
                        transfer.TransferStatus = TransferStatus.Rejected;

                    }
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
            // Act
            bool actualResult = dao.RejectTransfer(transfer);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(1000, GetBalanceByAccount(transfer.AccountFrom));
            Assert.AreEqual(1000, GetBalanceByAccount(transfer.AccountTo));
            Assert.AreEqual(3, GetTransferStatusByTransferId(transfer.TransferId));

        }
        [TestMethod]
        public void CreateTransferTest()
        {
            // Arrange
            Transfer transfer = new Transfer();
            transfer.TransferType = TransferType.Send;
            transfer.TransferStatus = TransferStatus.Approved;
            transfer.AccountFrom = 1;
            transfer.AccountTo = 2;
            transfer.Amount = 20;

            int maxIdBeforeTransfer = GetMaxTransferId();

            // Act
            bool actualResult = dao.CreateTransfer(transfer);

            int maxIdAfterTransfer = GetMaxTransferId();
            Transfer returnedTranForMaxId = GetTransferByTransferId(maxIdAfterTransfer);
            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(980, GetBalanceByAccount(returnedTranForMaxId.AccountFrom));
            Assert.AreEqual(1020, GetBalanceByAccount(returnedTranForMaxId.AccountTo));
            Assert.IsTrue(maxIdAfterTransfer == maxIdBeforeTransfer + 1);
            Assert.IsTrue(transfer.TransferType == returnedTranForMaxId.TransferType);
            Assert.IsTrue(transfer.TransferStatus == returnedTranForMaxId.TransferStatus);
            Assert.IsTrue(transfer.UserNameFrom == returnedTranForMaxId.UserNameFrom);
            Assert.IsTrue(transfer.UserNameTo == returnedTranForMaxId.UserNameTo);
            Assert.IsTrue(transfer.Amount == returnedTranForMaxId.Amount);
            Assert.IsTrue(transfer.AccountFrom == returnedTranForMaxId.AccountFrom);
            Assert.IsTrue(transfer.AccountTo == returnedTranForMaxId.AccountTo);

        }

        [TestMethod]
        public void CreatePendingTransfer()
        {
            // Arrange
            Transfer transfer = new Transfer();
            transfer.TransferType = TransferType.Request;
            transfer.TransferStatus = TransferStatus.Pending;
            transfer.AccountFrom = 1;
            transfer.AccountTo = 2;
            transfer.Amount = 20;
            int maxIdBeforeTransfer = GetMaxTransferId();

            // Act
            bool actualResult = dao.CreatePendingTransfer(transfer);

            int maxIdAfterTran = GetMaxTransferId();
            Transfer returnedTranForMaxId = GetTransferByTransferId(maxIdAfterTran);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(1000, GetBalanceByAccount(returnedTranForMaxId.AccountFrom));
            Assert.AreEqual(1000, GetBalanceByAccount(returnedTranForMaxId.AccountTo));
            Assert.IsTrue(maxIdAfterTran == maxIdBeforeTransfer + 1);
            Assert.IsTrue(transfer.TransferType == returnedTranForMaxId.TransferType);
            Assert.IsTrue(transfer.TransferStatus == returnedTranForMaxId.TransferStatus);
            Assert.IsTrue(transfer.UserNameFrom == returnedTranForMaxId.UserNameFrom);
            Assert.IsTrue(transfer.UserNameTo == returnedTranForMaxId.UserNameTo);
            Assert.IsTrue(transfer.Amount == returnedTranForMaxId.Amount);
            Assert.IsTrue(transfer.AccountFrom == returnedTranForMaxId.AccountFrom);
            Assert.IsTrue(transfer.AccountTo == returnedTranForMaxId.AccountTo);

        }
        private int GetMaxTransferId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT MAX(transfer_id) FROM transfers", conn);
                    int maxId = Convert.ToInt32(cmd.ExecuteScalar());
                    return maxId;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }
        private Transfer GetTransferByTransferId(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    Transfer transfer = null;
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfers WHERE transfer_id = @transferId;", conn);
                    cmd.Parameters.AddWithValue("@transferId", id);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        transfer = RowToObject(rdr);
                    }

                    return transfer;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }
        private int GetTransferStatusByTransferId(int transferId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_status_id FROM transfers WHERE transfer_id = @transferId;", conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    int transferStatusId = Convert.ToInt32(cmd.ExecuteScalar());
                    return transferStatusId;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
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
        private int GetUserIdByUsername(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id FROM users WHERE username = @username;", conn);
                    cmd.Parameters.AddWithValue("@username", username);
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
