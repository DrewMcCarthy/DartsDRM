using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState.Games
{
    public abstract class Game
    {
        #region Properties
        public string Name { get; set; }
        public List<Player> Players => GameSetup.Instance.Players;
        public Player ActivePlayer => Players[ActivePlayerIndex];
        public Player NextPlayer => Players[NextPlayerIndex];
        public int ActivePlayerIndex { get; set; }
        // Use 0 as index if we get higher than player count
        public int NextPlayerIndex => ActivePlayerIndex + 1 > GameSetup.Instance.PlayerCount - 1 ? 0 : ActivePlayerIndex + 1;
        public bool TurnOver { get; set; }
        public Dart[] DartsThisTurn { get; set; }
        public int DartsThisTurnCount { get; set; }
        public bool IsWin { get; set; }
        public EventHandler<ThrowDartEventArgs> OnDartThrown;
        #endregion

        #region Constructor
        protected Game()
        {
            ActivePlayerIndex = 0;
            DartsThisTurn = new Dart[3];
        }
        #endregion


        #region Overrideable Methods
        public virtual void ThrowDart(Dart dart)
        {
            // Calculate dart value
            dart.Value = GetDartValue(dart);

            // Add to round's darts
            DartsThisTurn[DartsThisTurnCount] = dart;
            DartsThisTurnCount++;

            OnDartThrown(this, new ThrowDartEventArgs(dart, DartsThisTurnCount));

            SetRoundScore();

            if (DartsThisTurnCount == 3)
            {
                TurnOver = true;
            }
        }
        // Main method that should call other methods to determine
        // how to update player score and marks
        public abstract int GetDartValue(Dart dart);

        public abstract void CheckWinner(Dart dart);
        public abstract void AddRoundDartsToPlayer();
        public abstract void EndTurn();
        #endregion


        #region Methods
        public void SetRoundScore()
        {
            int result = 0;

            for(int i=0; i< DartsThisTurnCount; i++)
            {
                result += DartsThisTurn.ElementAt(i).Value;
            }

            ActivePlayer.RoundScore = result;
        }

        // Setting the previous game score here since
        // this is the earliest we access the next player
        protected void IncrementActivePlayer()
        {
            ActivePlayerIndex++;
            if (ActivePlayerIndex > (GameSetup.Instance.PlayerCount - 1))
            {
                ActivePlayerIndex = 0;
            }

            ActivePlayer.PrevGameScore = ActivePlayer.GameScore;
        }

        public void ResetRoundScore()
        {
            ActivePlayer.RoundScore = 0;
        }

        public void UndoThrow()
        {
            if (DartsThisTurnCount > 0 && DartsThisTurn[DartsThisTurnCount - 1] != default)
            {
                var undoneDart = DartsThisTurn[DartsThisTurnCount - 1];

                DartsThisTurn[DartsThisTurnCount - 1] = default;
                DartsThisTurnCount--;
                ActivePlayer.GameScore += undoneDart.Value;
                SetRoundScore();
            }
        }
        #endregion
    }
}
