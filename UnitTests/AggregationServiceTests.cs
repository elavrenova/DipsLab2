using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DipsLab2.Controllers;
using DipsLab2.Models;
using DipsLab2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class AggregationServiceTests
    {
        private ILogger<AggregationController> logger;
        private IStockService stockService;
        private IOrderService orderService;
        private ITransferService transferService;

        [TestInitialize]
        public void TestInitialize()
        {
            var loggerMock = new Mock<ILogger<AggregationController>>();
            logger = loggerMock.Object;
            stockService = GetStockService();
            transferService = GetTransferService();
            orderService = GetEmptyOrderService();
        }

        [TestMethod]
        public void TestAddOrderValid()
        {
            stockService = GetStockService(Code: HttpStatusCode.OK);
            transferService = GetTransferService(Code: HttpStatusCode.OK);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            var result = aggregationController.AddOrder(stockId,value).Result;
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void TestRefuseOrderValid()
        {
            stockService = GetStockService(Code: HttpStatusCode.OK);
            transferService = GetTransferService(Code: HttpStatusCode.OK);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            int transferId = 1;
            var result = aggregationController.RefuseOrder(stockId,value,transferId).Result;
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void TestAddOrderValidStockNotFound()
        {
            stockService = GetStockService(Code: HttpStatusCode.NotFound);
            transferService = GetTransferService(Code: HttpStatusCode.OK);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            var result = aggregationController.AddOrder(stockId,value).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestRefuseOrderValidStockNotFound()
        {
            stockService = GetStockService(Code: HttpStatusCode.NotFound);
            transferService = GetTransferService(Code: HttpStatusCode.OK);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            int transferId = 1;
            var result = aggregationController.RefuseOrder(stockId,value,transferId).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }


        [TestMethod]
        public void TestAddOrderValidNotEnoughPlace()
        {
            stockService = GetStockService(Code: HttpStatusCode.BadRequest);
            transferService = GetTransferService(Code: HttpStatusCode.OK);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            var result = aggregationController.AddOrder(stockId,value).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestAddOrderValidNoTransfers()
        {
            stockService = GetStockService(Code: HttpStatusCode.OK);
            transferService = GetTransferService(Code: HttpStatusCode.NoContent);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            var result = aggregationController.AddOrder(stockId,value).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestRefuseOrderValidNoTransferInfo()
        {
            stockService = GetStockService(Code: HttpStatusCode.OK);
            transferService = GetTransferService(Code: HttpStatusCode.NoContent);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            int transferId = 1;
            var result = aggregationController.RefuseOrder(stockId,value,transferId).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestAddOrderValidBusyTransfers()
        {
            stockService = GetStockService(Code: HttpStatusCode.OK);
            transferService = GetTransferService(Code: HttpStatusCode.BadRequest);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            var result = aggregationController.AddOrder(stockId,value).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestRefuseOrderValidTransferNotFound()
        {
            stockService = GetStockService(Code: HttpStatusCode.OK);
            transferService = GetTransferService(Code: HttpStatusCode.NotFound);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            int transferId = 1;
            var result = aggregationController.RefuseOrder(stockId,value,transferId).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestGetInfoValid()
        {
            var stocks = new List<string> { "stock1", "stock2" };
            stockService = GetStockService(stocks);
            var transfers = new List<string> { "tr1", "tr2" };
            transferService = GetTransferService(transfers);
            var stocksandtransfers = new List<string> { "stock1", "stock2", "*", "tr1", "tr2" };
            var aggregationController = GetAggregationController();
            int page = 0;
            int size = 0;
            var result = aggregationController.GetInfo(page,size).Result;
            Assert.AreEqual(stocksandtransfers.Count, result);
        }

        [TestMethod]
        public void TestGetInfoNotValid()
        {
            var stocks = new List<string> {};
            stockService = GetStockService(stocks);
            var transfers = new List<string> {};
            transferService = GetTransferService(transfers);
            var stocksandtransfers = new List<string> { };
            var aggregationController = GetAggregationController();
            int page = 0;
            int size = 0;
            var result = aggregationController.GetInfo(page,size).Result;
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestAddOrderNotValidEmptyStockService()
        {
            stockService = GetEmptyStockService();
            transferService = GetTransferService(Code: HttpStatusCode.OK);
            var aggregationController = GetAggregationController();
            int stockId = 1;
            double value = 50.0;
            var result = aggregationController.AddOrder(stockId, value).Result;
            Assert.IsTrue(result is BadRequestObjectResult);
        }


        private IStockService GetStockService(List<string> getStocks = null, HttpStatusCode Code = HttpStatusCode.OK)
        {
            return Mock.Of<IStockService>(s =>
                s.RefuseStock(It.IsAny<StockTransferOrderModel>()) == Task.FromResult(GetResponseMessage(Code)) &&
                s.BookStock(It.IsAny<StockTransferOrderModel>()) == Task.FromResult(GetResponseMessage(Code)) &&
                s.GetAllStocks(It.IsAny<int>(), It.IsAny<int>()) == getStocks);
        }
        private ITransferService GetTransferService(List<string> getTransfers = null, HttpStatusCode Code = HttpStatusCode.OK)
        {
            return Mock.Of<ITransferService>(s =>
                s.RefuseTransfer(It.IsAny<StockTransferOrderModel>()) == Task.FromResult(GetResponseMessage(Code)) &&
                s.BookTransfer(It.IsAny<StockTransferOrderModel>()) == Task.FromResult(GetResponseMessage(Code)) &&
                s.GetAllTransfers(It.IsAny<int>(), It.IsAny<int>()) == getTransfers);
        }

        private IStockService GetEmptyStockService()
        {
            return Mock.Of<IStockService>();
        }

        private IOrderService GetEmptyOrderService()
        {
            return Mock.Of<IOrderService>();
        }

        private ITransferService GetEmptyTransferService()
        {
            return Mock.Of<ITransferService>();
        }

        private AggregationController GetAggregationController()
        {
            return new AggregationController(orderService,stockService, transferService, logger);
        }

        private HttpResponseMessage GetResponseMessage(HttpStatusCode registerCode)
        {
            return Mock.Of<HttpResponseMessage>(hwr => hwr.StatusCode == registerCode);
        }

    }
}
