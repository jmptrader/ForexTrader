using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Logging.Interfaces
{
    public interface IArchitectureExplorer
    {
        bool IsUnix();

        string ArchRootPath(string target = null);
    }
}
