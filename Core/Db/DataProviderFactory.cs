namespace MRI.Core.Db
{
    public class DataProviderFactory
    {
        public static MongoDataProvider GetDataProvider()
        {
            return MongoDataProvider.Instance;
        }
    }
}
