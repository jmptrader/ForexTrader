using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Cryptography.Interfaces
{
    public interface IEncrypt
    {
        string AesEncrypt(string input, string pass, byte[] iv = null);
    }
}
