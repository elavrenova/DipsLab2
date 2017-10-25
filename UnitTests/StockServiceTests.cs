using System.Collections.Generic;
using DipsLab2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Models;

namespace UnitTests
{
    [TestClass]
    public class StockServiceTests
    {
        private StockContext dbContext;

        [TestInitialize]
        public void TestInitialize()
        {
           // dbContext = GetDbContext();
        }

        [TestMethod]
        public void TestMethod1()
        {
             
        }


        //private StockContext GetDbContext(List<Stock> news = null)
        //{
        //    if (news == null)
        //        news = new List<Stock>();
        //}
    }
}
