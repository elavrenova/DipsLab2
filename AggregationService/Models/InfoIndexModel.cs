using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregationService.Models
{
    public class InfoIndexModel
    {
        public List<Tuple<string, string, string>> Stocks { get; set; } = new List<Tuple<string, string, string>>();
        public List<Tuple<string, string, string>> Transfers { get; set; } = new List<Tuple<string, string, string>>();
        public int Page { get; set; }
        public int Size { get; set; }
        public int MaxPage { get; set; }
    }
}
