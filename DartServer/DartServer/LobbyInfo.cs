using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartServer
{
    public class LobbyInfo
    {
        public int LobbyPlayerCount { get; set; }
        public List<ServerGame> ServerGames { get; set; }
    }
}
