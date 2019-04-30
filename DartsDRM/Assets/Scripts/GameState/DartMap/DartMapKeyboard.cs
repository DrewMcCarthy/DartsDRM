using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameState.DartMap
{
    public static class DartMapKeyboard
    {
        public static Dictionary<KeyCode, Dart> GetMark = new Dictionary<KeyCode, Dart>
        {
            {KeyCode.Alpha1, new Dart(25, 2)},
            {KeyCode.Alpha2, new Dart(25, 1)},
            {KeyCode.Alpha3, new Dart(20, 3)},
            {KeyCode.Alpha4, new Dart(20, 2)},
            {KeyCode.Alpha5, new Dart(20, 1)},
            {KeyCode.Alpha6, new Dart(19, 3)},
            {KeyCode.Alpha7, new Dart(19, 2)},
            {KeyCode.Alpha8, new Dart(19, 1)},
            {KeyCode.Alpha9, new Dart(18, 3)},
            {KeyCode.Alpha0, new Dart(18, 2)},
            {KeyCode.Q, new Dart(18, 1)},
            {KeyCode.W, new Dart(17, 3)},
            {KeyCode.E, new Dart(17, 2)},
            {KeyCode.R, new Dart(17, 1)},
            {KeyCode.T, new Dart(16, 3)},
            {KeyCode.Y, new Dart(16, 2)},
            {KeyCode.U, new Dart(16, 1)},
            {KeyCode.I, new Dart(15, 3)},
            {KeyCode.O, new Dart(15, 2)},
            {KeyCode.P, new Dart(15, 1)},
            {KeyCode.A, new Dart(14, 3)},
            {KeyCode.S, new Dart(14, 2)},
            {KeyCode.D, new Dart(14, 1)},
            {KeyCode.F, new Dart(13, 3)},
            {KeyCode.G, new Dart(13, 2)},
            {KeyCode.H, new Dart(13, 1)},
            {KeyCode.J, new Dart(12, 3)},
            {KeyCode.K, new Dart(12, 2)},
            {KeyCode.L, new Dart(12, 1)},
            {KeyCode.Z, new Dart(11, 3)},
            {KeyCode.X, new Dart(11, 2)},
            {KeyCode.C, new Dart(11, 1)},
            {KeyCode.V, new Dart(10, 3)},
            {KeyCode.B, new Dart(10, 2)},
            {KeyCode.N, new Dart(10, 1)}
        };

    }
}
