using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StatisticServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Services.Implementations
{
    public class StatisticsService : Service, IStatisticsService
    {
        public StatisticsService(IConfiguration configuration) : base(configuration.GetSection("ServiceUrl")["Stat"]) { }

        public async Task<List<OperationDetailModel>> GetOperationsDetailed()
        {
            var res = await Get("operations/detail");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<OperationDetailModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }

        public async Task<List<RequestModel>> GetRequests()
        {
            var res = await Get("requests");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<RequestModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }

        public async Task<List<RequestDetailModel>> GetRequestsDetailed()
        {
            var res = await Get("requests/detail");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<RequestDetailModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }

        public async Task<List<OrderAdditionModel>> GetOrdersAdditions()
        {
            var res = await Get("ordersadditions");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<OrderAdditionModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }

        public async Task<List<OrderAdditionDetailModel>> GetOrdersAdditionsDetailed()
        {
            var res = await Get("ordersadditions/detail");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<OrderAdditionDetailModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }
        public async Task<List<OrderValueModel>> GetOrderValues()
        {
            var res = await Get("ordervalues");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<OrderValueModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }

        public async Task<List<OrderValueDetailModel>> GetOrderValuesDetailed()
        {
            var res = await Get("ordervalues/detail");
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<OrderValueDetailModel>>(res.Content.ReadAsStringAsync().Result);
            return null;
        }
    }
}
