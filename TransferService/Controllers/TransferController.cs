using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using TransferService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TransferService.Controllers
{
    [Route("")]
    public class TransferController : Controller
    {
        private readonly TransferContext dbcontext;
        private ILogger<TransferController> logger;

        public TransferController(TransferContext context, ILogger<TransferController> logger)
        {
            this.dbcontext = context;
            this.logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllTransfers(int page, int size)
        {
            var transfers = dbcontext.Transfers.Where(s=> true);
            //var transfers = dbcontext.Transfers.AsEnumerable();
            if (size != 0 && page != 0)
            {
                transfers = transfers.Skip(size * page);
            }
            if (size != 0)
            {
                transfers = transfers.Take(size);
            }
            return StatusCode(200,transfers.Select(transf => $"{transf.Name}: carrying = {transf.Carrying} , status: {transf.Status}").ToList());
        }

        [HttpPost("")]
        public IActionResult AddTransfer(TransferModel item)
        {
            var prevTransf = dbcontext.Transfers.FirstOrDefault(n => n.Name == item.Name && n.Carrying == item.Carrying);
            if (prevTransf == null)
            {
                dbcontext.Transfers.Add(new Transfer(item)
                {
                    Name = item.Name,
                    Carrying = item.Carrying,
                    Status = item.Status
                });
                dbcontext.SaveChanges();
                return Ok();
            }
            return BadRequest(); 
        }

        [HttpPut("find_transfer")]
        public IActionResult FindTransfer(StockTransferOrderModel item)
        {
            var trans = dbcontext.Transfers;
            if (trans.Count() == 0)
            {
                return NoContent();
            }
            var TransId = 0;
            foreach (var t in trans)
            {
                if (TransId > 0)
                    break;
                if (t.Carrying >= item.Value && t.Status == 0)
                {
                    TransId = t.Id;
                }
            }

            if (TransId == 0)
            {
                return BadRequest("All transfers are busy");
            }
            var transfer = dbcontext.Transfers.FirstOrDefault(t => t.Id == TransId);
            transfer.Status = 1;
            item.TransferId = TransId;
            dbcontext.Transfers.Update(transfer);
            dbcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("book_transfer")]
        public IActionResult BookTransfer(StockTransferOrderModel item)
        {
            var trans = dbcontext.Transfers;
            var transfer = trans.FirstOrDefault(t => t.Id == item.TransferId);
            if (transfer == null)
            {
                return StatusCode(404, "There is no transfer with such id");
            }
            transfer.Status = 1;
            dbcontext.Transfers.Update(transfer);
            dbcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("refuse_transfer")]
        public IActionResult RefuseTransfer([FromBody]StockTransferOrderModel item)
        {
            if (item.TransferId == 0)
            {
                return NoContent();
            }

            var trans = dbcontext.Transfers.FirstOrDefault(t => t.Id == item.TransferId);
            if (trans == null)
            {
                return StatusCode(404,"Transfer with such id wasn't found");
            }

            trans.Status = 0;

            dbcontext.Transfers.Update(trans);
            dbcontext.SaveChanges();
            return Ok();
        }
    }
}
