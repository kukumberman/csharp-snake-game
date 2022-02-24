using System;
using WebSocketSharp.Server;

namespace websocket_server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WebSocketServer(5555);
            server.AddWebSocketService<SnakeBehaviour>("/snake");
            server.Start();

            Console.WriteLine("Hello World!");
            Console.ReadKey();

            server.Stop();
        }
    }
}
