using MRI.Integrity.Parser;
using MRI.Message;

namespace MRI.Integrity
{
    class ReferenceIntegrity
    {
        public static void Handle(MongoInsertMessage request)
        {
            if ( request.GetType() == typeof( MongoInsertMessage ) )
            {
                HandleInsertRequest(request);
            }            
        }

        private static void HandleInsertRequest(MongoInsertMessage request)
        {
            var references = MongoQueryParser.Parse( request );
        }
    }
}
