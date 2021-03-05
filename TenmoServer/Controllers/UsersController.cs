using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UsersController : ControllerBase
    {
        private IAccountDAO accountDAO;
        private ITransferDAO transferDAO;
        public UsersController(IAccountDAO accountDAO, ITransferDAO transferDAO)
        {
            this.accountDAO = accountDAO;
            this.transferDAO = transferDAO;
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

        [HttpGet("{id}/transfers")]
        public ActionResult<List<Transfer>> GetTransfersByUser(int id)
        {
            List<Transfer> transfers = transferDAO.GetTransfersByUser(id);

            return Ok(transfers);
        }

        [HttpGet]
        public ActionResult<List<DisplayAccount>> GetAllAccounts()
        {

            List<DisplayAccount> accounts = accountDAO.GetAllDisplayAccounts();

            return Ok(accounts);

        }

    }
}
