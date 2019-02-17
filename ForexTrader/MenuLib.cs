using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Linq;
using ForexTrader.Cryptography;
using ForexTrader.DataCollector.Messages;
using ForexTrader.Interfaces;
using ForexTrader.Cryptography.Interfaces;
using ForexTrader.Logging.Interfaces;

namespace ForexTrader
{
    public class MenuLib : IMenuLib
    {
        private readonly string _unixLoc = @"/opt/ForexTrader/";
        private readonly string _winLoc = @"Program Files\ForexTrader\";
        private static ILogger _logger;
        private string _dirPath;
        private IDecrypt _decrypt;
        private IEncrypt _encrypt;

        public MenuLib(ILogger logger)
        {
            _logger = logger;
            _encrypt = new Encrypt(_logger);
            _decrypt = new Decrypt(_logger);
        }

        public AccountSettingsMessage SetAccountSettings()
        {
            string apiKey = string.Empty;
            string accountId = string.Empty;

            while (true)
            {
                Console.WriteLine("Input account details here. Letter case and spacing needs to be accurate.");
                Console.Write("Api Key (Empty for no change): ");
                var apiInput = Console.ReadLine();
                if (apiInput != string.Empty)
                {
                    apiKey = apiInput;
                    _logger.AddLogEntry("User updated API key.");
                }

                Console.Write("Account ID (Empty for no change): ");
                var accountInput = Console.ReadLine();
                if (accountInput != string.Empty)
                {
                    accountId = accountInput;
                    _logger.AddLogEntry("User updated Account ID.");
                }

                if ((accountId != string.Empty || accountId != null) && (apiKey != string.Empty || apiKey != null))
                {
                    SaveSettings(apiKey, accountId);
                    Console.Clear();
                    return new AccountSettingsMessage(apiKey, accountId); 
                }

                Console.WriteLine("Not all required details are supplied");
            }
        }

        public bool CheckFirstTime(bool isUnix)
        {
            if (isUnix)
            {
                _dirPath = Path.Join(Path.GetPathRoot(Environment.SystemDirectory), _unixLoc);
            }
            else
            {
                _dirPath = Path.Join(Path.GetPathRoot(Environment.SystemDirectory), _winLoc);
            }
            try
            {
                var files = Directory.GetFiles(_dirPath);
                if (files.Contains(_dirPath + "Settings.txt"))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.AddLogEntry("Failed to search for settings.txt, Error: " + e);
                Console.WriteLine("Exiting");
                Console.ReadLine();
                Environment.Exit(0);
            }
            return true;
        }

        public AccountSettingsMessage LoadSettings()
        {
            string password = null;
            while (true)
            {
                password = InputPassword();

                if (password != null || password != string.Empty)
                {
                    break;
                }

                _logger.AddLogEntry("User inserted invalid password upon loading settings.");
                Console.WriteLine("We need a valid password.");
            }

            string encryptedSettings;
            using (var sr = new StreamReader(Path.Join(_dirPath, "Settings.txt")))
            {
                encryptedSettings = sr.ReadLine();
            }

            if (encryptedSettings == string.Empty || encryptedSettings == null)
            {
                Console.WriteLine("Failed to retrieve settings.");
                _logger.AddLogEntry("Failed to get settings from settings file. Nothing was returned.");
                return null;
            }

            var settings = _decrypt.AesDecrypt(encryptedSettings, password);
            var settingsSplit = settings.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (settingsSplit.Length != 2)
            {
                _logger.AddLogEntry($"Settings file loaded incorrect number of settings values {settingsSplit.Length}");
                return null;
            }

            var apiKey = settingsSplit[0];
            var accountId = settingsSplit[1];

            return new AccountSettingsMessage(apiKey, accountId);
        }

        public void SaveSettings(string apiKey, string accountId)
        {
            string password = null;
            while (true)
            {
                password = InputPassword();

                if (password != null || password != string.Empty)
                {
                    break;
                }

                _logger.AddLogEntry("User inserted invalid password upon saving settings.");
                Console.WriteLine("We need a valid password.");
            }

            var encryptedSettings = _encrypt.AesEncrypt(apiKey + ":" + accountId, password);
            using (var sw = new StreamWriter(Path.Join(_dirPath, "Settings.txt"), false))
            {
                sw.WriteLine(encryptedSettings);
            }

            _logger.AddLogEntry("Saved settings to settings file.");
        }

        public string InputPassword()
        {
            string pass = null;
            Console.Write("Input password: ");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                pass += key.KeyChar;
            }

            return pass;
        }
    }
}
