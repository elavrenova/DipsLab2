using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers
{
    [Route("")]
    public class OrderController : Controller
    {
        private readonly OrderContext dbcontext;
        private ILogger<OrderController> logger;
        public OrderController(OrderContext context)
        {
            this.logger = logger;
            this.dbcontext = context;
        }

        //[HttpGet("")]
        //public async Task<List<Order>> GetAllOrders(int page, int size)
        //{
        //    logger.LogDebug($"Getting list of orders on page={page} ");
        //    var orders = dbcontext.Orders.Where(s=>true);
        //    if (size != 0 && page != 0)
        //    {
        //        logger.LogDebug($"Looking for page {page} with orders ");
        //        orders = orders.Skip(size * page);
        //    }
        //    if (size != 0)
        //    {
        //        logger.LogDebug($"Getting first {size} orders");
        //        orders = orders.Take(size);
        //    }
        //    logger.LogDebug($"Returning {orders.Count()} orders");
        //    return orders.ToList();
        //}

        //[HttpGet("getorder/{id}")]
        //public IActionResult GetById(long id)
        //{
        //    logger.LogDebug($"Getting order by id");
        //    var item = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
        //    if (item == null)
        //    {
        //        logger.LogDebug($"Can't find order with id = {id}");
        //        return NotFound();
        //    }
        //    logger.LogDebug($"Returning order");
        //    return new ObjectResult(item);
        //}

        //[HttpPost("")]
        //public async Task<IActionResult> AddOrder(StockTransferOrderModel item)
        //{
        //    if (item == null)
        //    {
        //        logger.LogDebug($"Info for creating order is empty");
        //        return NoContent();
        //    }

        //    dbcontext.Orders.Add(new Order()
        //    {
        //        Status = item.OrderStatus,
        //        StockId = item.StockId,
        //        TransferId = item.TransferId,
        //        UserId = item.UserId,
        //        Value = item.Value
        //    });
        //    dbcontext.SaveChanges();
        //    return Ok();
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateOrder( StockTransferOrderModel item)
        //{
        //    if (item == null)
        //    {
        //        return NoContent();
        //    }

        //    var ord = dbcontext.Orders.FirstOrDefault(t => t.Id == item.Id);
        //    if (ord == null)
        //    {
        //        return NotFound();
        //    }

        //    ord.UserId = item.UserId;
        //    ord.StockId = item.StockId;
        //    ord.Value = item.Value;
        //    ord.Status = item.TransferStatus;

        //    dbcontext.Orders.Update(ord);
        //    dbcontext.SaveChanges();
        //    return new NoContentResult();
        //}

        //[HttpDelete("{id}")]
        //public IActionResult Delete(long id)
        //{
        //    var ord = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
        //    if (ord == null)
        //    {
        //        return NotFound();
        //    }

        //    dbcontext.Orders.Remove(ord);
        //    dbcontext.SaveChanges();
        //    return new NoContentResult();
        //}
    }
}
