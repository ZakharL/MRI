using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MRI.Core.AppConfig;
using MRI.Core.MongoException;
using MRI.Integrity;
using MRI.Integrity.Parser;

namespace MRI.Core.Storage.Mongo
{
    class MongoStorage : IStorage
    {
        MongoDatabase MongoDatabase { get; set; }

        private const string IdField = "_id";
        private const string CountField = "count";

        public MongoStorage()
        {
            var client = new MongoClient( Config.StorageConnectionString );
            var server = client.GetServer();
            MongoDatabase = server.GetDatabase( Config.StorageCollectionName );
        }

        private static string GetCollectionName( string db, string collection )
        {
            return db + "_" + collection;
        }

        private MongoCollection GetCollection( MongoReference reference )
        {
            return MongoDatabase.GetCollection( GetCollectionName( reference.DatabaseName, reference.CollectionName ) );
        }

        public void Link( MongoReference reference, int count = 1 )
        {
            if (count == 0)
            {
                return;
            }

            var collection = GetCollection( reference );

            var result = collection.FindAndModify( new FindAndModifyArgs
            {
                Query = Query.And( Query.EQ( IdField, reference.Reference ) ),
                Update = Update.Inc( CountField, count ),
                Upsert = true
            } );

            if ( !result.Ok )
            {
                throw new StorageRequestException();
            }
        }

        public void Link(ReferencePack change)
        {
            Link(change.MongoReference, change.Count);
        }

        public void Unlink( MongoReference reference, int count = 1 )
        {
            if (count == 0)
            {
                return;
            }

            var collection = GetCollection( reference );

            var result = collection.FindAndModify( new FindAndModifyArgs
            {
                Query = Query.And( Query.EQ( IdField, reference.Reference ) ),
                Update = Update.Inc( CountField, -count ),
                Upsert = true
            } );

            if ( !result.Ok )
            {
                //TODO : log last error
                throw new StorageRequestException();
            }

            var x = result.ModifiedDocument;

            if ( x == null || !x.Contains( CountField ) || x[ CountField ] < 0 )
            {
                throw new ReferenceOutOfRangeException();
            }
        }

        public void Unlink(ReferencePack change)
        {
            Unlink(change.MongoReference, change.Count);
        }

        public int Count( MongoReference reference )
        {
            var document = GetCollection( reference ).FindOneByIdAs<BsonDocument>( reference.Reference );

            return document == null ? 0 : Convert.ToInt32( document[ CountField ] );
        }
    }
}