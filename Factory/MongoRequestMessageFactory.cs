using System;
using System.Collections.Generic;
using System.IO;
using MRI.Core.Message;
using MRI.Message;

namespace MRI.Factory
{
    class MongoRequestMessageFactory
    {
        private readonly static IDictionary<MongoOpCode, Type> TypeByCode = new Dictionary<MongoOpCode, Type>
        {
            { MongoOpCode.OP_REPLY, typeof(MongoReplyMessage) },
            { MongoOpCode.OP_QUERY, typeof(MongoQueryMessage) },
            { MongoOpCode.OP_MSG, typeof(MongoMsgMessage) },
            { MongoOpCode.OP_UPDATE, typeof(MongoUpdateMessage) },
            { MongoOpCode.OP_INSERT, typeof(MongoInsertMessage) },
            { MongoOpCode.OP_DELETE, typeof(MongoDeleteMessage) },

        };

        public static MongoStandardMessage GetRequest(BinaryReader reader, byte[] rawRequest)
        {
            var messageLength = reader.ReadInt32();
            var requestId = reader.ReadInt32();
            var responseTo = reader.ReadInt32();
            
            var opCode = ( MongoOpCode )reader.ReadInt32();
            var messageType = TypeByCode.ContainsKey( opCode ) ? TypeByCode[ opCode ] : typeof( MongoStandardMessage );
            var message = ( MongoStandardMessage )Activator.CreateInstance( messageType, opCode, rawRequest );
            
            message.MessageLength = messageLength;
            message.RequestId = requestId;
            message.ResponseTo = responseTo;
            message.ReadData( reader );

            return message;
        }
    }
}
