using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace ForEXTrader
{
    public class ApiRequest
    {
        private readonly string key = "bef06b9e0a53a0ca13149ebcecb3f9fc-606b51db4feaa79f58827dcb4d50d5e7";
        private HttpClient _httpClient;

        public async Task<HttpResponseMessage> GetExchangeRates()
        {
            return await _httpClient.GetAsync("https://api-fxpractice.oanda.com/v3/accounts/" + key + "/pricing");
        }
        
    }
}
