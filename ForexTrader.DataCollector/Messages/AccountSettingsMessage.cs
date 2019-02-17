using ForexTrader.DataCollector.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector.Messages
{
    public class AccountSettingsMessage : IMessage
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
