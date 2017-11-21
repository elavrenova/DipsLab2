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
using DipsLab2.Queue;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DipsLab2.Controllers
{
    [Route("")]
    public class AggregationController : Controller
    {
        private IOrderService orderService;
        private IStockService stockService;
        private ITransferService transferService;
        private ILogger<AggregationController> logger;

        public AggregationController(
            IOrderService orderService,
            IStockService stockService,
            ITransferService transferService,
            ILogger<AggregationController> logger)
        {
            this.orderService = orderService;
            this.stockService = stockService;
            this.transferService = transferService;
            this.logger = logger;
        }

        [HttpGet("order")]
        public async Task<IActionResult> AddOrder()
        {
            var stockList = await stockService.GetStocks(0, 0);
            ViewBag.stockList = new SelectList(stockList, "Id", "Name");
            return Ok();
        }

        [HttpPost("order")]
        public async Task<IActionResult> AddOrder(int? stockId, double? value)
        {
            if (stockId == null || value == null)
            {
                return StatusCode(500, "Parameters are invalid");
            }
            var item = new StockTransferOrderModel();
            item.StockId = stockId.GetValueOrDefault();
            item.Value = value.GetValueOrDefault();

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
            var transfResp = await transferService.FindTransfer(item);
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                stockResp = await stockService.RefuseStock(item);
                return StatusCode(503, "TransferService is unavailable, so recall of refusing stock was made");
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
            var code = await orderService.AddOrder(item);
            return Ok();
        }


        [HttpPut("refuse")]
        public async Task<IActionResult> RefuseOrder(int? stockId, double? value, int? transferId)
        {
            if (stockId == null || value == null || transferId == null)
            {
                return StatusCode(500, "Parameters are invalid");
            }
            var item = new StockTransferOrderModel();
            item.StockId = stockId.GetValueOrDefault();
            item.Value = value.GetValueOrDefault();
            item.TransferId = transferId.GetValueOrDefault();

            var stockResp = await stockService.RefuseStock(item);
            var transfResp = await transferService.RefuseTransfer(item);
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.Forbidden || transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                if (stockResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    await transferService.BookTransfer(item);
                if (transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    await stockService.BookStock(item);
                QueueProducer.GetIntoQueueTillSuccess(async() =>
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            stockResp = await stockService.RefuseStock(item);
                            transfResp = await transferService.RefuseTransfer(item);
                            if (stockResp?.StatusCode == System.Net.HttpStatusCode.Forbidden || transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                if (stockResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                    await transferService.BookTransfer(item);
                                if (transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                    await stockService.BookStock(item);
                                return false;
                            }
                            return true;
                        }
                        catch
                        {
                        }
                        return false;
                    }
                });
                return StatusCode(503, $"StockService {(stockResp?.StatusCode != System.Net.HttpStatusCode.Forbidden ? "online" : "offline")}; " +
                                       $"TransferService: {(transfResp?.StatusCode != System.Net.HttpStatusCode.Forbidden ? "online" : "offline")}");
                
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus = 20;
                return BadRequest("Stock wasn't found");
            }
            item.OrderStatus = 90;
            
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
            //await orderService.RefuseOrder(item);
            return Ok();
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(int? page, int? size)
        {
            
            if (page == null || size == null)
            {
                var msg = "";
                if (page == null)
                {
                    if (size == null)
                        msg = "Parameters page and size are invalid";
                    else
                    {
                        msg = "Page parameter is not valid";
                    }
                }  
                else
                {
                    msg = "Size parameter is invalid";
                }

                return RedirectToAction("Error400",new{msg});
            }
            var message = string.Empty;
            List<string> stockList = await stockService.GetAllStocks(page.GetValueOrDefault(), size.GetValueOrDefault());
            List<string> transferList = await transferService.GetAllTransfers(page.GetValueOrDefault(), size.GetValueOrDefault());
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
            stockList.Add("");
            stockList.AddRange(transferList);

            return View(stockList);
        }

        public IActionResult Error400(string msg)
        {
            ViewBag.msg = msg;
            return View();

        }
    }
}
