using System.IO;

namespace MRI.Core.Message
{
    public class MongoStandardMessage
    {
        public MongoOpCode OpCode { get; private set; }
        public byte[] RawRequest { get; private set; }

        public int MessageLength { get; set; }
        public int RequestId { get; set; }
        public int ResponseTo { get; set; }
        public string Request { get; set; }

        public MongoStandardMessage( MongoOpCode opCode, byte[] rawRequest )
        {
            RawRequest = rawRequest;
            OpCode = opCode;
        }

        public virtual void ReadData( BinaryReader reader )
        {
        }

        public virtual bool WaitForResponse()
        {
            return false;
        }

        public virtual bool IsBlockingMessage()
        {
            return false;
        }
    }
}
