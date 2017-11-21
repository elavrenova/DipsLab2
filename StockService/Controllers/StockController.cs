using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockService.Models;
using Microsoft.Extensions.Logging;
using Gateway.Models;
using Gateway.Pagination;
using Newtonsoft.Json;

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

        [HttpGet("get_all_stocks")]
        public async Task<ListForPagination<string>> GetAllStocks(int size, int page)
        {
            int lastPage = 0;
            logger.LogDebug($"Getting list of stocks on page={page} ");
            var stocks = dbcontext.Stocks.Where(s => true);
            if (size != 0 && page != 0)
            { 
                logger.LogDebug($"Looking for page {page} ");
                stocks = stocks.Skip(size * page);
            }
            if (size != 0)
            {
                logger.LogDebug($"Getting {size} stocks");
                lastPage = stocks.Count() / size + (stocks.Count() % size == 0 ? -1 : 0);
                stocks = stocks.Take(size);
            }
            logger.LogDebug($"Returning {stocks.Count()} stocks");
            return new ListForPagination<string>(stocks.Select(stock => $"Id: {stock.Id}{Environment.NewLine}Name: {stock.Name}{Environment.NewLine}Free place: {stock.FreePlace}")
                .ToList(),size,page,lastPage);
        }

        [HttpGet("get_stocks")]
        public async Task<string> GetStocks()
        {
            var stocks = dbcontext.Stocks.Where(s => true).ToList();
            int st_q = stocks.Count();
            var str = JsonConvert.SerializeObject(stocks);
            return str;
        }

        //[HttpPost("addstock")]
        //public IActionResult AddStock(StockModel item)
        //{
        //    logger.LogDebug($"Looking for stock with the same info");
        //    var prevStock = dbcontext.Stocks.FirstOrDefault(n => n.Name == item.Name && n.FreePlace == item.FreePlace);
        //    if (prevStock == null)
        //    {
        //        logger.LogDebug($"Adding new stock");
        //        dbcontext.Stocks.Add(new Stock(item));
        //        logger.LogDebug($"Saving changes in db");
        //        dbcontext.SaveChanges();
        //        return Ok();
        //    }
        //    logger.LogDebug($"There is a stock with the same info");
        //    return BadRequest();
        //}

        [HttpPut("book_stock")]
        public IActionResult BookStock([FromBody]StockTransferOrderModel item)
        {
            logger.LogDebug($"Getting stock with id = {item.StockId}");
            var stck = dbcontext.Stocks.FirstOrDefault(t => t.Id == item.StockId);
            if (stck == null)
            {
                logger.LogDebug($"Can't find stock with id = {item.StockId}");
                return NotFound();
            }
            if (stck.FreePlace < item.Value)
            {
                logger.LogDebug($"The stock {item.StockId} doesn't have enough place for order");
                return BadRequest("not enough place");
            }
            logger.LogDebug($"Booking place in the stock id = {item.StockId}");
            stck.FreePlace = stck.FreePlace - item.Value;
            logger.LogDebug($"Updating database");
            var res = dbcontext.Stocks.Update(stck);
            logger.LogDebug($"Saving changes");
            dbcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("refuse_stock")]
        public IActionResult RefuseStock([FromBody]StockTransferOrderModel item)
        {
            logger.LogDebug($"Getting stock with id = {item.StockId}");
            var stck = dbcontext.Stocks.FirstOrDefault(t => t.Id == item.StockId);
            if (stck == null)
            {
                logger.LogDebug($"Can't find stock with id = {item.StockId}");
                return NotFound();
            }
            logger.LogDebug($"Refusing place in the stock id = {item.StockId}");
            stck.FreePlace = stck.FreePlace + item.Value;
            logger.LogDebug($"Updating database");
            dbcontext.Stocks.Update(stck);
            logger.LogDebug($"Saving changes");
            dbcontext.SaveChanges();
            return Ok();
        }
    }
}
