using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
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
