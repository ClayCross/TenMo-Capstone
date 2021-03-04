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
