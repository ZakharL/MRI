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

        public static int ConvertByteToInt32( byte[] lenghtBytes )
        {
            return BitConverter.ToInt32( lenghtBytes, 0 );
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

            byte item;
            do
            {
                item = reader.ReadByte();
                if (item != '\0')
                {
                    ms.Write(BitConverter.GetBytes(item), 0, 1);    
                }
            } while (item != '\0');

            return Encoding.UTF8.GetString( ms.ToArray() );
        }

        public static string ParseCollectionName(string fullName)
        {
            var parts = fullName.Split('.');
            return parts[1];
        }

        public static string ParseDbName(string fullName)
        {
            var parts = fullName.Split('.');
            return parts[0];
        }
    }
}
