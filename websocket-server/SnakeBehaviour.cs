using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace websocket_server
{
    class World
    {
        public int Counter = 0;

        private List<Room> m_Rooms = new List<Room>();
    }

    class Player
    {

    }

    class Room
    {
        private Dictionary<string, Player> m_Clients = new Dictionary<string, Player>();

        public void Add(string id)
        {
            m_Clients.Add(id, new Player());
        }

        public void Remove(string id)
        {
            m_Clients.Remove(id);
        }

        public bool IsFull()
        {
            return m_Clients.Count == 2;
        }
    }

    class SnakeBehaviour : WebSocketBehavior
    {
        private static World world = new World();

        protected override void OnOpen()
        {
            Console.WriteLine($"OnOpen {ID}");
            BroadcastOnlineCounter();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"OnClose {ID}");
            BroadcastOnlineCounter();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine($"OnMessage {ID}");
            Console.WriteLine(e.Data);

            if (e.Data == "counter")
            {
                world.Counter += 1;
                Send($"{world.Counter}");
            }
        }

        private void BroadcastOnlineCounter()
        {
            string data = $"{{ \"id\": \"count\", \"data\": {Sessions.Count} }}";
            Sessions.Broadcast(data);
        }
    }
}
