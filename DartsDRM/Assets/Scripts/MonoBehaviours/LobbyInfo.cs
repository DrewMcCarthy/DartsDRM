using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MonoBehaviours
{
    public class LobbyInfo
    {
        public int LobbyPlayerCount { get; set; }
        public List<ServerGame> ServerGames { get; set; }
    }
}
