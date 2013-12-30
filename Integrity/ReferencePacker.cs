using System.Collections.Generic;
using System.Linq;
using MRI.Integrity.Parser;

namespace MRI.Integrity
{
    public class ReferencePacker
    {
        List<ReferencePack> References { get; set; }

        public ReferencePacker()
        {
            References = new List<ReferencePack>();
        }

        public void Add( MongoReference reference )
        {
            foreach ( var item in References )
            {
                if ( item.MongoReference == reference )
                {
                    item.Count++;
                    return;
                }
            }
            References.Add(new ReferencePack(reference));
        }

        public void Add (List<MongoReference> references)
        {
            foreach ( var mongoReference in references )
            {
                Add( mongoReference );
            }
        }

        public List<ReferencePack> Get()
        {
            return References;
        }

        private static List<ReferencePack> ApplyChange(List<ReferencePack> afterChanges, ReferencePack beforeChange)
        {
            foreach (var afterChange in afterChanges)
            {
                if (beforeChange == afterChange)
                {
                    afterChange.Count = afterChange.Count - beforeChange.Count;
                    return afterChanges;
                }
            }

            var change = (ReferencePack) beforeChange.Clone();
            change.Count *= -1;

            afterChanges.Add(change);

            return afterChanges;
        }

        public static List<ReferencePack> CalculateChanges(List<ReferencePack> beforeChanges, List<ReferencePack> afterChanges)
        {
            var result = afterChanges.Select(item => (ReferencePack)item.Clone()).ToList();

            foreach (var beforeChage in beforeChanges)
            {
                result = ApplyChange(result, beforeChage);
            }
            
            return result;
        }
    }
}
