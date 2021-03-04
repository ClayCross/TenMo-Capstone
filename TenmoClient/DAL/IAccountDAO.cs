using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.DAL
{
    public interface IAccountDAO
    {
        Account GetAccountByUserId(int userId);
    }
}
