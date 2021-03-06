using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServerIntegrationTests
{
    [TestClass]
    public class AccountSqlDAOTests : SqlDAOTests
    {
        private const string connectionString = "Server=.\\SQLEXPRESS;Database=tenmo_test;Trusted_Connection=True;";
        private AccountSqlDAO dao;

        [TestInitialize]
        public void ArrangeForEachTest()
        {
            this.dao = new AccountSqlDAO(connectionString);
            
            SetupDB();
        }

        [TestMethod]
        public void GetAccountByUserIdTest()
        {
            // Arrange
            int test1UserId = GetTest1UserId();
            Account expectedAccount = GetTest1Account(test1UserId);

            // Act
            Account actualAccount = dao.GetAccountByUserId(test1UserId);

            // Assert
            Assert.IsNotNull(actualAccount);
            Assert.AreEqual(expectedAccount.AccountId, actualAccount.AccountId);
            Assert.AreEqual(expectedAccount.Balance, actualAccount.Balance);
        }

        [TestMethod]
        public void GetAllDisplayAccountTest()
        {
            // Arrange

            // Act
            List<DisplayAccount> actualResult = dao.GetAllDisplayAccounts();

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(5, actualResult.Count);
            Assert.IsTrue(CheckIfListContainsUsername("test1", actualResult));
            Assert.IsTrue(CheckIfListContainsUsername("test3", actualResult));
            Assert.IsTrue(CheckIfListContainsUsername("test5", actualResult));
        }

        private bool CheckIfListContainsUsername(string username, List<DisplayAccount> accounts)
        {

            foreach (DisplayAccount account in accounts)
            {
                if (account.Username == username)
                {
                    return true;
                }
            }
            return false;
        }

        private Account GetTest1Account(int test1UserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Select * FROM accounts WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", test1UserId);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    Account account = null;
                    if (rdr.Read())
                    {
                        account = new Account();
                        account.AccountId = Convert.ToInt32(rdr["account_id"]);
                        account.UserId = Convert.ToInt32(rdr["user_id"]);
                        account.Balance = Convert.ToDecimal(rdr["balance"]);
                    }
                    return account;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
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
