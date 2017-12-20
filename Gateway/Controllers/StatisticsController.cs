using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatisticServer.Models;
using Gateway.Services;

namespace Gateway.Controllers
{
    [Produces("application/json")]
    [Route("admin")]
    public class StatisticsController : Controller
    {
        private IStatisticsService statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            this.statisticsService = statisticsService;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> RequestsStatistics()
        {
            return View();
        }

        [HttpGet("requests/values")]
        public async Task<List<RequestModel>> Requests()
        {
            return await statisticsService.GetRequests();
        }

        [HttpGet("requests/values/detail")]
        public async Task<List<RequestDetailModel>> RequestsDetail()
        {
            return await statisticsService.GetRequestsDetailed();
        }

        [HttpGet("operations")]
        public async Task<IActionResult> OperationsStatistics()
        {
            return View();
        }

        [HttpGet("operations/detail")]
        public async Task<List<OperationDetailModel>> OperationsDetail()
        {
            return await statisticsService.GetOperationsDetailed();
        }

        [HttpGet("orders/added")]
        public async Task<IActionResult> AddOrderStatistics()
        {
            return View();
        }

        [HttpGet("orders/added/values")]
        public async Task<List<OrderAdditionModel>> OrdersAddedValue()
        {
            return await statisticsService.GetOrdersAdditions();
        }
        [HttpGet("orders/added/detail")]
        public async Task<List<OrderAdditionDetailModel>> OrdersAddedDetail()
        {
            return await statisticsService.GetOrdersAdditionsDetailed();
        }

        [HttpGet("orders/ordersvalues")]
        public async Task<IActionResult> OrderValueStatistics()
        {
            return View();
        }

        [HttpGet("orders/ordersvalues/values")]
        public async Task<List<OrderAdditionModel>> OrderValues()
        {
            return await statisticsService.GetOrdersAdditions();
        }

        [HttpGet("orders/ordersvalues/detail")]
        public async Task<List<OrderAdditionDetailModel>> OrderValuesDetail()
        {
            return await statisticsService.GetOrdersAdditionsDetailed();
        }

    }
}