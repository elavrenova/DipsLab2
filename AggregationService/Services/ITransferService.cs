﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AggregationService.Models;

namespace AggregationService.Services
{
    public interface ITransferService
    {
        Task<List<string>> GetAllTransfers(int page, int size);
        Task<HttpResponseMessage> BookTransfer(StockTransferOrderModel item);
        Task<HttpResponseMessage> FindTransfer(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseTransfer(StockTransferOrderModel item);
    }
}