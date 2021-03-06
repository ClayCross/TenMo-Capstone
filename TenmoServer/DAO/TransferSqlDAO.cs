using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using static TenmoServer.Models.Enums;

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
        private const string SQL_AUTHORIZE_TRANSFER = @"BEGIN TRANSACTION
                                                    UPDATE accounts SET balance = @fromBalance WHERE account_id = @accountFrom;
                                                    UPDATE accounts SET balance = @toBalance WHERE account_id = @accountTo;
                                                    UPDATE transfers SET transfer_status_id = 2 WHERE transfer_id = @transferId;
                                                    COMMIT TRANSACTION;";
        private const string SQL_REJECT_TRANSFER = @"UPDATE transfers SET transfer_status_id = 3 WHERE transfer_id = @transferId;";
        private const string SQL_CREATE_PENDING_TRANSFER = @"INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES
	                                                            (@transferTypeId, @transferStatusId, @accountFrom, @accountTo, @amount);";

        private const string SQL_GET_TRANSFERS_BY_USER = @"SELECT t.*, uFrom.username AS FromUsername, uTo.username AS ToUsername
	                                                         FROM transfers t
	                                                         JOIN accounts aFrom ON t.account_from = aFrom.account_id
	                                                         JOIN accounts aTo ON t.account_to = aTo.account_id
	                                                         JOIN users uFrom ON aFrom.user_id = uFrom.user_id
	                                                         JOIN users uTo ON aTo.user_id = uTo.user_id
                                                             WHERE aFrom.user_id = @userId OR aTo.user_id = @userId;";
        private const string SQL_GET_PENDING_BY_USER = @"SELECT t.*, uFrom.username AS FromUsername, uTo.username AS ToUsername
	                                                         FROM transfers t
	                                                         JOIN accounts aFrom ON t.account_from = aFrom.account_id
	                                                         JOIN accounts aTo ON t.account_to = aTo.account_id
	                                                         JOIN users uFrom ON aFrom.user_id = uFrom.user_id
	                                                         JOIN users uTo ON aTo.user_id = uTo.user_id
                                                             WHERE aFrom.user_id = @userId AND t.transfer_status_id = 1";

        public TransferSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Transfer> GetTransfersByUser(int id)
        {
            List<Transfer> transfers = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_TRANSFERS_BY_USER, conn);
                    cmd.Parameters.AddWithValue("@userId", id);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Transfer transfer = RowToObject(rdr);
                        transfers.Add(transfer);
                    }

                    return transfers;
                }
            }
            catch (SqlException ex)
            {

                throw;
            }

        }

        public List<Transfer> GetPendingByUser(int id)
        {
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_GET_PENDING_BY_USER, conn);
                    cmd.Parameters.AddWithValue("@userId", id);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while(rdr.Read())
                    {
                        Transfer transfer = RowToObject(rdr);
                        transfers.Add(transfer);
                    }

                    return transfers;
                }

            }
            catch (SqlException ex)
            {

                throw;
            }
        }

        public bool AuthorizeTransfer(Transfer transfer)
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

                    cmd = new SqlCommand(SQL_AUTHORIZE_TRANSFER, conn);
                    
                    cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);
                    cmd.Parameters.AddWithValue("@fromBalance", fromBalance);
                    cmd.Parameters.AddWithValue("@accountFrom", transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@toBalance", toBalance);
                    cmd.Parameters.AddWithValue("@accountTo", transfer.AccountTo);

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

        public bool RejectTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_REJECT_TRANSFER, conn);
                    cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
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

        public bool CreatePendingTransfer(Transfer transfer)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SQL_CREATE_PENDING_TRANSFER, conn);
                    cmd.Parameters.AddWithValue("@transferTypeId", (int)transfer.TransferType);
                    cmd.Parameters.AddWithValue("@transferStatusId", (int)transfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@accountFrom",transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@accountTo", transfer.AccountTo);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if(rowsAffected == 1)
                    {
                        return true;
                    }
                    return false;
                }
                
                        
            }
            catch (Exception)
            {

                throw;
            }
        }
    



    private Transfer RowToObject(SqlDataReader rdr)
        {
            Transfer transfer = new Transfer();

            transfer.TransferId = Convert.ToInt32(rdr["transfer_id"]);
            transfer.TransferType = (TransferType)Convert.ToInt32(rdr["transfer_type_id"]);
            transfer.TransferStatus = (TransferStatus)Convert.ToInt32(rdr["transfer_status_id"]);
            transfer.AccountFrom = Convert.ToInt32(rdr["account_from"]);
            transfer.UserNameFrom = Convert.ToString(rdr["FromUsername"]);
            transfer.AccountTo = Convert.ToInt32(rdr["account_to"]);
            transfer.UserNameTo = Convert.ToString(rdr["ToUsername"]);
            transfer.Amount = Convert.ToDecimal(rdr["amount"]);

            return transfer;
        }


    }

}
