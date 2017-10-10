using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers
{
    [Route("api/[order]")]
    public class OrderController : Controller
    {
        private readonly OrderContext dbcontext;
        public OrderController(OrderContext context)
        {
            dbcontext = context;

            if (dbcontext.Orders.Count() == 0)
            {
                dbcontext.Orders.Add(new Order { UserId = 1, StockId = 1, TransferId = 1, Weight = 100.0, Status = 1 });
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
    }
}
