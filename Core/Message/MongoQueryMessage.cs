using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MRI.Helpers;
using MRI.Message;

namespace MRI.Core.Message
{
    class MongoQueryMessage : MongoStandardMessage
    {
        public int Flags { get; set; }
        public string FullCollectionName { get; set; }
        public int NumberToSkip { get; set; }
        public int NumberToReturn { get; set; }

        public BsonDocument Document { get; set; }
        public BsonDocument ReturnFieldsSelector { get; set; }

        public MongoQueryMessage(MongoOpCode opCode, byte[] rawRequest): base(opCode, rawRequest)
        {
        }

        public override bool WaitForResponse()
        {
            return true;
        }

        public override void ReadData(BinaryReader reader)
        {
            Flags = reader.ReadInt32();
            FullCollectionName = Util.ReadString(reader);
            NumberToSkip = reader.ReadInt32();
            NumberToReturn = reader.ReadInt32();
            Document = BsonSerializer.Deserialize<BsonDocument>(reader.BaseStream);
            
            if (reader.PeekChar() != -1)
            {
                ReturnFieldsSelector = BsonSerializer.Deserialize<BsonDocument>(reader.BaseStream);    
            }
        }
    }
}
