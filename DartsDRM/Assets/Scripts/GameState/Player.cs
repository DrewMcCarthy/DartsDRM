using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState
{
    [Serializable]
    public class Player
    {
        #region Constructor
        public Player()
        {
            ThrowHistory = new ThrowHistory();
        }
        #endregion


        #region Properties
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public int DartValues => ThrowHistory.SumValue;
        public int DartCount => ThrowHistory.CountDarts;
        public int GameScore { get; set; }
        public int RoundScore { get; set; }
        public ThrowHistory ThrowHistory { get; set; }
        public int PrevGameScore { get; set; }
        #endregion


        #region Methods
        public override string ToString()
        {
            return "Player: " + Name + " Game Score: " + GameScore.ToString() + " Round Score: " + RoundScore.ToString();
        }
        #endregion
    }
}
