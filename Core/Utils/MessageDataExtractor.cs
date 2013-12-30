using System;
using System.Linq;
using MongoDB.Bson;
using MRI.Core.Message;
using MRI.Message;

namespace MRI.Core.Utils
{
    public static class MessageDataExtractor
    {
        public static bool IsHasErrorRequest(MongoStandardMessage message)
        {
            if (message.GetType() == typeof (MongoQueryMessage))
            {
                var query = (MongoQueryMessage) message;

                return query.Document.Contains("getlasterror");
            }

            return false;
        }

        public static bool IsHasError(MongoStandardMessage message)
        {
            if (message.GetType() == typeof (MongoReplyMessage))
            {
                var reply = (MongoReplyMessage) message;
                var document = reply.Documents.FirstOrDefault();
                if (document != null)
                {
                    if (document.Contains("ok") && document.Contains("err") && 
                        document["ok"] == 1.0)
                    {
                        return false;    
                    }
                }
            }
            //TODO : throw suit.. ex
            throw new Exception("it is not good message");
        }
    }
}
