using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO: ITransferDAO
    {

        private string connectionString;

        public TransferSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
    
}
