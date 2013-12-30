using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MRI.Message;

namespace MRI.Core.Message
{
    class MongoReplyMessage : MongoStandardMessage
    {
        public int ResponseFlags { get; set; }
        public long CursorId { get; set; }
        public int StartingFrom { get; set; }
        public int NumberReturned { get; set; }

        public List<BsonDocument> Documents { get; set; }

        public MongoReplyMessage(MongoOpCode opCode, byte[] rawRequest) : base(opCode, rawRequest)
        {
            Documents = new List<BsonDocument>();
        }

        public override void ReadData(BinaryReader reader)
        {
            ResponseFlags = reader.ReadInt32();
            CursorId = reader.ReadInt64();
            StartingFrom = reader.ReadInt32();
            NumberReturned = reader.ReadInt32();

            for (int i = 0; i < NumberReturned; i++)
            {
                Documents.Add( BsonSerializer.Deserialize<BsonDocument>(reader.BaseStream) );
            }
        }
    }
}
