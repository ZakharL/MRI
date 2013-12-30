using MRI.Message;

namespace MRI.Core.Message
{
    class MongoKillCursorMessage : MongoStandardMessage
    {
        public MongoKillCursorMessage(MongoOpCode opCode, byte[] rawRequest): base(opCode, rawRequest)
        {
        }
    }
}
