using System;

namespace MRI.Core.AppConfig
{
    public static class Config
    {
        public static int Threads
        {
            get
            {
                return Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings[ "Threads" ] );
            }
        }        
        
        public static int ClientPort
        {
            get
            {
                return Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings[ "ClientPort" ] );
            }
        }
        
        public static int ServerPort
        {
            get
            {
                return Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings[ "ServerPort" ] );
            }
        }

        public static string StorageCollectionName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[ "StorageCollectionName" ];
            }
        }

        public static string StorageConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[ "StorageConnectionString" ];
            }
        }

        public static string StorageName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[ "StorageName" ];
            }
        }

        public static string ServerConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ServerConnectionString"];
            }
        }
    }
}
