using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState.DartMap
{
    public static class DartMapBoard10x10
    {
        // Dictionary for GPIO to score lookup
        public static Dictionary<string, Dart> GetMark = new Dictionary<string, Dart>
        {
            {"39-28", new Dart(20,3)},
            {"37-28", new Dart(20,2)},
            {"38-28", new Dart(20,1)},
            {"39-24", new Dart(19,3)},
            {"37-24", new Dart(19,2)},
            {"38-24", new Dart(19,1)},
            {"32-29", new Dart(18,3)},
            {"34-29", new Dart(18,2)},
            {"35-29", new Dart(18,1)},
            {"33-25", new Dart(17,3)},
            {"34-25", new Dart(17,2)},
            {"35-25", new Dart(17,1)},
            {"39-22", new Dart(16,3)},
            {"37-22", new Dart(16,2)},
            {"38-22", new Dart(16,1)},
            {"33-23", new Dart(15,3)},
            {"34-23", new Dart(15,2)},
            {"35-23", new Dart(15,1)},
            {"39-44", new Dart(14,3)},
            {"37-44", new Dart(14,2)},
            {"38-44", new Dart(14,1)},
            {"33-27", new Dart(13,3)},
            {"34-27", new Dart(13,2)},
            {"35-27", new Dart(13,1)},
            {"39-26", new Dart(12,3)},
            {"37-26", new Dart(12,2)},
            {"38-26", new Dart(12,1)},
            {"39-45", new Dart(11,3)},
            {"37-45", new Dart(11,2)},
            {"38-45", new Dart(11,1)},
            {"33-45", new Dart(10,3)},
            {"34-45", new Dart(10,2)},
            {"35-45", new Dart(10,1)},
            {"39-27", new Dart(9,3)},
            {"37-27", new Dart(9,2)},
            {"38-27", new Dart(9,1)},
            {"39-23", new Dart(8,3)},
            {"37-23", new Dart(8,2)},
            {"38-23", new Dart(8,1)},
            {"39-25", new Dart(7,3)},
            {"37-25", new Dart(7,2)},
            {"38-25", new Dart(7,1)},
            {"33-44", new Dart(6,3)},
            {"34-44", new Dart(6,2)},
            {"35-44", new Dart(6,1)},
            {"39-29", new Dart(5,3)},
            {"37-29", new Dart(5,2)},
            {"38-29", new Dart(5,1)},
            {"33-26", new Dart(4,3)},
            {"34-26", new Dart(4,2)},
            {"35-26", new Dart(4,1)},
            {"33-24", new Dart(3,3)},
            {"34-24", new Dart(3,2)},
            {"35-24", new Dart(3,1)},
            {"33-22", new Dart(2,3)},
            {"34-22", new Dart(2,2)},
            {"35-22", new Dart(2,1)},
            {"33-28", new Dart(1,3)},
            {"34-28", new Dart(1,2)},
            {"35-28", new Dart(1,1)},
            {"36-28", new Dart(25,2)},
            {"36-29", new Dart(25,1)}
        };
    }
}
