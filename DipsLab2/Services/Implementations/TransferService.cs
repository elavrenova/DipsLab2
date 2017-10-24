using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DipsLab2.Services.Implementations
{
    public class TransferService : Service, ITransferService
    {
        public TransferService(IConfiguration configuration) : 
            base(configuration.GetSection("Urls")["Transf"]) { }

        public async Task<List<string>> GetAllTransfers(int page, int perpage)
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

        public async Task<HttpResponseMessage> BookTransfer(TransferModel transfer) =>
            await PutJson("book_t/"+transfer.Id, transfer);

        public async Task<HttpResponseMessage> RefuseTransfer(TransferModel transfer) =>
            await PutJson("refuse_t/"+transfer.Id, transfer);

    }
}
