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
            if (!x.Reference.Equals(y.Reference))
            {
                return false;
            }
            if(!x.CollectionName.Equals(y.CollectionName))
            {
                return false;
            }
            if(!x.DatabaseName.Equals(y.DatabaseName))
            {
                return  false;
            }

            return true;
        }

        public static bool operator != (MongoReference x, MongoReference y)
        {
            if ( x.Reference.Equals( y.Reference ) && x.CollectionName.Equals( y.CollectionName ) && x.DatabaseName.Equals( y.DatabaseName ) )
            {
                return false;
            }

            return true;
        }
    }
}
