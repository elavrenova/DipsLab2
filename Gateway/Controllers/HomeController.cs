using Gateway.Models;
using Gateway.Pagination;
using Gateway.Queue;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Authorisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Gateway.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private AggregationController aggregationController;
        private IStockService stockService;
        private ITransferService transferService;
        private IOrderService orderService;


        public HomeController(AggregationController aggregationController, IStockService stockService,
            ITransferService transferService, IOrderService orderService)
        {
            this.aggregationController = aggregationController;
            this.stockService = stockService;
            this.transferService = transferService;
            this.orderService = orderService;
        }

        [HttpGet("order")]
        public async Task<IActionResult> AddOrder()
        {
            var stockList = await stockService.GetStocks();
            if (stockList == null)
            {
                var resp = StatusCode(500, "StockService is unavailable. Please, try again later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.stockList = new SelectList(stockList, "Id", "Name");
            return View();
        }

        [HttpPost("order")]
        public async Task<IActionResult> AddOrder(StockTransferOrderModel item)
        {
            if (ModelState.IsValid)
            {
                var resp = await aggregationController.AddNewOrder(item);
                if (resp.StatusCode == 200)
                {
                    return RedirectToAction("CorrectlyAddedOrder");
                }
                return View("MyError", new ErrorModel(resp));
            }
            var stockList = await stockService.GetStocks();
            if (stockList == null)
            {
                var resp = StatusCode(500, "StockService is unavailable. Please, try again later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.stockList = new SelectList(stockList, "Id", "Name", item.StockId);
            return View();
        }

        [HttpGet("refuse")]
        public async Task<IActionResult> RefuseOrder()
        {
            var stockList = await stockService.GetStocks();
            ViewBag.stockList = new SelectList(stockList, "Id", "Name");
            if (stockList == null)
            {
                var resp = StatusCode(500, "StockService is unavailable. Please, try later");
                return View("MyError", new ErrorModel(resp));
            }
            var transfList = await transferService.GetTransfers();
            if (transfList == null)
            {
                var resp = StatusCode(500, "TransferService is unavailable. Please, try again later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.transfList = new SelectList(transfList, "Id", "Name");
            return View();
        }

        [HttpPost("refuse")]
        public async Task<IActionResult> RefuseOrder(StockTransferOrderModel item)
        {
            if (ModelState.IsValid)
            {
                var resp = await aggregationController.RefuseOrder(item);
                if (resp.StatusCode == 200)
                {
                    return RedirectToAction("CorrectlyRefusedOrder");
                }
                return View("MyError", new ErrorModel(resp));
            }
            var stockList = await stockService.GetStocks();
            if(stockList == null)
            {
                var resp = StatusCode(500, "Your data wasn't correct. And StockService is unavailable now. Please, try later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.stockList = new SelectList(stockList, "Id", "Name", item.StockId);
            var transfList = await transferService.GetTransfers();
            if (transfList == null)
            {
                var resp = StatusCode(500, "Your data wasn't correct. And TransferService is unavailable. Please, try again later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.transfList = new SelectList(transfList, "Id", "Name");
            return View();
        }

        [HttpGet("refuse_by_order_id")]
        public async Task<IActionResult> RefuseOrderByOrderId()
        {
            var ordList = await orderService.GetOrders();
            if (ordList == null)
            {
                var resp = StatusCode(500, "OrderService is unavailable. Please, try again later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.ordList = new SelectList(ordList, "Id", "Id");
            return View();
        }

        [HttpPost("refuse_by_order_id")]
        public async Task<IActionResult> RefuseOrderByOrderId(StockTransferOrderModel item)
        {
            if (ModelState.IsValid)
            {
                var itm = await orderService.GetById(item.Id);
                var resp = await aggregationController.RefuseOrder(itm);
                if (resp.StatusCode == 200)
                {
                    return RedirectToAction("CorrectlyRefusedOrder");
                }
                return View("MyError", new ErrorModel(resp));
            }
            var ordList = await orderService.GetOrders();
            if (ordList == null)
            {
                var resp = StatusCode(500, "OrderService is unavailable. Please, try again later");
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.ordList = new SelectList(ordList, "Id", "Id");
            return View();
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo()
        {
            var msg = String.Empty;
            var stockList = await stockService.GetStocks();
            var transfList = await transferService.GetTransfers();
            if (transfList == null)
            {
                msg = "TranserService is unavailable";
                if (stockList == null)
                {
                    msg += " and StockService is also unavailable";
                    var resp = StatusCode(503, msg);
                    return View("MyError", new ErrorModel(resp));
                }
                ViewBag.stockList = stockList.ToList();
                return View("InfoDegradation");
            }
            ViewBag.stockList = stockList.ToList();
            ViewBag.transferList = transfList.ToList();
            return View();
        }

        [HttpGet("stocks")]
        public async Task<IActionResult> GetStocks()
        {
            var msg = String.Empty;
            var stockList = await stockService.GetStocks();
            if (stockList == null)
            {
                msg += "StockService is unavailable";
                var resp = StatusCode(503, msg);
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.stockList = stockList.ToList();
            return View("InfoDegradation");
        }

        [Authorize]
        [HttpGet("getoaoth2")]
        public async Task<string> GetStocksOAuth()
        {
            string test = "Correct oaoth2 test";
            return test;
        }

        [HttpGet("transfers")]
        public async Task<IActionResult> GetTransfers()
        {
            var msg = String.Empty;
            var transfList = await transferService.GetTransfers();
            if (transfList == null)
            {
                msg += "TransferService is unavailable";
                var resp = StatusCode(503, msg);
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.transfList = transfList.ToList();
            return View("Transfers");   
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var msg = String.Empty;
            var stockList = await stockService.GetStocks();
            if (stockList == null)
            {
                var resp = StatusCode(500, "StockService is unavailable");
                return View("MyError", new ErrorModel(resp));
            }
               
            var transfList = await transferService.GetTransfers();
            if (transfList == null)
            {
                var resp = StatusCode(500, "StockService is unavailable");
                return View("MyError", new ErrorModel(resp));
            }
            var orderList = await orderService.GetOrders();
            if (orderList == null)
            {
                msg += "OrderService is unavailable";
                var resp = StatusCode(503, msg);
                return View("MyError", new ErrorModel(resp));
            }
            ViewBag.stockList = stockList.ToList();
            ViewBag.transfList = transfList.ToList();
            ViewBag.orderList = orderList.ToList();
            return View("Orders");
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet("")]
        //public IActionResult OAuth2Callback(string code, string scope)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var tokenReponse = client.PostAsync("http://localhost:50539/connect/token?", new FormUrlEncodedContent(new Dictionary<string, string>
        //        {
        //            {"grant_type", "authorization_code" },
        //            {"code", code },
        //            {"redirect_uri", "http://localhost:60992" },
        //            {"client_id", "clientId" },
        //            {"client_secret", "Secret" }
        //        }));
        //    }
        //        return Ok();
        //}

        [HttpGet("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("success_adding_order")]
        public IActionResult CorrectlyAddedOrder()
        {
            return View();
        }

        [HttpGet("success_refusing_order")]
        public IActionResult CorrectlyRefusedOrder()
        {
            return View();
        }

    }
}
