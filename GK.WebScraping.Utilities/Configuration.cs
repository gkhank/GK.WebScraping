using Newtonsoft.Json;
using System;
using System.IO;

namespace GK.WebScraping.Utilities
{

    public class Configuration
    {
        public Boolean IsDevelopment { get { return Environment.MachineName != "GK-WS1"; } }

        public ServerConfig SqlServer { get; set; }
        public ServiceConfig Services { get; set; }
        public ApplicationQueuesConfig Queues { get; set; }

        #region Singleton

        protected static Configuration _instance;

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    String file = Path.Combine(ApplicationPath.ConfigDirectory, "config.json");
                    _instance = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
                }

                return _instance;
            }
        }

        #endregion


    }
    #region Config Models
    public class ServerConfig
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string PasswordEncrypted { get; set; }
    }

    public class ReaderServiceConfig
    {
        public int NumberOfThreads { get; set; }
        public int BulkSize { get; set; }
        public int IterationSleep { get; set; }
        public string OperationStartTime { get; set; }
        public string OperationStopTime { get; set; }
        public bool UseProxy{ get; set; }
    }

    public class ServiceConfig
    {
        public ReaderServiceConfig ReaderService { get; set; }
    }

    public class QueueConfig
    {
        public int NumberOfThreads { get; set; }
        public int Capacity { get; set; }
        public int TresholdReachedSleepMiliseconds { get; set; }
    }

    public class ApplicationQueuesConfig
    {
        public QueueConfig FileOperationQueue { get; set; }
        public QueueConfig DatabaseTransactionQueue { get; set; }
    }

    #endregion
}
