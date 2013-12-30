using System;
using System.Data;
using MRI.Core.AppConfig;
using MRI.Core.Storage.Mongo;

namespace MRI.Core.Storage
{
    static public class StorageFactory
    {
        public static IStorage GetStorage()
        {
            var storage = (StorageType) Enum.Parse(typeof(StorageType), Config.StorageName);

            switch ( storage )
            {
                case StorageType.Mongodb:
                    return new MongoStorage();

                default:
                    throw new DataException();
            }
        }
    }
}
