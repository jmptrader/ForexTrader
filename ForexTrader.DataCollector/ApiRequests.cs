using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ForexTrader.DataCollector
{
    public class ApiRequests
    {
        private readonly string _key = "bef06b9e0a53a0ca13149ebcecb3f9fc-606b51db4feaa79f58827dcb4d50d5e7";
        private readonly string _accountId = "101-004-9917812-001";
        private readonly string _instruments = "GBP_AUD%2CGBP_CAD%2CGBP_CHF%2CGBP_JPY%2CGBP_NZD%2CGBP_PLN%2CGBP_SGD%2CGBP_USD%2CGBP_ZAR%2CEUR_GBP";
        private HttpClient _httpClient = new HttpClient();

        public ApiRequests(string key, string accountId)
        {
            _key = key;
            _accountId = accountId;
        }

        public async Task<HttpResponseMessage> GetExchangeRates()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api-fxpractice.oanda.com/v3/accounts/" + _accountId + "/pricing?instruments=" + _instruments),
                Method = HttpMethod.Get
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _key);
            return await _httpClient.SendAsync(request);
        }
        
    }
}
