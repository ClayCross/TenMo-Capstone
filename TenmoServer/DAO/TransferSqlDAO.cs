using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO: ITransferDAO
    {

        private string connectionString;
        private const string SQL_CREATE_TRANSFER = @"BEGIN TRANSACTION
    UPDATE accounts SET balance = @fromBalance WHERE account_id = @accountFrom;
    UPDATE accounts SET balance = @toBalance WHERE account_id = @accountTo;
    INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES
	    (@transferTypeId, @transferStatusId, @accountFrom, @accountTo, @amount);
    COMMIT TRANSACTION";

        public TransferSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool CreateTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE account_id = @fromAccountId", conn);
                    cmd.Parameters.AddWithValue("@fromAccountId", transfer.AccountFrom);
                    decimal fromBalance = Convert.ToDecimal(cmd.ExecuteScalar());
                    fromBalance = fromBalance - transfer.Amount;

                    cmd = new SqlCommand("SELECT balance FROM accounts WHERE account_id = @toAccountId", conn);
                    cmd.Parameters.AddWithValue("@toAccountId", transfer.AccountTo);
                    decimal toBalance = Convert.ToDecimal(cmd.ExecuteScalar());
                    toBalance = toBalance + transfer.Amount;

                    cmd = new SqlCommand(SQL_CREATE_TRANSFER, conn);
                    cmd.Parameters.AddWithValue("@fromBalance", fromBalance);
                    cmd.Parameters.AddWithValue("@accountFrom", transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@toBalance", toBalance);
                    cmd.Parameters.AddWithValue("@accountTo", transfer.AccountTo);
                    cmd.Parameters.AddWithValue("@transferTypeId", (int)transfer.TransferType);
                    cmd.Parameters.AddWithValue("@transferStatusId", (int)transfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 3)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }

    }

}
