using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AggregationService.Models;
using AggregationService.Pagination;
using AggregationService.Services;
using Microsoft.AspNetCore.Mvc;
using AggregationService.Pagination;
using AggregationService.Queue;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;

namespace AggregationService.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private IOrderService orderService;
        private IStockService stockService;
        private ITransferService transferService;
        private ILogger<HomeController> logger;

        public HomeController(
            IOrderService orderService,
            IStockService stockService,
            ITransferService transferService,
            ILogger<HomeController> logger)
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
                var msg = "";
            if (stockId == null || value == null)
            {
                if (stockId == null)
                {
                    if (value == null)
                        msg = "Stock identificator and order value are invalid";
                    else
                        msg = "Stock identificator is invalid";
                }
                else
                    msg = "Order value is invalid";
                return StatusCode(400, msg);
            }
            var item = new StockTransferOrderModel();
            item.StockId = stockId.GetValueOrDefault();
            item.Value = value.GetValueOrDefault();

            var stockResp = await stockService.BookStock(item);
                if (stockResp == null)
                {
                    msg = "StockService is unavailable";
                    return StatusCode(503, msg);
                }
                if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    item.OrderStatus = 20;
                    msg = "Stock wasn't found";
                    return StatusCode(404, msg);
                }
                if (stockResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    item.OrderStatus = 30;
                    msg = "There isn't enough place";
                    return StatusCode(507, msg);
                }
                item.OrderStatus = 10;
                var transfResp = await transferService.FindTransfer(item);
                if (transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    stockResp = await stockService.RefuseStock(item);
                    msg = "TransferService is unavailable, so your order wasn't booked. Try again later";
                    return StatusCode(503, msg);
                }
                if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    item.OrderStatus = item.OrderStatus + 2;
                    stockResp = await stockService.RefuseStock(item);
                    msg = "There are no available transfers. Try again later";
                    return StatusCode(500, msg);
                }
                if (transfResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    item.OrderStatus += 3;
                    stockResp = await stockService.RefuseStock(item);
                    msg = "All transfers are busy. Try again later";
                    return StatusCode(507, msg);
                }
                item.OrderStatus += 1;
                //var code = await orderService.AddOrder(item);
            return StatusCode(200);
            //var stockList = await stockService.GetAllStocks(0, 0);
            //ViewBag.stockList = new SelectList(stockList, "Id", "Name",item.StockId);
            //return View(item);
        }

        [HttpPut("refuse")]
        public async Task<IActionResult> RefuseOrder(int? stockId, double? value, int? transferId)
        {
            var msg = "";
            if (stockId == null || value == null || transferId == null)
            {
                if (stockId == null)
                {
                    if (value == null)
                        msg = "Stock identificator and order value are invalid";
                    else if (transferId == null)
                    {
                        msg = "Stock identificator and transfer identificator are invalid";
                    }
                    else
                        msg = "Stock identificator is invalid";
                }
                else if(value == null)
                {
                    if (transferId == null)
                        msg = "Order value and transfer identificator are invalid";
                    else
                        msg = "Transfer identificator is invalid";
                }
                else
                {
                    msg = "Transfer identificator is invalid";
                }
                return StatusCode(400, msg);
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
                QueueProducer.GetIntoQueueTillSuccess(async () =>
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
                //return StatusCode(503, $"StockService: {(stockResp?.StatusCode != System.Net.HttpStatusCode.Forbidden ? "online" : "offline")}; " +
                //                       $"TransferService: {(transfResp?.StatusCode != System.Net.HttpStatusCode.Forbidden ? "online" : "offline")}");
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus = 20;
                msg = "Stock wasn't found";
                return StatusCode(404, msg);
            }
            item.OrderStatus = 90;
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                item.OrderStatus = item.OrderStatus + 2;
                msg = "No info for refusing transfer";
                return StatusCode(404, msg);
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.OrderStatus += 3;
                msg = "Can't find transfer for refuse";
                return StatusCode(404, msg);
            }
            item.OrderStatus += 9;
            //await orderService.RefuseOrder(item);
            return Ok();
        }

        //public async Task<IActionResult> GetInfo()
        //{
        //    //var stockList = await stockService.GetAllStocks(0, 0);
        //    //var trList = await transferService.GetAllTransfers(0, 0);
        //    //ViewBag.stockList = new SelectList(stockList, "Id", "Name");
        //    return View();
        //}

        [HttpGet("info")]
        public async Task<ObjectResult> GetInfo(int? page, int? size)
        {
            var msg = String.Empty;
            int maxPage = 0;
            if (page == null || size == null)
            {
                if (page == null)
                {
                    if (size == null)
                        msg = "Parameters page and size are invalid";
                    else
                        msg = "Page parameter is invalid";
                }
                else
                    msg = "Size parameter is invalid";
                return StatusCode(400, msg);
            }
            IEnumerable<string> stocks = Enumerable.Empty<string>();
            IEnumerable<string> transfers = Enumerable.Empty<string>();
            //ListForPagination<string> paginatedStockList = (await stockService.GetAllStocks(page.GetValueOrDefault(), size.GetValueOrDefault()));
            ListForPagination<string> stockList = await stockService.GetAllStocks(page.GetValueOrDefault(), size.GetValueOrDefault());
            List<string> transferList = await transferService.GetAllTransfers(page.GetValueOrDefault(), size.GetValueOrDefault());
            if (transferList == null)
            {
                logger.LogCritical("TransferService is unavailable");
                msg = "TranserService is unavailable";
                if (stockList == null)
                {
                    logger.LogCritical("Transfer & Stock Services are unavailable both");
                    msg += " and StockService is also unavailable";
                    return StatusCode(500, msg);
                }
                return StatusCode(200, stockList);
            }
            
            //stockList.Add("");
            //stockList.AddRange(transferList);

            return StatusCode(200,msg);
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }


        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        //public IActionResult Error400(string msg)
        //{
        //    ViewBag.msg = msg;
        //    return View();
        //}
        //public IActionResult Error500(string msg)
        //{
        //    ViewBag.msg = msg;
        //    return View();
        //}

        //public IActionResult Error503(string msg)
        //{
        //    ViewBag.msg = msg;
        //    return View();
        //}

        //public IActionResult Error404(string msg)
        //{
        //    ViewBag.msg = msg;
        //    return View();
        //}

        //public IActionResult Error507(string msg)
        //{
        //    ViewBag.msg = msg;
        //    return View();
        //}

        //public IActionResult CorrectAddedOrder()
        //{
        //    return View();
        //}
    }
}
