using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState.DartMap
{
    public static class DartMapBoard
    {
        // Dictionary for GPIO to score lookup
        public static Dictionary<string, Dart> GetMark = new Dictionary<string, Dart>
        {
            {"25-4", new Dart(20,3)},
            {"25-17", new Dart(20,2)},
            {"25-22", new Dart(20,1)},
            {"20-5", new Dart(19,3)},
            {"20-6", new Dart(19,2)},
            {"20-14", new Dart(19,1)},
            {"20-4", new Dart(18,3)},
            {"20-17", new Dart(18,2)},
            {"20-22", new Dart(18,1)},
            {"26-5", new Dart(17,3)},
            {"26-6", new Dart(17,2)},
            {"26-14", new Dart(17,1)},
            {"25-5", new Dart(16,3)},
            {"25-6", new Dart(16,2)},
            {"25-14", new Dart(16,1)},
            {"23-5", new Dart(15,3)},
            {"23-6", new Dart(15,2)},
            {"23-14", new Dart(15,1)},
            {"12-5", new Dart(14,3)},
            {"12-6", new Dart(14,2)},
            {"12-14", new Dart(14,1)},
            {"26-4", new Dart(13,3)},
            {"26-17", new Dart(13,2)},
            {"26-22", new Dart(13,1)},
            {"24-4", new Dart(12,3)},
            {"24-17", new Dart(12,2)},
            {"24-22", new Dart(12,1)},
            {"24-5", new Dart(11,3)},
            {"24-6", new Dart(11,2)},
            {"24-14", new Dart(11,1)},
            {"0-0", new Dart(10,3)},
            {"23-17", new Dart(10,2)},
            {"23-22", new Dart(10,1)},
            {"12-4", new Dart(9,3)},
            {"12-17", new Dart(9,2)},
            {"12-22", new Dart(9,1)},
            {"18-5", new Dart(8,3)},
            {"18-6", new Dart(8,2)},
            {"18-14", new Dart(8,1)},
            {"21-5", new Dart(7,3)},
            {"21-6", new Dart(7,2)},
            {"21-14", new Dart(7,1)},
            {"19-4", new Dart(6,3)},
            {"19-17", new Dart(6,2)},
            {"19-22", new Dart(6,1)},
            {"18-4", new Dart(5,3)},
            {"18-17", new Dart(5,2)},
            {"18-22", new Dart(5,1)},
            {"16-4", new Dart(4,3)},
            {"16-17", new Dart(4,2)},
            {"16-22", new Dart(4,1)},
            {"16-5", new Dart(3,3)},
            {"16-6", new Dart(3,2)},
            {"16-14", new Dart(3,1)},
            {"19-5", new Dart(2,3)},
            {"19-6", new Dart(2,2)},
            {"19-14", new Dart(2,1)},
            {"21-4", new Dart(1,3)},
            {"21-17", new Dart(1,2)},
            {"21-22", new Dart(1,1)},
            {"12-27", new Dart(25,2)},
            {"24-27", new Dart(25,1)}
        };
    }
}
