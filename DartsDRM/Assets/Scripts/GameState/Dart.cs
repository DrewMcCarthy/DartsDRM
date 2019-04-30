using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState
{
    [Serializable]
    public class Dart
    {
        public Dart(int mark, int multiplier)
        {
            Mark = mark;
            Multiplier = multiplier;
        }

        public int Mark { get; set; }
        public int Multiplier { get; set; }
        // Determined by applying the rules of the game to the Mark/Multiplier
        public int Value { get; set; }

        public override string ToString()
        {
            var multiplierLabel = "";

            switch (Multiplier)
            {
                case 1:
                    multiplierLabel = "Single";
                    break;
                case 2:
                    multiplierLabel = "Double";
                    break;
                case 3:
                    multiplierLabel = "Triple";
                    break;
                default:
                    multiplierLabel = null;
                    break;
            }

            var markLabel = "";
            markLabel = Mark == 25 ? "Bull" : Mark.ToString();

            return multiplierLabel + " " + markLabel;
        }
    }
}
