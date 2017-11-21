using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gateway.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gateway.Services.Implementations
{
    public class OrderService : Service, IOrderService
    {
        public OrderService(IConfiguration configuration) : 
            base("http://orderservicedl2.azurewebsites.net/") { }

        public List<string> GetAllOrders(int page, int size)
        {
            var res = Get($"?page={page}&size={size}").Result;
            string response = res.Content.ReadAsStringAsync().Result;
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(response);
            }
            catch
            {
                return null;
            }

        }

        public async Task<HttpResponseMessage> AddOrder(StockTransferOrderModel item)
        {
            return await PostJson("", item);
        }

        public async Task<HttpResponseMessage> RefuseOrder(StockTransferOrderModel item)
        {
            return await PutJson("", item);
        }

        public async Task<HttpResponseMessage> UpdateOrder(StockTransferOrderModel item)
        {
            return await PutJson("update", item);
        }

    }
}
    