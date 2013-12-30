using MRI.Integrity;
using MRI.Integrity.Parser;

namespace MRI.Core.Storage
{
    public interface IStorage
    {
        void Link( MongoReference reference, int count = 1);

        void Link( ReferencePack change );

        void Unlink( MongoReference reference, int count = 1);

        void Unlink( ReferencePack change );

        int Count( MongoReference reference );
    }
}
