using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartServer
{
    public enum MessageType { PlayerGuid, GameGuid, String, Player, LobbyInfo, HostGame, JoinGame, Dart, EndTurn, Disconnect };

    [Serializable]
    public class MessageWrapper
    {
        public MessageType Type { get; set; }
        public object Message { get; set; }
    }
}
