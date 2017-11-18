using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;

namespace Gateway.Services
{
    public interface IStockService
    {
        Task<List<string>> GetAllStocks(int page, int size);
        Task<HttpResponseMessage> BookStock(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseStock(StockTransferOrderModel item);
    }
}
