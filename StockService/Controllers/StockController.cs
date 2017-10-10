using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockService.Controllers
{
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private readonly StockContext dbcontext;
        public StockController(StockContext context)
        {
            dbcontext = context;

            if (dbcontext.Stocks.Count() == 0)
            {
                dbcontext.Stocks.Add(new Stock { Name = "Stock1", FreePlace = 10000000.0});
                dbcontext.SaveChanges();
            }
        }
    }
}
