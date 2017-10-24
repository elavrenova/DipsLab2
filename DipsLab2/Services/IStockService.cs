using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DipsLab2.Services
{
    public interface IStockService
    {
        List<string> GetAllStocks(int page, int perpage);
        HttpResponseMessage BookStock(string transfer);
        HttpResponseMessage RefuseStock(string transfer);
    }
}
