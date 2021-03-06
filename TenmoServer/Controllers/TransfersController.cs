using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using static TenmoServer.Models.Enums;

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

        [HttpPost("request")]
        public IActionResult RequestTransfer(Transfer transfer)
        {
            bool wasCreated = transferDAO.CreatePendingTransfer(transfer);

            if (wasCreated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTransfer(Transfer transfer)
        {
            bool wasUpdated = false;

            if (transfer.TransferStatus == TransferStatus.Approved)
            {
                wasUpdated = transferDAO.AuthorizeTransfer(transfer);
            }
            else
            {
                wasUpdated = transferDAO.RejectTransfer(transfer);
            }

            if (wasUpdated)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }


    }


}
