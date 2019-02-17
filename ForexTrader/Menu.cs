using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using ForexTrader.Logging;
using ForexTrader.DataCollector.Messages;
using ForexTrader.Interfaces;
using ForexTrader.Logging.Interfaces;
using ForexTrader.DataCollector.Interfaces;

namespace ForexTrader
{
    public class Menu : IMenu
    {
        private readonly ILogger _logger;
        private readonly ICollector _collector;
        private IMenuLib _menuLib;
        private IArchitectureExplorer _architectureExplorer;

        public bool RetrievedAccSettings { get; set; }

        public Menu(ILogger logger, ICollector collector)
        {
            RetrievedAccSettings = false;
            _logger = logger;
            _collector = collector;
            _menuLib = new MenuLib(_logger);
            _architectureExplorer = new ArchitectureExplorer(_logger);
        }

        public void StartMenu()
        {
            Console.WriteLine("Checking if first time run...");
            
            // Check first time run
            if (!_menuLib.CheckFirstTime(_architectureExplorer.IsUnix()))
            {
                Console.WriteLine("Welcome back.");
                var settings = _menuLib.LoadSettings();
                if (settings == null)
                {
                    _collector.AddToMessageQueue(_menuLib.SetAccountSettings());
                }
                _collector.AddToMessageQueue(settings);
                RetrievedAccSettings = true;
                BaseMenu();
            }
            else
            {
                var settings = _menuLib.SetAccountSettings();
                {
                    _collector.AddToMessageQueue(settings);
                }

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
                Console.WriteLine(
                    "1. Set Collector Frequency.\n" +
                    "2. Set Account Details.\n" +
                    "3. Exit Program.");
                var basemenuInput = Console.ReadKey();
                int.TryParse(basemenuInput.KeyChar.ToString(), out var intInput);
                switch(intInput)
                {
                    case 1:
                        Console.WriteLine("Setting collector frequency");
                        break;
                    case 2:
                        _collector.AddToMessageQueue(_menuLib.SetAccountSettings());
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
