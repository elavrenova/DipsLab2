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
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Gateway.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private AggregationController aggregationController;
        private IStockService stockService;
        private ITransferService transferService;

        public HomeController(AggregationController aggregationController, IStockService stockService,
            ITransferService transferService)
        {
            this.aggregationController = aggregationController;
            this.stockService = stockService;
            this.transferService = transferService;
        }

        [HttpGet("order")]
        public async Task<IActionResult> AddOrder()
        {
            var stockList = await stockService.GetStocks();
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
            ViewBag.stockList = new SelectList(stockList, "Id", "Name", item.StockId);
            return View();
        }

        [HttpGet("refuse")]
        public async Task<IActionResult> RefuseOrder()
        {
            var stockList = await stockService.GetStocks();
            ViewBag.stockList = new SelectList(stockList, "Id", "Name");
            var transfList = await transferService.GetTransfers();
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
            ViewBag.stockList = new SelectList(stockList, "Id", "Name", item.StockId);
            var transfList = await transferService.GetTransfers();
            ViewBag.transfList = new SelectList(transfList, "Id", "Name");
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

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

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
