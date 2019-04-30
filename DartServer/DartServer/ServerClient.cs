using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DartServer
{
    [Serializable]
    public class ServerClient
    {
        public TcpClient TcpClient;
        public string Name;
        public bool Disconnected;
        public Guid PlayerGuid { get; set; }
        public Guid GameGuid { get; set; }

        public ServerClient(TcpClient clientSocket)
        {
            Name = "Guest";
            TcpClient = clientSocket;
        }

    }
}
