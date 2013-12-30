using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MRI.Helpers;

namespace MRI.Core.Message
{
    class MongoInsertMessage : MongoStandardMessage
    {
        public int Flags { get; set; }
        public string FullCollectionName { get; set; }
        public List<BsonDocument> Documents { get; set; }

        public MongoInsertMessage(MongoOpCode opCode, byte[] rawRequest): base(opCode, rawRequest)
        {
            Documents = new List<BsonDocument>();
        }

        public override bool IsBlockingMessage()
        {
            return true;
        }

        public override void ReadData(BinaryReader reader)
        {
            Flags = reader.ReadInt32();
            FullCollectionName = Util.ReadString(reader);

            while (reader.PeekChar() != -1)
            {
                Documents.Add(BsonSerializer.Deserialize<BsonDocument>(reader.BaseStream));
            }
        }
    }
}
