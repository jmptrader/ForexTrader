using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForEXTrader
{
    class Menu
    {
        bool FirstTimeRun { get; set; }

        public string Api { get; set; }

        public int Keep { get; set; }

        public void StartMenu()
        {
            Console.WriteLine("Checking if first time run...");
            FirstTimeRun = CheckFirstTime();
            if (FirstTimeRun == true)
            {
                Console.WriteLine("Welcome back.");
                LoadSettings();
                BaseMenu();
            }
            else
            {
                var validKeep = false;
                Console.WriteLine("Welcome! Please fill in the following settings to continue.\nAPI key");
                Api = Console.ReadLine();
                while (!validKeep)
                {
                    Console.WriteLine("Percentage of cash to save");
                    var percentage = Console.ReadLine();
                    if (int.TryParse(percentage, out var result))
                    {
                        validKeep = true;
                        Keep = result;
                        break;
                    }
                    Console.WriteLine("Incorrect answer type. Please insert an integer cunt.");
                }
            }
        }

        private bool CheckFirstTime()
        {
            try
            {
                if (Directory.GetFiles(Directory.GetCurrentDirectory(), "settings.txt", SearchOption.TopDirectoryOnly).FirstOrDefault() == "settings.txt")
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to search for settings.txt, Error: " + e);
            }
            return false;
        }

        private List<string> LoadSettings()
        {
            return new List<string>();
        }

        private void BaseMenu()
        {

        }
    }
}
