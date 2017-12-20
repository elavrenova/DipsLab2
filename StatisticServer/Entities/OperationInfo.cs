using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Entities
{
    public class OperationInfo
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Operation Operation { get; set; }
        public DateTime Time { get; set; }
    }
    public enum Operation
    {
        AddedOrder,
        DeletedOrder,
        AllInformation,
        StocksInfo,
        TransfersInfo
    }
}
