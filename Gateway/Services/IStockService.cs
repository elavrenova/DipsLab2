using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;
using Gateway.Pagination;

namespace Gateway.Services
{
    public interface IStockService
    {
        Task<ListForPagination<string>> GetAllStocks(int page, int size);
        Task<List<StockModel>> GetStocks();
        Task<HttpResponseMessage> BookStock(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseStock(StockTransferOrderModel item);
    }
}
