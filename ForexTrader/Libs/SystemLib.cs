using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ForEXTrader
{
    public static class SystemLib
    {
        public static bool IsUnix()
        {
            int platform = (int)Environment.OSVersion.Platform;
            if (platform == 5)
            {
                Environment.Exit(1);
            }
            return (platform == 4) || (platform == 6);
        }

        public static string ArchRootPath(string target = null)
        {
            if(target == null || target == string.Empty)
            {
                return Path.GetPathRoot(Environment.SystemDirectory);
            }
            return Path.Join(Path.GetPathRoot(Environment.SystemDirectory), target);
        }
    }
}
