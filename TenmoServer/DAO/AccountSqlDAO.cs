using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private string connectionString;
        private const string SQL_GET_ACCOUNT_BALANCE = "SELECT * FROM accounts WHERE user_id = @userId;";
        private const string SQL_GET_ALL_DISPLAY_ACCOUNTS = @"SELECT u.username, a.account_id FROM accounts a JOIN users u ON u.user_id = a.user_id";
        public AccountSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Account GetAccountByUserId(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    Account account = null;

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_ACCOUNT_BALANCE, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        account = RowToObject(rdr);
                    }

                    return account;
                }
            }
            catch (SqlException)
            {

                throw;
            }
        }

        public List<DisplayAccount> GetAllDisplayAccounts()
        {
            List<DisplayAccount> accounts = new List<DisplayAccount>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_ALL_DISPLAY_ACCOUNTS, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        DisplayAccount account = new DisplayAccount();
                        account.Username = Convert.ToString(rdr["username"]);
                        account.AccountId = Convert.ToInt32(rdr["account_id"]);
                        accounts.Add(account);
                    }

                    return accounts;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }


        private Account RowToObject(SqlDataReader rdr)
        {
            Account account = new Account();

            account.AccountId = Convert.ToInt32(rdr["account_id"]);
            account.UserId = Convert.ToInt32(rdr["user_id"]);
            account.Balance = Convert.ToDecimal(rdr["balance"]);

            return account;
        }
    }
}
