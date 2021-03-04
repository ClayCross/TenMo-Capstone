using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Enums
    {
        public enum TransferType
        {
            Request = 1,
            Send
        }
        public enum TransferStatus
        {
            Pending = 1,
            Approved,
            Rejected

        }
    }
}
