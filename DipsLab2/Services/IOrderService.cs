using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;

namespace DipsLab2.Services
{
    public interface IOrderService
    {
        Task<List<string>> GetInfoOrders(int page, int perpage);
        Task<HttpResponseMessage> AddOrder(OrderModel orderModel);
        Task<HttpResponseMessage> UpdateOrderStatus(OrderModel orderModel);
    }
}
