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
    public class TransfersController : ControllerBase
    {
        ITransferDAO transferDAO;
        public TransfersController(ITransferDAO transferDAO)
        {
            this.transferDAO = transferDAO;
        }

       [HttpPost]
       public IActionResult CreateTransfer(Transfer transfer)
        {
            bool wasCreated = transferDAO.CreateTransfer(transfer);

            if (wasCreated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


    }


}
