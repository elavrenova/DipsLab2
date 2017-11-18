using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gateway.Services
{
    public class Service
    {
        private string baseAddress;

        public Service(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }
        protected async Task<HttpResponseMessage> PostJson(string addr, object obj)
        {
            using (var client = new HttpClient())
                try
                {
                    return await client.PostAsync(GetAddress(addr), new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));
                }
                catch
                {
                    return null;
                }
        }
        protected async Task<HttpResponseMessage> PutJson(string addr, object obj)
        {
            using (var client = new HttpClient())
                try
                {
                    return await client.PutAsync(GetAddress(addr), new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));
                }
                catch
                {
                    return null;
                }
        }

        protected async Task<HttpResponseMessage> Get(string addr)
        {
            using (var client = new HttpClient())
                try
                {
                    return await client.GetAsync(GetAddress(addr));
                }
                catch
                {
                    return null;
                }
        }

        protected async Task<HttpResponseMessage> Delete(string addr)
        {
            using (var client = new HttpClient())
                try
                {
                    return await client.DeleteAsync(GetAddress(addr));
                }
                catch
                {
                    return null;
                }
        }

        private string GetAddress(string addr)
        {
            return $"{baseAddress}/{addr}";
        }
    }
}
