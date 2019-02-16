using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Cryptography.Interfaces
{
    public interface IDecrypt
    {
        string AesDecrypt(string input, string pass, byte[] iv = null);
    }
}
