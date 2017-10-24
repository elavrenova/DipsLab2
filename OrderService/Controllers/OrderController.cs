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
    [Route("api")]
    public class OrderController : Controller
    {
        private readonly OrderContext dbcontext;
        public OrderController(OrderContext context)
        {
            this.dbcontext = context;
        }

        [HttpGet("")]
        public IEnumerable<Order> GetAll()
        {
            return dbcontext.Orders.ToList();
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult GetById(long id)
        {
            var item = dbcontext.Orders.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult AddOrder([FromBody] OrderModel item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            dbcontext.Orders.Add(new Order(item));
            dbcontext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] OrderModel item)
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
