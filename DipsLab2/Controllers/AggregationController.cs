using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using DipsLab2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DipsLab2.Controllers
{
    [Route("")]
    public class AggregationController : Controller
    {
        //private IOrderService orderService;
        private IStockService stockService;
        private ITransferService transferService;
        private ILogger<AggregationController> logger;

        public AggregationController(
            //IOrderService orderService,
            IStockService stockService,
            ITransferService transferService,
            ILogger<AggregationController> logger)
        {
            //this.orderService = orderService;
            this.stockService = stockService;
            this.transferService = transferService;
            this.logger = logger;
        }



        [HttpPost("order")]
        public async Task<IActionResult> AddOrder(StockTransferOrderModel item)
        {
            var stockResp =  await stockService.BookStock(item);
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus = 20;
                return BadRequest("Stock wasn't found");
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                item.OrderStatus = 30;
                return BadRequest("There isn't enough place");
            }
            item.OrderStatus = 10;
            var transfResp = await transferService.BookTransfer(item);
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                item.OrderStatus = item.OrderStatus + 2;
                return BadRequest("There are no transfers");
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                item.OrderStatus += 3;
                return BadRequest("All transfers are busy");
            }
            item.OrderStatus += 1;
            return Ok();
        }
    

        [HttpPut("refuse")]
        public async Task<IActionResult> RefuseOrder(StockTransferOrderModel item)
        {
            var stockResp = await stockService.RefuseStock(item);
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus = 20;
                return BadRequest("Stock wasn't found");
            }
            item.OrderStatus = 90;
            var transfResp = await transferService.RefuseTransfer(item);
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                item.OrderStatus = item.OrderStatus + 2;
                return BadRequest("No info for refusing transfer");
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus += 3;
                return BadRequest("Can't find transfer for refuse");
            }
            item.OrderStatus += 9;
            //var orderRes = orderService.UpdateOrder(item);
            return Ok();
        }

        [HttpGet("info")]
        public async Task<List<string>> GetInfo()
        {
            int page = 0;
            int size = 0;
            List<string> stockList = stockService.GetAllStocks(page, size);
            List<string> transferList = await transferService.GetAllTransfers(page, size);
            if (transferList.Count == 0 || stockList.Count == 0)
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
    }
}
