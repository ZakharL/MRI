using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MRI.Core.AppConfig;
using MRI.Helpers;
using MRI.Integrity;
using MRI.Integrity.Parser;

namespace MRI.Core.Db
{
    public class MongoDataProvider
    {
        private static volatile MongoDataProvider _instance;
        private static readonly object Sync = new Object();

        private MongoServer MongoServer { get; set; }
        
        private MongoDataProvider()
        {
            var client = new MongoClient(Config.ServerConnectionString);
            MongoServer = client.GetServer();
        }

        public static MongoDataProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            _instance = new MongoDataProvider();
                    }
                }
                return _instance;
            }
        }

        private MongoCollection GetCollection(string fullCollectionName)
        {
            return GetCollection(Util.ParseDbName(fullCollectionName), Util.ParseCollectionName(fullCollectionName));
        }

        private MongoCollection GetCollection(string db, string collection)
        {
            var mongoDatabase = MongoServer.GetDatabase(db);
            return mongoDatabase.GetCollection<BsonDocument>(collection);
        }

        public List<ReferencePack> Read(string fullCollectionName, BsonDocument query)
        {
            var collection = GetCollection(fullCollectionName);
            var mongoQuery = new QueryDocument(query);

            var packer = new ReferencePacker();

            foreach ( var doc in collection.FindAs<BsonDocument>(mongoQuery) )
            {
                packer.Add( MongoDocumentReferenceExtractor.Parse(doc) );
            }

            return packer.Get();
        }

        public bool Exist(string db, string collection, BsonValue id)
        {
            var mongoCollection = GetCollection(db, collection);
            var mongoQuery = Query.EQ("_id", id );
                
            var document = mongoCollection.FindOneAs<BsonDocument>(mongoQuery);

            return (document != null);
        }
    }
}
