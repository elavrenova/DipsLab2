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
        Task<List<string>> GetAllTransfers(int page, int perpage);
        Task<HttpResponseMessage> BookTransfer(TransferModel transfer);
        Task<HttpResponseMessage> RefuseTransfer(TransferModel transfer);
    }
}
