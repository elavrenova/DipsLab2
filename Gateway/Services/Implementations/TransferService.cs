using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gateway.Services.Implementations
{
    public class TransferService : Service, ITransferService
    {
        public TransferService(IConfiguration configuration) : 
            base(configuration.GetSection("Urls")["Transf"]) { }

        public async Task<List<string>> GetAllTransfers(int page, int size)
        {
            var res = await Get($"?page={page}&size={size}");
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

        public async Task<HttpResponseMessage> BookTransfer(StockTransferOrderModel item) =>
            await PutJson("book_transfer", item);
        public async Task<HttpResponseMessage> FindTransfer(StockTransferOrderModel item) =>
            await PutJson("find_transfer", item);

        public async Task<HttpResponseMessage> RefuseTransfer(StockTransferOrderModel item) =>
            await PutJson("refuse_transfer", item);

    }
}
