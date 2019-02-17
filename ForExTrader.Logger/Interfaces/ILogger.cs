﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Logging.Interfaces
{
    public interface ILogger
    {
        void AddLogEntry(object value);
        void QueueChecker();
    }
}
