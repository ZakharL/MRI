using System;
using System.IO;
using System.Text;

namespace MRI.Helpers
{
    public class Util
    {
        public static T[] SubArray<T>( T[] data, int index, int length )
        {
            var result = new T[ length ];
            Array.Copy( data, index, result, 0, length );
            return result;
        }

        public static T[] MergeArray<T>( T[] array1, T[] array2 )
        {
            var result = new T[ array1.Length + array2.Length ];

            Array.Copy( array1, result, array1.Length );
            Array.Copy( array2, 0, result, array1.Length, array2.Length );

            return result;
        }

        public static string ReadString( BinaryReader reader )
        {
            var ms = new MemoryStream();
            while ( true )
            {
                byte b = reader.ReadByte();
                if ( b != 0 ) break;
                ms.WriteByte( b );
            }
            return Encoding.UTF8.GetString( ms.ToArray() );
        }
        
        public static string ParseCollectionName(string fullName)
        {
            return fullName.Split('.')[1];
        }

        public static string ParseDbName(string fullName)
        {
            return fullName.Split('.')[0];
        }
    }
}
