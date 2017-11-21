using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AggregationService.Models;
using AggregationService.Pagination;

namespace AggregationService.Services
{
    public interface IStockService
    {
        Task<ListForPagination<string>> GetAllStocks(int page, int size);
        Task<List<StockModel>> GetStocks(int page, int size);
        Task<HttpResponseMessage> BookStock(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseStock(StockTransferOrderModel item);
    }
}
