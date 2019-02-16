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

namespace ForexTrader
{
    public class Menu : IMenu
    {
        private static ConcurrentQueue<object> _loggerQueue;
        private MenuLib _menuLib;

        public ConcurrentQueue<AccountSettingsMessage> _accountSettingsQueue;

        public bool RetrievedAccSettings { get; set; }

        public Menu(ConcurrentQueue<object> loggerQueue, ConcurrentQueue<AccountSettingsMessage> accountSettingsQueue)
        {
            RetrievedAccSettings = false;
            _loggerQueue = loggerQueue;
            _menuLib = new MenuLib(_loggerQueue);
            _accountSettingsQueue = accountSettingsQueue;
        }

        public void StartMenu()
        {
            Console.WriteLine("Checking if first time run...");
            
            // Check first time run
            if (!_menuLib.CheckFirstTime(ArchitectureExplorer.IsUnix()))
            {
                Console.WriteLine("Welcome back.");
                var settings = _menuLib.LoadSettings();
                if (settings == null)
                {
                    _accountSettingsQueue.Enqueue(_menuLib.SetAccountSettings());
                }
                _accountSettingsQueue.Enqueue(settings);
                RetrievedAccSettings = true;
                BaseMenu();
            }
            else
            {
                var settings = _menuLib.SetAccountSettings();
                {
                    _accountSettingsQueue.Enqueue(settings);
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
                        _accountSettingsQueue.Enqueue(_menuLib.SetAccountSettings());
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
