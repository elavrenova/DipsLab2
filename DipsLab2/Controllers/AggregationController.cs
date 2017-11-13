using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using DipsLab2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

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
        public async Task<IActionResult> AddOrder(int stockId, double value)
        {
            var item = new StockTransferOrderModel();
            item.StockId = stockId;
            item.Value = value;

            var stockResp = await stockService.BookStock(item);
            if (stockResp == null)
            {
                return StatusCode(503, "StockService is unavailable");
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus = 20;
                return StatusCode(500, "Stock wasn't found");
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                item.OrderStatus = 30;
                return StatusCode(507, "There isn't enough place");
            }
            item.OrderStatus = 10;
            var transfResp = await transferService.BookTransfer(item);
            if (transfResp == null)
            {
                stockResp = await stockService.RefuseStock(item);
                return StatusCode(503, "TransferService is unavailable");
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                item.OrderStatus = item.OrderStatus + 2;
                stockResp = await stockService.RefuseStock(item);
                return StatusCode(500, "There are no transfers");
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                item.OrderStatus += 3;
                stockResp = await stockService.RefuseStock(item);
                return StatusCode(500, "All transfers are busy");
            }
            item.OrderStatus += 1;
            return Ok();
        }


        [HttpPut("refuse")]
        public async Task<IActionResult> RefuseOrder(int stockId, double value, int transferId)
        {
            var item = new StockTransferOrderModel();
            item.StockId = stockId;
            item.Value = value;
            item.TransferId = transferId;

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
        public async Task<IActionResult> GetInfo(int? page, int? size)
        {
            if (page == null || size == null)
            {
                return StatusCode(500);
            }
            var message = string.Empty;
            List<string> stockList = stockService.GetAllStocks(page.GetValueOrDefault(), size.GetValueOrDefault());
            if (stockList == null)
            {
                logger.LogCritical("StockServise is unavailable");
                message = "Stock Service is unavailable";
            }
            List<string> transferList = transferService.GetAllTransfers(page.GetValueOrDefault(), size.GetValueOrDefault());
            if (transferList == null)
            {
                logger.LogCritical("TransferService is unavailable");
                message = "TranserService is unavailable";
                if (stockList == null)
                {
                    logger.LogCritical("Transfer & Stock Services are unavailable both");
                    message += " and StockService is also unavailable";
                    return StatusCode(500, message);
                }
                return StatusCode(200, stockList);
            }
            else
            {
                stockList.Add("");
                foreach (var t in transferList)
                    stockList.Add(t);
                return StatusCode(200, stockList);
            }
        }
    }
}
