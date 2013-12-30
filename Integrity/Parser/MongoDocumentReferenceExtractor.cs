using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MRI.Core.Message;
using MRI.MongoException;

namespace MRI.Integrity.Parser
{
    class MongoDocumentReferenceExtractor
    {
        private const string ReferenceNodeName = "_ref";

        public static List<MongoReference> Parse( MongoInsertMessage message )
        {
            var result = new List<MongoReference>();
            foreach ( var document in message.Documents )
            {
                result = result.Concat( Parse( document ) ).ToList();
            }
            return result;
        }

        private static List<MongoReference> ParseReferences( IEnumerable<BsonValue> value )
        {
            var result = new List<MongoReference>();

            foreach ( var item in value )
            {
                result.Add( ParseReference( item.AsBsonDocument ) );
            }

            return result;
        }

        private static MongoReference ParseReference( BsonDocument document )
        {
            try
            {
                return new MongoReference
                {
                    DatabaseName = document[ "db" ].AsString,
                    CollectionName = document[ "collection" ].AsString,
                    Reference = document[ "id" ]
                };
            }
            catch ( Exception )
            {
                throw new WrongReferenceFormatException();
            }
        }

        public static IEnumerable<MongoReference> Parse( IEnumerable<BsonElement> document )
        {
            var result = new List<MongoReference>();

            foreach ( var item in document )
            {
                if ( ReferenceNodeName.Equals( item.Name ) )
                {
                    var refs = ( item.Value.IsBsonArray )
                        ? ParseReferences( item.Value.AsBsonArray )
                        : new List<MongoReference> { ParseReference( item.Value.AsBsonDocument ) };

                    result = result.Concat( refs ).ToList();
                }
                else if ( item.Value.IsBsonArray || item.Value.IsBsonDocument )
                {
                    result = result.Concat( Parse( item.Value.AsBsonDocument ) ).ToList();
                }
            }

            return result;
        }
    }
}
