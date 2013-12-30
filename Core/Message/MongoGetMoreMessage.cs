using MRI.Message;

namespace MRI.Core.Message
{
    class MongoGetMoreMessage : MongoStandardMessage
    {
        public MongoGetMoreMessage(MongoOpCode opCode, byte[] rawRequest): base(opCode, rawRequest)
        {
        }

        public override bool WaitForResponse()
        {
            return true;
        }
    }
}
