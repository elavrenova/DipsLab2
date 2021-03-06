﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DipsLab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections;

namespace DipsLab2.Services.Implementations
{
    public class StockService : Service, IStockService
    {
        public StockService(IConfiguration configuration) : 
            base(configuration.GetSection("Urls")["Stock"]) { }

        public async Task<List<string>> GetAllStocks(int page, int size)
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

        public async Task<List<StockModel>> GetStocks(int page, int size)
        {
            var res = await Get($"?page={page}&size={size}");
            string response = await res.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<List<StockModel>>(response);
            }
            catch
            {
                return null;
            }
        }

        //public async Task<HttpResponseMessage> GetStockById(int id, StockModel stock, double val) =>
        //    await PutJson("book_s/" + stock.Id, stock);

        public async Task<HttpResponseMessage> BookStock(StockTransferOrderModel item) =>
            await PutJson("books", item);

        public async Task<HttpResponseMessage> RefuseStock(StockTransferOrderModel item) =>
            await PutJson("refuses", item);
    }
}
