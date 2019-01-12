using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using ForexTrader.Logging;

namespace ForexTrader
{
    public class Menu
    {
        private static ConcurrentQueue<object> _loggerQueue;
        private MenuLib _menuLib;

        public string ApiKey { get; set; }

        public string AccountId { get; set; }

        public bool RetrievedAccSettings { get; set; }

        public Menu(ConcurrentQueue<object> loggerQueue)
        {
            RetrievedAccSettings = false;
            _loggerQueue = loggerQueue;
            _menuLib = new MenuLib(_loggerQueue);
        }

        public void StartMenu()
        {
            Console.WriteLine("Checking if first time run...");
            
            // Check first time run
            if (!_menuLib.CheckFirstTime(ArchitectureExplorer.IsUnix()))
            {
                Console.WriteLine("Welcome back.");
                var loadedSettings = _menuLib.LoadSettings();
                ApiKey = loadedSettings.Item1;
                AccountId = loadedSettings.Item2;
                RetrievedAccSettings = true;
                BaseMenu();
            }
            else
            {
                var accountSettings = _menuLib.SetAccountSettings();
                ApiKey = accountSettings.Item1;
                AccountId = accountSettings.Item2;
                RetrievedAccSettings = true;
            }
            BaseMenu();
        }        
        
        private void BaseMenu()
        {
            Console.Clear();
            Console.WriteLine("=== FOREX TRADER ===\n");
            var exit = false;
            while (exit == false)
            {
                Console.WriteLine("1. Set Collector Frequency.\n" +
                    "2. Set Account Details.\n" +
                    "3. Exit Program.");
                var basemenuInput = Console.ReadLine();
                int.TryParse(basemenuInput, out var intInput);
                switch(intInput)
                {
                    case 1:
                        Console.WriteLine("Setting collector frequency");
                        
                        break;
                    case 2:
                        _menuLib.SetAccountSettings();
                        break;
                    case 3:
                        exit = true;
                        break;
                }
            }

            ExitProgram(0);
        }

        private void ExitProgram(int exitCode)
        {
            Environment.Exit(exitCode);
        }
    }
}
