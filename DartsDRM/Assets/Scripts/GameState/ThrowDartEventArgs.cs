using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState
{
    public class ThrowDartEventArgs : EventArgs
    {
        public Dart Dart { get; }
        public int DartNumberInRound { get; }

        public ThrowDartEventArgs(Dart dart, int dartNumberInRound)
        {
            Dart = dart;
            DartNumberInRound = dartNumberInRound;
        }
    }
}
