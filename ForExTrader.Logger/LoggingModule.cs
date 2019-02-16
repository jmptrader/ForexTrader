using ForexTrader.Logging.Interfaces;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Logging
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
        }
    }
}
