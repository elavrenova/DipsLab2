using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;
using Gateway.Pagination;
using Gateway.Queue;
using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gateway.Controllers
{
    public class AggregationController : Controller
    {
        private IOrderService orderService;
        private IStockService stockService;
        private ITransferService transferService;
        private ILogger<HomeController> logger;

        public AggregationController(
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

        public async Task<ObjectResult> AddNewOrder(StockTransferOrderModel item)
        {
            var msg = "";
            var stockResp = await stockService.BookStock(item);
            if (stockResp == null)
            {
                msg = "StockService is unavailable";
                return StatusCode(503, msg);
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                msg = "Stock wasn't found";
                return StatusCode(404, msg);
            }
            if (stockResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                msg = "There isn't enough place";
                return StatusCode(507, msg);
            }
            item.Status = 10;
            var transfResp = await transferService.FindTransfer(item);
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                stockResp = await stockService.RefuseStock(item);
                msg = "TransferService is unavailable, so your order wasn't booked. Try again later";
                return StatusCode(503, msg);
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                item.Status = item.Status + 2;
                stockResp = await stockService.RefuseStock(item);
                msg = "There are no available transfers. Try again later";
                return StatusCode(404, msg);
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                stockResp = await stockService.RefuseStock(item);
                msg = "All transfers are busy. Try again later";
                return StatusCode(507, msg);
            }
            var trId = transfResp.Content.ReadAsStringAsync().Result;
            item.TransferId = Int16.Parse(trId);
            item.Status += 1;
            var ordResp = await orderService.AddOrder(item);
            return StatusCode(200,msg);
        }

        public async Task<ObjectResult> RefuseOrder(StockTransferOrderModel item)
        {
            var msg = "";
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
                item.Status = 20;
                msg = "Stock wasn't found";
                return StatusCode(404, msg);
            }
            item.Status = 90;
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                item.Status = item.Status + 2;
                msg = "No info for refusing transfer";
                return StatusCode(404, msg);
            }
            if (transfResp?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                item.Status += 3;
                msg = "Can't find transfer for refuse";
                return StatusCode(404, msg);
            }
            item.Status += 9;
            await orderService.RefuseOrder(item);
            return StatusCode(200, msg);
        }

        //[HttpGet("get_info")]
        //public async Task<ObjectResult> GetInfo(int? page, int? size)
        //{
        //    var msg = String.Empty;
        //    int maxPage = 0;
        //    if (page == null || size == null)
        //    {
        //        if (page == null)
        //        {
        //            if (size == null)
        //                msg = "Parameters page and size are invalid";
        //            else
        //                msg = "Page parameter is invalid";
        //        }
        //        else
        //            msg = "Size parameter is invalid";
        //        return StatusCode(400, msg);
        //    }
        //    IEnumerable<string> stocks = Enumerable.Empty<string>();
        //    IEnumerable<string> transfers = Enumerable.Empty<string>();
        //    //ListForPagination<string> paginatedStockList = (await stockService.GetAllStocks(page.GetValueOrDefault(), size.GetValueOrDefault()));
        //    ListForPagination<string> stockList = await stockService.GetAllStocks(page.GetValueOrDefault(), size.GetValueOrDefault());
        //    List<string> transferList = await transferService.GetAllTransfers(page.GetValueOrDefault(), size.GetValueOrDefault());
        //    if (transferList == null)
        //    {
        //        //logger.LogCritical("TransferService is unavailable");
        //        msg = "TranserService is unavailable";
        //        if (stockList == null)
        //        {
        //            //logger.LogCritical("Transfer & Stock Services are unavailable both");
        //            msg += " and StockService is also unavailable";
        //            return StatusCode(500, msg);
        //        }
        //        return StatusCode(200, stockList);
        //    }

        //    //stockList.Add("");
        //    //stockList.AddRange(transferList);

        //    return StatusCode(200, msg);
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}