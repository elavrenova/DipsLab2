using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DipsLab2.Services.Implementations
{
    public class StockService : Service, IStockService
    {
        public StockService(IConfiguration configuration) : 
            base(configuration.GetSection("Urls")["Stock"]) { }

        public async Task<List<string>> GetAllStocks(int page, int perpage)
        {
            var res = await Get($"?page={page}&perpage={perpage}");
            string response = await res.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(response);
            }
            catch
            {
                return null;
            }
        }

        //public async Task<HttpResponseMessage> GetStockById(int id, StockModel stock, double val) =>
        //    await PutJson("book_s/" + stock.Id, stock);

        public async Task<HttpResponseMessage> BookStock(StockTransferOrderModel item) =>
            await PutJson("book_s/"+item.StockId, item);

        public async Task<HttpResponseMessage> RefuseStock(StockTransferOrderModel item) =>
            await PutJson("refuse_s/" + item.StockId, item);
    }
}
