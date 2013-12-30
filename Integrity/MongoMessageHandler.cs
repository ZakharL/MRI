using System.Collections.Generic;
using MRI.Core.Db;
using MRI.Core.Message;
using MRI.Core.Storage;
using MRI.Integrity.Parser;

namespace MRI.Integrity
{
    class MongoMessageHandler
    {
        public static List<ReferencePack> GetChanges(MongoInsertMessage request)
        {
            var packer = new ReferencePacker();
            packer.Add(MongoDocumentReferenceExtractor.Parse(request));
            return packer.Get();
        }

        public static List<ReferencePack> GetChanges(MongoUpdateMessage request)
        {
            return DataProviderFactory.GetDataProvider().Read( request.FullCollectionName, request.Selector );
        }

        public static List<ReferencePack> GetChanges(MongoDeleteMessage request)
        {
            return DataProviderFactory.GetDataProvider().Read(request.FullCollectionName, request.Document);
        }

        public static void ApplyDeleteChanges(List<ReferencePack> changes)
        {
            var storage = StorageFactory.GetStorage();
            foreach (var change in changes)
            {
                storage.Unlink(change);    
            }
        }

        public static void ApplyUpdateChanges(List<ReferencePack> beforeUpdate, List<ReferencePack> afterUpdate)
        {
            var changes = ReferencePacker.CalculateChanges(beforeUpdate, afterUpdate);

            if (changes.Count <= 0)
            {
                return;
            }

            var storage = StorageFactory.GetStorage();
            foreach (var change in changes)
            {
                storage.Link(change);
            }
        }

        public static void ApplyInsertChanges(List<ReferencePack> changes)
        {
            var storage = StorageFactory.GetStorage();

            foreach (var change in changes)
            {
                storage.Link(change);
            }
        }

        public static bool IsInsertAllowed(List<ReferencePack> newReferences)
        {
            if (newReferences.Count == 0)
            {
                return true;
            }

            var dataProvider = DataProviderFactory.GetDataProvider();

            foreach (var reference in newReferences)
            {
                if (
                    !dataProvider.Exist(reference.MongoReference.DatabaseName, reference.MongoReference.CollectionName,
                        reference.MongoReference.Reference))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
