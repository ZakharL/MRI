using MongoDB.Bson;

namespace MRI.Integrity.Parser
{
    public class MongoReference
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public BsonValue Reference { get; set; }

        public static bool operator == (MongoReference x, MongoReference y)
        {
            if ( x == y )
            {
                return true;
            }
            if ( x == null || y == null )
            {
                return false;
            }
            return x.Reference == y.Reference 
                && x.CollectionName ==  y.CollectionName
                && x.DatabaseName == y.DatabaseName;
        }

        public static bool operator != (MongoReference x, MongoReference y)
        {
            return !( x == y );
        }
    }
}
