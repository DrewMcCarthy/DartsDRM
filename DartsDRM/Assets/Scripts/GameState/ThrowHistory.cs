using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState
{
    [Serializable]
    public class ThrowHistory
    {
        #region Fields
        private List<Dart> Darts;
        #endregion


        #region Constructor
        public ThrowHistory()
        {
            Darts = new List<Dart>();
        }
        #endregion


        #region Properties
        public int SumValue => Darts.Sum(dart => dart.Value);
        
        public int SumMarks(int mark)
        {
            int result = 0;

            foreach(var dart in Darts)
            {
                if (dart.Mark == mark)
                {
                    result += dart.Multiplier;
                }
            }

            return result;
        }

        public int CountDarts => Darts.Count();
        #endregion


        #region Methods
        public void AddThrow(Dart dart)
        {
            Darts.Add(dart);
        }

        public void UndoThrow()
        {
            if (Darts.Any())
            {
                Darts.RemoveAt(Darts.Count - 1);
            }
        }
        #endregion
    }
}
