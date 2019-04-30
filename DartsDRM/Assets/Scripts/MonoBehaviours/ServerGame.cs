using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MonoBehaviours
{
    [Serializable]
    public class ServerGame
    {
        public Guid GameGuid { get; set; }
        public List<Guid> PlayerGuids { get; set; }

        public ServerGame()
        {
            PlayerGuids = new List<Guid>();
        }
    }
}
