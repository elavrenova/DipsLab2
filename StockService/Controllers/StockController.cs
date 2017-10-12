using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockService.Controllers
{
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private readonly StockContext dbcontext;
        public StockController(StockContext context)
        {
            dbcontext = context;

            if (dbcontext.Stocks.Count() == 0)
            {
                dbcontext.Stocks.Add(new Stock { Name = "Stock1", FreePlace = 10000000.0});
                dbcontext.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<Stock> GetAll()
        {
            return dbcontext.Stocks.ToList();
        }

        [HttpGet("{id}", Name = "GetStock")]
        public IActionResult GetById(long id)
        {
            var item = dbcontext.Stocks.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Stock item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            dbcontext.Stocks.Add(item);
            dbcontext.SaveChanges();

            return CreatedAtRoute("GetStock", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Stock item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var stck = dbcontext.Stocks.FirstOrDefault(t => t.Id == id);
            if (stck == null)
            {
                return NotFound();
            }

            stck.Name = item.Name;
            stck.FreePlace = item.FreePlace;

            dbcontext.Stocks.Update(stck);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var stck = dbcontext.Stocks.FirstOrDefault(t => t.Id == id);
            if (stck == null)
            {
                return NotFound();
            }

            dbcontext.Stocks.Remove(stck);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }
    }
}
