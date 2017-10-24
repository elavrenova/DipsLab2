using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using TransferService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TransferService.Controllers
{
    [Route("api")]
    public class TransferController : Controller
    {
        private readonly TransferContext dbcontext;

        public TransferController(TransferContext context)
        {
            this.dbcontext = context;
        }

        [HttpGet("")]
        public List<string> GetAllTransfers([FromRoute] int page, int perpage)
        {
            var transfers = dbcontext.Transfers;
            return transfers.Select(n => $"Name: {n.Name}{Environment.NewLine}Carrying: {n.Carrying}{Environment.NewLine}Status: {n.Status}")
                .ToList();
        }

        [HttpGet("{id}", Name = "GetTransfer")]
        public IActionResult GetById(long id)
        {
            var item = dbcontext.Transfers.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost("")]
        public IActionResult AddTransfer([FromBody] TransferModel item)
        {
            var prevTransf = dbcontext.Transfers.FirstOrDefault(n => n.Name == item.Name);
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


        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Transfer item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var trans = dbcontext.Transfers.FirstOrDefault(t => t.Id == id);
            if (trans == null)
            {
                return NotFound();
            }

            trans.Name = item.Name;
            trans.Carrying = item.Carrying;
            trans.Status = item.Status;

            dbcontext.Transfers.Update(trans);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var trans = dbcontext.Transfers.FirstOrDefault(t => t.Id == id);
            if (trans == null)
            {
                return NotFound();
            }

            dbcontext.Transfers.Remove(trans);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }
    }
}
