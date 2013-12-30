using System;
using System.IO;
using System.Net.Sockets;
using MRI.Core.Message;
using MRI.Factory;

namespace MRI.Core.Proxy
{
    public class Reader
    {
        private readonly BinaryReader _reader;

        private const int MessageLengthFieldSize = 4;

        public Reader( NetworkStream stream )
        {
            _reader = new BinaryReader( stream );
        }

        public MongoStandardMessage Read()
        {
            var length = _reader.ReadInt32();
            
            var ms = new MemoryStream( length );
            ms.Write( BitConverter.GetBytes( length ), 0, 4 );
            ms.Write( _reader.ReadBytes( length - MessageLengthFieldSize ), 0, length - MessageLengthFieldSize );
            ms.Position = 0;

            return MongoRequestMessageFactory.GetRequest( ms.ToArray() );
        }
    }
}
