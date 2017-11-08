using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DipsLab2.Controllers;
using DipsLab2.Models;
using DipsLab2.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    class AggregationServiceTests
    {
        private ILogger<AggregationController> logger;
        private IStockService stockService;
        private ITransferService transferService;

        [TestInitialize]
        public void TestInitialize()
        {
            var loggerMock = new Mock<ILogger<AggregationController>>();
            logger = loggerMock.Object;
            stockService = GetStockService();
            transferService = GetTransferService();
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
                s.GetAllTransfers(It.IsAny<int>(), It.IsAny<int>()) == Task.FromResult(getTransfers));
        }

        private IStockService GetEmptyStockService()
        {
            return Mock.Of<IStockService>();
        }

        private ITransferService GetEmptyTransferService()
        {
            return Mock.Of<ITransferService>();
        }

        private AggregationController GetAggregationController()
        {
            return new AggregationController(stockService, transferService, logger);
        }

        private HttpResponseMessage GetResponseMessage(HttpStatusCode registerCode)
        {
            return Mock.Of<HttpResponseMessage>(hwr => hwr.StatusCode == registerCode);
        }

    }
}
