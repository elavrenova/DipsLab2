using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockService.Models;
using Microsoft.Extensions.Logging;
using Gateway.Models;

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
        public async Task<IActionResult> GetAllStocks(int page, int size)
        {
            logger.LogDebug($"Getting list of stocks on page={page} ");
            var stocks = dbcontext.Stocks.Where(s => true);
            //var stocks = dbcontext.Stocks.AsEnumerable();
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
            return StatusCode(200, await stocks.Select(stock => $"{stock.Name}: free place = {stock.FreePlace}").ToListAsync());
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

        [HttpPut("books")]
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

        [HttpPut("refuses")]
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
