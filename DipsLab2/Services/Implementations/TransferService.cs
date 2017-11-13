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

        public List<string> GetAllTransfers(int page, int size)
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

        public async Task<HttpResponseMessage> BookTransfer(StockTransferOrderModel item) =>
            await PutJson("bookt", item);

        public async Task<HttpResponseMessage> RefuseTransfer(StockTransferOrderModel item) =>
            await PutJson("refuset", item);

    }
}
