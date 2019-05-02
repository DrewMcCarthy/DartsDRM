using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.GameState.Constants;

namespace Assets.Scripts.GameState.Games
{
    public class ZeroOne : Game
    {
        #region Fields
        private int _startScore;
        //private InMode _inMode;
        private OutMode _outMode;
        #endregion


        #region Constructor
        public ZeroOne(int startScore)
        {
            _startScore = startScore;

            Name =_startScore.ToString();

            InitGameScore();

            //_inMode = InMode.OpenIn;
            _outMode = OutMode.OpenOut;
        }
        #endregion


        #region Base class overrides
        public override void ThrowDart(Dart dart)
        {
            base.ThrowDart(dart);
            ActivePlayer.GameScore -= dart.Value;
            BustOnPoints(dart);

            CheckWinner(dart);
        }

        public override void AddRoundDartsToPlayer()
        {
            for (int i = 0; i < DartsThisTurnCount; i++)
            {
                var dart = DartsThisTurn.ElementAt(i);

                if (IsBust)
                {
                    dart.Value = 0;
                }

                ActivePlayer.ThrowHistory.AddThrow(dart);
            }
        }

        public override void RevertGameScoreOnBust()
        {
            if (IsBust)
            {
                ActivePlayer.GameScore = ActivePlayer.PrevGameScore;
            }
        }

        public override int GetDartValue(Dart dart)
        {
            return (dart.Mark * dart.Multiplier);
        }

        public override void CheckWinner(Dart dart)
        {
            if(ActivePlayer.GameScore == 0)
            {
                if(_outMode == OutMode.OpenOut)
                {
                    IsWin = true;
                }
                if(_outMode == OutMode.DoubleOut && dart.Multiplier == 2)
                {
                    IsWin = true;
                }
            }
        }

        public override void BustOnPoints(Dart dart)
        {
            if (_outMode == OutMode.DoubleOut)
            {
                if (ActivePlayer.GameScore == 1 || ActivePlayer.GameScore < 0)
                {
                    IsBust = true;
                }

                if (ActivePlayer.GameScore == 0 && dart.Multiplier != 2)
                {
                    IsBust = true;
                }
            }
            else
            {
                if (ActivePlayer.GameScore < 0)
                {
                    IsBust = true;
                }
            }
        }
        #endregion


        #region Methods
        private void InitGameScore()
        {
            foreach(var player in GameSetup.Instance.Players)
            {
                player.GameScore = _startScore;
                player.PrevGameScore = _startScore;
            }
        }
        #endregion

    }
}
