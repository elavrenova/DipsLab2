﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;

namespace DipsLab2.Services
{
    public interface IOrderService
    {
        List<string> GetAllOrders(int page, int size);
        Task<HttpResponseMessage> AddOrder(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseOrder(StockTransferOrderModel item);
        Task<HttpResponseMessage> UpdateOrder(StockTransferOrderModel item);
    }
}
