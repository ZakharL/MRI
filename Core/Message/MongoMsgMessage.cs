using MRI.Message;

namespace MRI.Core.Message
{
    class MongoMsgMessage : MongoStandardMessage
    {
        public MongoMsgMessage(MongoOpCode opCode, byte[] rawRequest) : base(opCode, rawRequest)
        {
        }
    }
}
