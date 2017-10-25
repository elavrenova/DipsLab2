using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using DipsLab2.Models;
using DipsLab2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockService.Controllers;
using StockService.Models;

namespace UnitTests 
{
    [TestClass]
    public class StockServiceTests
    {
        private StockContext dbContext;
        private ILogger<StockController> logger;


        [TestInitialize]
        public void TestInitialize()
        {
            dbContext = GetDbContext();
            logger = Mock.Of<ILogger<StockController>>();
        }

        [TestMethod]
        public void TestMethod1()
        {
            List<Stock> stocks = new List<Stock> {new Stock()};
            dbContext = GetDbContext(stocks);
            var stockController = GetStockController();

            var result = stockController.GetAllStocks(0, 0);
            Assert.IsTrue(result.Count == stocks.Count);
        }

        [TestMethod]
        public void TestGetStocksNotValid()
        {
            var stockController = GetStockController();

            var result = stockController.GetAllStocks(0, 0);
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void TestAddStockValid()
        {
            var stocks = new List<Stock>();
            dbContext = GetDbContext(stocks);
            var stockController = GetStockController();

            var result = stockController.AddStock(Mock.Of<StockModel>());
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void TestAddStockNotValid()
        {
            var stocks = new List<Stock> { new Stock() };
            dbContext = GetDbContext(stocks);
            var stockController = GetStockController();

            var result = stockController.AddStock(Mock.Of<StockModel>());
            Assert.IsFalse(result is OkResult);
        }

        private StockController GetStockController()
        {
            return new StockController(dbContext, logger);
        }


        private StockContext GetDbContext(List<Stock> stocks = null)
        {
            if (stocks == null)
                stocks = new List<Stock>();
            return Mock.Of<StockContext>(db =>
                db.Stocks == GetStocks(stocks));
        }
        private DbSet<Stock> GetStocks(List<Stock> stocks)
        {
            var newsQueryable = stocks.AsQueryable();
            var mockSet = new Mock<DbSet<Stock>>();
            var mockSetQueryable = mockSet.As<IQueryable<Stock>>();
            mockSetQueryable.Setup(m => m.Provider).Returns(newsQueryable.Provider);
            mockSetQueryable.Setup(m => m.Expression).Returns(newsQueryable.Expression);
            mockSetQueryable.Setup(m => m.ElementType).Returns(newsQueryable.ElementType);
            mockSetQueryable.Setup(m => m.GetEnumerator()).Returns(stocks.GetEnumerator());
            mockSet.Setup(d => d.Add(It.IsAny<Stock>())).Callback<Stock>(u => stocks.Add(u));
            return mockSet.Object;
        }
        private HttpResponseMessage GetResponseMessage(HttpStatusCode registerCode)
        {
            return Mock.Of<HttpResponseMessage>(hwr => hwr.StatusCode == registerCode);
        }
    }
}
