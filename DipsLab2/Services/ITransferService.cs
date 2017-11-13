using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;

namespace DipsLab2.Services
{
    public interface ITransferService
    {
        List<string> GetAllTransfers(int page, int size);
        Task<HttpResponseMessage> BookTransfer(StockTransferOrderModel item);
        Task<HttpResponseMessage> RefuseTransfer(StockTransferOrderModel item);
    }
}
