using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatisticServer.Models;
using StatisticServer.Entities;

namespace StatisticServer.Controllers
{
    [Produces("application/json")]
    public class MainController : Controller
    {
        private ApplicationDbContext dbContext;
        public MainController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        

        [HttpGet("requests")]
        public async Task<List<RequestModel>> GetRequests(int forSeconds = 60)
        {
            var requests = dbContext.Requests.Where(i => i.RequestType == Events.RequestType.Gateway).ToList();
            var minDate = DateTime.Now - TimeSpan.FromSeconds(forSeconds);
            var maxDate = DateTime.Now;
            int numOfIntervals = 10;
            var span = (maxDate - minDate) / numOfIntervals;
            var intervaledRequests = new Dictionary<DateTime, List<RequestInfo>>();
            for (int i = 0; i < numOfIntervals; i++)
            {
                intervaledRequests.Add(minDate + 0.5 * span + span * i, new List<RequestInfo> { new RequestInfo { Time = minDate + 0.5 * span + span * i, From = null } });
            }
            foreach (var request in requests)
            {
                foreach (var key in intervaledRequests.Keys)
                    if ((key - request.Time).Duration() < (0.5 * span))
                    {
                        intervaledRequests[key].Add(request);
                        break;
                    }
            }
            var test = intervaledRequests.Values.Select(v => v.Count()).ToList();
            var diffconst = 0.5 * (forSeconds / numOfIntervals);
            return intervaledRequests
                .SelectMany(kv => kv.Value.GroupBy(i => i.From).Select(g => (kv.Key, g.Key, g.ToList())))
                .Select(t => new RequestModel
                {
                    Count = t.Item2 == null ? 0 : t.Item3.Count,
                    From = t.Item2,
                    Time = $"{((t.Item1 - maxDate).Seconds - diffconst).ToString()}s - {((t.Item1 - maxDate).Seconds + diffconst).ToString()}s"
                }).ToList();
        }

        [HttpGet("requests/detail")]
        public async Task<List<RequestDetailModel>> GetRequestsDetailed()
        {
            return dbContext.Requests.Select(i => new RequestDetailModel { From = i.From, Time = i.Time, To = i.To }).ToList();
        }

        [HttpGet("operations/detail")]
        public async Task<List<OperationDetailModel>> GetOperationsDetailed()
        {
            return dbContext.UserOperations.Select(i => new OperationDetailModel { Operation = Enum.GetName(typeof(Operation), i.Operation), Username = i.Username, Time = i.Time }).ToList();
        }

        [HttpGet("ordersadditions")]
        public async Task<List<OrderAdditionModel>> GetOrdersAdditions()
        {
            return dbContext.OrderAdditions
                .Select(nai => nai.Author)
                .GroupBy(s => s)
                .Select(g => new OrderAdditionModel { Author = g.Key, Count = g.Count() })
                .ToList();
        }

        [HttpGet("ordersadditions/detail")]
        public async Task<List<OrderAdditionDetailModel>> GetOrdersAdditionsDetailed()
        {
            return dbContext.OrderAdditions.Select(i => new OrderAdditionDetailModel
            {
                Author = i.Author,
                Time = i.AddedTime.ToString(@"ddMM-HH:mm")
            }).ToList();
        }
    }
}