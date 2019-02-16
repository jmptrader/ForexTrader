using ForexTrader.DataCollector.Interfaces;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.DataCollector
{
    public class DataCollectorModule<T> : NinjectModule
    {
        public override void Load()
        {
            Bind<ICollector>().To<Collector>().InSingletonScope();
            Bind<ILimitedQueue<T>>().To<LimitedQueue<T>>();
        }
    }
}
