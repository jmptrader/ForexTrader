using ForexTrader.Cryptography.Interfaces;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Cryptography
{
    public class CryptographyModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDecrypt>().To<Decrypt>();
            Bind<IEncrypt>().To<Encrypt>();
        }
    }
}
