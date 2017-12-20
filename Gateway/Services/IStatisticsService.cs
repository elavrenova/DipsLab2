using StatisticServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Services
{
    public interface IStatisticsService
    {
        Task<List<RequestModel>> GetRequests();
        Task<List<RequestDetailModel>> GetRequestsDetailed();
        Task<List<OperationDetailModel>> GetOperationsDetailed();
        Task<List<OrderAdditionModel>> GetOrdersAdditions();
        Task<List<OrderAdditionDetailModel>> GetOrdersAdditionsDetailed();
    }
}
