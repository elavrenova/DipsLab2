using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;

namespace DipsLab2.Services
{
    public interface IStockService
    {
        Task<List<string>> GetAllStocks(int page, int perpage);
        Task<HttpResponseMessage> BookStock(StockModel stock);
        Task<HttpResponseMessage> RefuseStock(StockModel stock);
    }
}
