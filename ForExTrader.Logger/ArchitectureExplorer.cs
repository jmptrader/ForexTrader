using ForexTrader.Logging.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ForexTrader.Logging
{
    public class ArchitectureExplorer : IArchitectureExplorer
    {
        private ILogger _logger;

        public ArchitectureExplorer(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsUnix()
        {
            int platform = (int)Environment.OSVersion.Platform;
            if (platform == 5)
            {
                _logger.AddLogEntry("Operating system found to be unable to run this program.");
                Environment.Exit(1);
            }

            if (platform == 4 || platform == 6)
            {
                _logger.AddLogEntry("Operating system found to be Unix based.");
                return true;
            }

            _logger.AddLogEntry("Operating system found to be Windows based.");
            return false;
        }

        public string ArchRootPath(string target = null)
        {
            if (target == null || target == string.Empty)
            {
                return Path.GetPathRoot(Environment.SystemDirectory);
            }

            return Path.Join(Path.GetPathRoot(Environment.SystemDirectory), target);
        }
    }
}
