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
    [Route("api/[controller]")]
    [ApiController]
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

        }
    }


}
