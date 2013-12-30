using System;
using MRI.Integrity.Parser;

namespace MRI.Integrity
{
    public class ReferencePack : ICloneable 
    {
        public MongoReference MongoReference { get; set; }
        public int Count { get; set; }

        public ReferencePack(MongoReference mongoReference)
        {
            MongoReference = mongoReference;
            Count = 1;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
