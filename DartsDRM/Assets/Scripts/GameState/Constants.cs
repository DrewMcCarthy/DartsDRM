using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameState
{
    public static class Constants
    {
        public static string GameSetup = "Which game do you want to play? ";
        public static string PlayerCount = "How many players? ";
        public static readonly List<String> Games = new List<String>{"301", "Cricket"};
        public enum InMode { OpenIn, DoubleIn };
        public enum OutMode { OpenOut, DoubleOut};

        public static string OpenIn = "Open In";
        public static string DoubleIn = "Double In";
        public static string OpenOut = "Open Out";
        public static string DoubleOut = "Double Out";

        public static List<string> InOutModes = new List<string>
        {
            string.Format("{0} / {1}", "Open In", "Open Out"),
            string.Format("{0} / {1}", "Open In", "Double Out")
        };

    }
}
