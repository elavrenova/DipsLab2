using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using StockService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockService.Controllers
{
    [Route("api")]
    public class StockController : Controller
    {
        private readonly StockContext dbcontext;
        public StockController(StockContext context)
        {
            this.dbcontext = context;
        }

        [HttpGet("")]
        public List<string> GetAllStocks([FromRoute] int page, int perpage)
        {
            var stocks = dbcontext.Stocks;
            return stocks.Select(n => $"Name: {n.Name}{Environment.NewLine}FreePlace: {n.FreePlace}{Environment.NewLine}")
                .ToList();
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

        [HttpPost("")]
        public IActionResult AddStock([FromBody] StockModel item)
        {
            var prevStock = dbcontext.Stocks.FirstOrDefault(n => n.Name == item.Name && n.FreePlace == item.FreePlace);
            if (prevStock == null)
            {
                dbcontext.Stocks.Add(new Stock(item));
                dbcontext.SaveChanges();
                return Ok();
            }
            return BadRequest();
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
