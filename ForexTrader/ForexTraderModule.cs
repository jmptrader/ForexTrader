using ForexTrader.Interfaces;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader
{
    public class ForexTraderModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMenu>().To<Menu>().InSingletonScope();
            Bind<IMenuLib>().To<MenuLib>().InSingletonScope();
        }
    }
}
