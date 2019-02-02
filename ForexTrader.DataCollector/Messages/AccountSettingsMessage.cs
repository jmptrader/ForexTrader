using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector.Messages
{
    public class AccountSettingsMessage
    {
        public AccountSettingsMessage(string apiKey, string accountId)
        {
            ApiKey = apiKey;
            AccountId = accountId;
        }

        public string ApiKey { get; set; }
        public string AccountId { get; set; }
    }
}
