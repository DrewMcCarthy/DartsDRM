﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartServer
{
    [Serializable]
    public class OnlineDart
    {
        public Guid GameGuid { get; set; }
        public Guid PlayerGuid { get; set; }
        public Dart Dart { get; set; }
    }
}
