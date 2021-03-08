using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServerIntegrationTests
{
    [TestClass]
    class UserSqlDAOTests : SqlDAOTests
    {
        private const string connectionString = "Server=.\\SQLEXPRESS;Database=tenmo_test;Trusted_Connection=True;";
        private UserSqlDAO dao;

        [TestInitialize]
        public void ArrangeForEachTest()
        {
            this.dao = new UserSqlDAO(connectionString);

            SetupDB();
        }

        [TestMethod]
        public void GetUserTest()
        {
            // Arrange
            User UserWithMaxId = GetUserWithMaxUserId();

            // Act
            User actualResult = dao.GetUser("test5");

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(UserWithMaxId.UserId, actualResult.UserId);
            Assert.AreEqual(UserWithMaxId.Username, actualResult.Username);
            Assert.AreEqual(UserWithMaxId.PasswordHash, actualResult.PasswordHash);
        }

        [TestMethod]
        public void AddUserTest()
        {
            // Arrange
            string username = "test6";
            string password = "test6";
            User previousMaxUserId = GetUserWithMaxUserId();

            // Act
            User user = dao.AddUser("test6", "test6");
            User newUserMax = GetUserWithMaxUserId();
            // Assert
            Assert.IsNotNull(user);
            Assert.IsTrue(user.UserId == previousMaxUserId.UserId + 1);
            Assert.IsTrue(user.Username == newUserMax.Username);
            Assert.IsTrue(user.PasswordHash == newUserMax.PasswordHash);
            
        }

        private User GetUserWithMaxUserId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    User user = null;
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM users ORDER BY user_id DESC;", conn);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        user = RowToObject(rdr);
                    }

                    return user;
                    
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        private User RowToObject(SqlDataReader rdr)
        {
            User user = new User();

            user.UserId = Convert.ToInt32(rdr["user_id"]);
            user.Username = Convert.ToString(rdr["username"]);
            user.PasswordHash = Convert.ToString(rdr["password_hash"]);
            user.Salt = Convert.ToString(rdr["salt"]);

            return user;
        }
    }
}
