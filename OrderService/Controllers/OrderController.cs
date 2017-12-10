using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        [HttpGet("")]
        public async Task<IActionResult> GetAllOrders(int page, int size)
        {
            var orders = dbcontext.Orders.Where(s => true);
            if (size != 0 && page != 0)
            {
                orders = orders.Skip(size * page);
            }
            if (size != 0)
            {
                
                orders = orders.Take(size);
            }
            return StatusCode(200,orders.Select(ord => $"stock:{ord.StockId} transfer:{ord.TransferId} value:{ord.Value} userId:{ord.UserId} status:{ord.Status} orderId:{ord.Id}").ToList());
        }

        [HttpGet("get_orders")]
        public async Task<string> GetOrders()
        {
            var orders = dbcontext.Orders.Where(s => true).ToList();
            var str = JsonConvert.SerializeObject(orders);
            return str;
        }

        [HttpGet("getorder/{id}")]
        public IActionResult GetById(int id)
        {
            logger.LogDebug($"Getting order by id");
            var item = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                logger.LogDebug($"Can't find order with id = {id}");
                return NotFound();
            }
            logger.LogDebug($"Returning order");
            return new ObjectResult(item);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddOrder([FromBody]StockTransferOrderModel item)
        {
            if (item == null)
            {
                logger.LogDebug($"Info for creating order is empty");
                return StatusCode(204,"the item for adding is empty");
            }
            dbcontext.Orders.Add(new Order()
            {
                Status = item.Status,
                StockId = item.StockId,
                TransferId = item.TransferId,
                UserId = item.UserId,
                Value = item.Value
            });
            dbcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("")]
        public async Task<IActionResult> RefuseOrder([FromBody]StockTransferOrderModel item)
        {
            if (item == null)
            {
                return NoContent();
            }
            var ord = dbcontext.Orders.FirstOrDefault(t => t.Id == item.Id);
            if (ord == null)
            {
                return NotFound();
            }
            ord.Status = item.Status;
            dbcontext.Orders.Update(ord);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder(StockTransferOrderModel item)
        {
            if (item == null)
            {
                return NoContent();
            }

            var ord = dbcontext.Orders.FirstOrDefault(t => t.Id == item.Id);
            if (ord == null)
            {
                return NotFound();
            }

            ord.UserId = item.UserId;
            ord.StockId = item.StockId;
            ord.Value = item.Value;
            ord.Status = item.Status;

            dbcontext.Orders.Update(ord);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }

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
