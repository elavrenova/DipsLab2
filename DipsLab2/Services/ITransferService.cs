using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DipsLab2.Services
{
    public interface ITransferService
    {
        Task<List<string>> GetAllTransfers(int page, int perpage);
        Task<HttpResponseMessage> BookTransfer(string transfer);
        Task<HttpResponseMessage> RefuseTransfer(string transfer);
    }
}
