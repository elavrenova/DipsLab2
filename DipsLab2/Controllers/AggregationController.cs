using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using DipsLab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace DipsLab2.Controllers
{
    [Route("")]
    public class AggregationController : Controller
    {
        private IOrderService orderService;
        private IStockService stockService;
        private ITransferService transferService;

        public AggregationController(
            IOrderService orderService,
            IStockService stockService,
            ITransferService transferService)
        {
            this.orderService = orderService;
            this.stockService = stockService;
            this.transferService = transferService;
        }



        [HttpPost("order")]
        public async Task<IActionResult> AddOrder(StockTransferOrderModel item)
        {
            var stockResp = stockService.BookStock();
            if (stockResp == null)
            {
                orderModel.Status = 20;
                return BadRequest("Stock wasn't found");
            }


        }

        [HttpGet("info")]
        public async Task<List<string>> GetInfo()
        {
            int page = 1;
            int size = 5;
            List<string> stockList = await stockService.GetAllStocks(page, size);
            List<string> transferList = await transferService.GetAllTransfers(page, size);
            if (transferList == null || stockList == null)
            {
                return null;
            }
            else 
            {
                stockList.Add("*");
                foreach (var t in transferList)
                {
                    stockList.Add(t);
                }
                return stockList;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
