using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IAccountDAO accountDAO;
        public UsersController(IAccountDAO accountDAO)
        {
            this.accountDAO = accountDAO;
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById(int id)
        {
            Account account = accountDAO.GetAccountByUserId(id);

            if (account == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(account);
            }
        }

        [HttpGet]
        public ActionResult<List<DisplayAccount>> GetAllAccounts()
        {

            List<DisplayAccount> accounts = accountDAO.GetAllDisplayAccounts();

            return Ok(accounts);

        }

    }
}
