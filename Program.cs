using System;
using System.Net;
using System.Net.Sockets;
using MRI.Core.AppConfig;
using MRI.Core.Proxy;

namespace MRI
{
    class Program
    {
        public static void StartListening()
        {
            var ipHostInfo = Dns.Resolve(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint( ipAddress, Config.ClientPort );

            var listener = new Socket( AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp );

            try
            {
                listener.Bind( localEndPoint );
                listener.Listen( Config.Threads );

                while ( true )
                {
                    Console.WriteLine( "Waiting for a connection..." );
                    Socket handler = listener.Accept();
                    new ProxyThread( CreateMongoConnection( ipAddress ), handler );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.ToString() );
            }

            Console.WriteLine( "\nPress ENTER to continue..." );
            Console.Read();
        }

        private static Socket CreateMongoConnection( IPAddress address )
        {
            var socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            socket.Connect( new IPEndPoint( address, Config.ServerPort ) );
            return socket;
        }

        public static int Main( string[] args )
        {
            StartListening();
            return 0;
        }
    }
}
