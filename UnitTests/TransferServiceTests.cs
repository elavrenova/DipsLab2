using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockService.Controllers;
using StockService.Models;
using TransferService.Controllers;
using TransferService.Models;

namespace UnitTests
{
    [TestClass]
    public class TransferServiceTests
    {
        private TransferContext dbContext;
        private ILogger<TransferController> logg;

        [TestInitialize]
        public void TestInitialize()
        {
            dbContext = GetDbContext();
            logg = Mock.Of<ILogger<TransferController>>();
        }
        [TestMethod]
        public void TestGetTransfersValid()
        {
            List<Transfer> transfers = new List<Transfer> { new Transfer() };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.GetAllTransfers( 0, 0).Result;
            Assert.IsTrue(result.Count == transfers.Count);
        }

        [TestMethod]
        public void TestGetTransfersNotValid()
        {
            var transferController = GetTransferController();

            var result = transferController.GetAllTransfers(0, 0).Result;
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void TestAddTransferValid()
        {
            var transfers = new List<Transfer>();
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.AddTransfer(Mock.Of<TransferModel>());
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void TestAddTransferNotValid()
        {
            var transfers = new List<Transfer> { new Transfer() };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.AddTransfer(Mock.Of<TransferModel>());
            Assert.IsFalse(result is OkResult);
        }


        private TransferController GetTransferController()
        {
            return new TransferController(dbContext, logg);
        }

        private TransferContext GetDbContext(List<Transfer> transfers = null)
        {
            if (transfers == null)
                transfers = new List<Transfer>();
            return Mock.Of<TransferContext>(db =>
                db.Transfers == GetTransfers(transfers));
        }
        private DbSet<Transfer> GetTransfers(List<Transfer> transfers)
        {
            var newsQueryable = transfers.AsQueryable();
            var mockSet = new Mock<DbSet<Transfer>>();
            var mockSetQueryable = mockSet.As<IQueryable<Transfer>>();
            mockSetQueryable.Setup(m => m.Provider).Returns(newsQueryable.Provider);
            mockSetQueryable.Setup(m => m.Expression).Returns(newsQueryable.Expression);
            mockSetQueryable.Setup(m => m.ElementType).Returns(newsQueryable.ElementType);
            mockSetQueryable.Setup(m => m.GetEnumerator()).Returns(transfers.GetEnumerator());
            mockSet.Setup(d => d.Add(It.IsAny<Transfer>())).Callback<Transfer>(u => transfers.Add(u));
            return mockSet.Object;
        }
        private HttpResponseMessage GetResponseMessage(HttpStatusCode registerCode)
        {
            return Mock.Of<HttpResponseMessage>(hwr => hwr.StatusCode == registerCode);
        }
    }
}
