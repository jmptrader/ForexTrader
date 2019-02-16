using ForexTrader.DataCollector.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Interfaces
{
    public interface IMenuLib
    {
        AccountSettingsMessage SetAccountSettings();

        bool CheckFirstTime(bool isUnix);

        AccountSettingsMessage LoadSettings();

        string InputPassword();
    }
}
