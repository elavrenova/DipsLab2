using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly OrderContext dbcontext;
        public OrderController(OrderContext context)
        {
            dbcontext = context;

            if (dbcontext.Orders.Count() == 0)
            {
                dbcontext.Orders.Add(new Order { UserId = 1, StockId = 1, TransferId = 1, Weight = 100.0, Status = 1 });
                dbcontext.Orders.Add(new Order { UserId = 1, StockId = 2, TransferId = 1, Weight = 1000.0, Status = 0 });
                dbcontext.Orders.Add(new Order { UserId = 1, StockId = 1, TransferId = 2, Weight = 500.0, Status = 2 });
                dbcontext.SaveChanges();
            }
        }

        [HttpGet]
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
        public IActionResult Create([FromBody] Order item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            dbcontext.Orders.Add(item);
            dbcontext.SaveChanges();

            return CreatedAtRoute("GetOrder", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Order item)
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
            ord.Weight = item.Weight;
            ord.TransferId = item.TransferId;
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
