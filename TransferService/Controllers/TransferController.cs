using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransferService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TransferService.Controllers
{
    [Route("api/[controller]")]
    public class TransferController : Controller
    {
        private readonly TransferContext dbcontext;
        public TransferController(TransferContext context)
        {
            dbcontext = context;

            if (dbcontext.Transfers.Count() == 0)
            {
                dbcontext.Transfers.Add(new Transfer { Name = "Volvo121", Carrying = 10000.0, Status = 1 });
                dbcontext.Transfers.Add(new Transfer { Name = "Man333", Carrying = 3500.0, Status = 0 });
                dbcontext.Transfers.Add(new Transfer { Name = "Kamaz987", Carrying = 5000.0, Status = 2 });
                dbcontext.SaveChanges();
            }
        }
        [HttpGet]
        public IEnumerable<Transfer> GetAll()
        {
            return dbcontext.Transfers.ToList();
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

        [HttpPost]
        public IActionResult Create([FromBody] Transfer item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            dbcontext.Transfers.Add(item);
            dbcontext.SaveChanges();

            return CreatedAtRoute("GetTransfer", new { id = item.Id }, item);
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
