using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MRI.Helpers;
using MRI.Message;

namespace MRI.Core.Message
{
    class MongoUpdateMessage : MongoStandardMessage
    {
        public int Zero { get; set; }
        public string FullCollectionName { get; set; }
        public int Flags { get; set; }
        public BsonDocument Selector { get; set; }
        public BsonDocument Update { get; set; }

        public MongoUpdateMessage(MongoOpCode opCode, byte[] rawRequest): base(opCode, rawRequest)
        {
        }

        public override bool IsBlockingMessage()
        {
            return true;
        }

        public override void ReadData(BinaryReader reader)
        {
            Zero = reader.ReadInt32();
            FullCollectionName = Util.ReadString(reader);
            Flags = reader.ReadInt32();
            Selector = BsonSerializer.Deserialize<BsonDocument>(reader.BaseStream);
            Update = BsonSerializer.Deserialize<BsonDocument>(reader.BaseStream);
        }
    }
}
