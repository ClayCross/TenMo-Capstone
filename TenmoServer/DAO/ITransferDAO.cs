using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        bool CreateTransfer(Transfer transfer);
        List<Transfer> GetTransfersByUser(int id);
        bool CreatePendingTransfer(Transfer transfer);
    }
}
