using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers
{
    [Route("")]
    public class OrderController : Controller
    {
        private readonly OrderContext dbcontext;
        public OrderController(OrderContext context)
        {
            this.dbcontext = context;
        }

        [HttpGet("")]
        public async Task<List<string>> GetAllOrders([FromRoute]int page, int size)
        {
            var orders = dbcontext.Orders.AsEnumerable<Order>();
            if (size != 0 && page != 0)
            {
                orders = orders.Skip(size * page);
            }
            if (size != 0)
            {
                orders = orders.Take(size);
            }
            return orders.Select(n => $"UserId: {n.UserId}{Environment.NewLine}StockId: {n.StockId}{Environment.NewLine}Status: {n.Status}{Environment.NewLine}Value: {n.Value}{Environment.NewLine}TransferId: {n.TransferId}")
                .ToList();
        }

        [HttpGet("getorder/{id}")]
        public IActionResult GetById(long id)
        {
            var item = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddOrder(StockTransferOrderModel item)
        {
            if (item == null)
            {
                return NoContent();
            }

            dbcontext.Orders.Add(new Order()
            {
                Status = item.OrderStatus,
                StockId = item.StockId,
                TransferId = item.TransferId,
                UserId = item.UserId,
                Value = item.Value
            });
            dbcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(long id, OrderModel item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var ord = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
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

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var ord = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
            if (ord == null)
            {
                return NotFound();
            }

            dbcontext.Orders.Remove(ord);
            dbcontext.SaveChanges();
            return new NoContentResult();
        }
    }
}
