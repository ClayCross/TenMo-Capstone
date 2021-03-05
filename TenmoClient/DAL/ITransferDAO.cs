using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.DAL
{
    public interface ITransferDAO
    {
        bool CreateTransfer(Transfer transfer);
        List<Transfer> GetTransfersByUser(int id);

    }
}
