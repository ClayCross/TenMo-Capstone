using System;
using System.Collections.Generic;
using System.Text;
using static TenmoClient.Data.Enums;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int TransferId { get; set; }

        public TransferType TransferType { get; set; }

        public TransferStatus TransferStatus { get; set; }

        public int AccountFrom { get; set; }

        public int AccountTo { get; set; }

        public string UserNameFrom { get; set; }

        public string UserNameTo { get; set; }

        public decimal Amount { get; set; }
    }
}
