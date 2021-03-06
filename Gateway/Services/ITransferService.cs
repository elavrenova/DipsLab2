﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;

namespace Gateway.Services
{
    public interface ITransferService
    {
        Task<List<string>> GetAllTransfers(int page, int size);
        Task<List<TransferModel>> GetTransfers();

        Task<HttpResponseMessage> BookTransfer(StockTransferOrderModel item);
        Task<HttpResponseMessage> FindTransfer(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseTransfer(StockTransferOrderModel item);
    }
}
