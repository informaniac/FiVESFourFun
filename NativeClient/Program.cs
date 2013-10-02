using System;
using NLog;
using System.Configuration;

namespace NativeClient
{
    class MainClass
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Logger.Info("Reading configuration");

            string serverURI = null;

            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try {
                serverURI = ConfigurationManager.AppSettings["ServerURI"].ToString();
            } catch (ConfigurationErrorsException e) {
                Logger.FatalException("Configuration is missing or corrupt.", e);
                return;
            }

            Logger.Info("Initiailizing client");

            ClientDriver driver = new ClientDriver(serverURI);
            driver.SimulateClient();

            Console.WriteLine("Client is running. Please any key to quit...");
            Console.ReadKey();
        }
    }
}