using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using StockService.Models;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockService.Controllers
{
    [Route("")]
    public class StockController : Controller
    {
        private readonly StockContext dbcontext;
        private ILogger<StockController> logger;
        public StockController(StockContext context, ILogger<StockController> logger)
        {
            this.logger = logger;
            this.dbcontext = context;
        }

        [HttpGet("")]
        public List<string> GetAllStocks([FromRoute] int page, int size)
        {
            logger.LogDebug($"Getting list of stocks on page={page} ");
            var stocks = dbcontext.Stocks.AsEnumerable<Stock>();
            if (size != 0 && page != 0)
            {
                logger.LogDebug($"Looking for page {page} ");
                stocks = stocks.Skip(size * page);
            }
            if (size != 0)
            {
                logger.LogDebug($"Getting {size} stocks");
                stocks = stocks.Take(size);
            }
            logger.LogDebug($"Returning {stocks.Count()} stocks");
            return stocks.Select(n => $"Name: {n.Name}{Environment.NewLine}FreePlace: {n.FreePlace}{Environment.NewLine}")
                .ToList();
        }

        [HttpGet("getstock/{id}")]
        public StockModel GetById(long id)
        {
            var item = dbcontext.Stocks.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return null;
            }
            StockModel sm = new StockModel();
            sm.Name = item.Name;
            sm.FreePlace = item.FreePlace;
            return sm;
        }

        [HttpPost("")]
        public IActionResult AddStock(StockModel item)
        {
            logger.LogDebug($"Looking for stock with the same info");
            var prevStock = dbcontext.Stocks.FirstOrDefault(n => n.Name == item.Name && n.FreePlace == item.FreePlace);
            if (prevStock == null)
            {
                logger.LogDebug($"Adding new stock");
                dbcontext.Stocks.Add(new Stock(item));
                logger.LogDebug($"Saving changes in db");
                dbcontext.SaveChanges();
                return Ok();
            }
            logger.LogDebug($"There is a stock with the same info");
            return BadRequest();
        }

        [HttpPut("upd/{id}")]
        public IActionResult Update(long id, Stock item)
        {
            
            if (item == null || item.Id != id)
            {
                logger.LogDebug($"");
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

        [HttpPut("book_s/{id}")]
        public async Task<IActionResult> BookStock(StockTransferOrderModel item)
        {
            var stck = dbcontext.Stocks.FirstOrDefault(t => t.Id == item.StockId);
            if (stck == null)
            {
                return NotFound();
            }
            if (stck.FreePlace < item.Value)
            {
                return BadRequest("not enough place");
            }
            stck.FreePlace = stck.FreePlace - item.Value;
            dbcontext.Stocks.Update(stck);
            dbcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("refuse_s/{id}")]
        public async Task<IActionResult> RefuseStock(StockTransferOrderModel item)
        {
            var stck = dbcontext.Stocks.FirstOrDefault(t => t.Id == item.StockId);
            if (stck == null)
            {
                return NotFound();
            }
            stck.FreePlace = stck.FreePlace + item.Value;
            dbcontext.Stocks.Update(stck);
            dbcontext.SaveChanges();
            return Ok();
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
