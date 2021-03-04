using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Views
{
    public enum TransferType
    {
        Request =1,
        Send
    }
    public enum TransferStatus
    {
       Pending = 1,
        Approved,
        Rejected

    }
    

}
