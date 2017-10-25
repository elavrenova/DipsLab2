using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DipsLab2.Services.Implementations
{
    public class OrderService : Service, IOrderService
    {
        public OrderService(IConfiguration configuration) : 
            base(configuration.GetSection("Urls")["Order"]) { }

        //public async Task<List<string>> GetAllOrders(int page, int perpage)
        //{
        //    var res = await Get($"?page={page}&perpage={perpage}");
        //    string response = await res.Content.ReadAsStringAsync();

        //    try
        //    {
        //        return JsonConvert.DeserializeObject<List<string>>(response);
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //}

        public async Task<HttpResponseMessage> AddOrder(StockTransferOrderModel item)
        {
            return await PostJson("", item);
        }

        public async Task<HttpResponseMessage> UpdateOrder(StockTransferOrderModel item)
        {
            return await PutJson("", item);
        }

    }
}
    