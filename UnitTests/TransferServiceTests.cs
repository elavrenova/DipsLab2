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
        private const double carrying = 5000.0;
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

        [TestMethod]
        public void TestBookTransferValid()
        {
            var transfers = new List<Transfer> { new Transfer { Carrying = carrying, Status = 0, Id = 1} };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.BookTransfer(Mock.Of<StockTransferOrderModel>(x => x.Value == 500.0));
            Assert.IsTrue(result is OkResult);
        }
        [TestMethod]
        public void TestBookTransferNotValidBusy()
        {
            var transfers = new List<Transfer> { new Transfer { Carrying = carrying, Status = 1 } };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.BookTransfer(Mock.Of<StockTransferOrderModel>(x => x.Value == 500.0));
            Assert.IsTrue(result is BadRequestObjectResult);
        }

        [TestMethod]
        public void TestBookTransferNotValidEmpty()
        {
            var transferController = GetTransferController();

            var result = transferController.BookTransfer(Mock.Of<StockTransferOrderModel>(x => x.Value == 500.0));
            Assert.IsTrue(result is NoContentResult);
        }

        [TestMethod]
        public void TestRefuseTransferValid()
        {
            var transfers = new List<Transfer> { new Transfer { Carrying = carrying, Status = 1, Id = 1 } };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.RefuseTransfer(Mock.Of<StockTransferOrderModel>(x => x.TransferId == 1));
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void TestRefuseTransferNotValidNotFound()
        {
            var transfers = new List<Transfer> { new Transfer { Carrying = carrying, Status = 1, Id = 1 } };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.RefuseTransfer(Mock.Of<StockTransferOrderModel>(x => x.TransferId == 5));
            Assert.IsTrue(result is NotFoundResult);
        }

        [TestMethod]
        public void TestRefuseTransferNotValidNullItem()
        {
            var transfers = new List<Transfer> { new Transfer { Carrying = carrying, Status = 1, Id = 1 } };
            dbContext = GetDbContext(transfers);
            var transferController = GetTransferController();

            var result = transferController.RefuseTransfer(Mock.Of<StockTransferOrderModel>());
            Assert.IsTrue(result is NoContentResult);
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
    }
}
