using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace O3DBase.Configuration
{
    public static class O3DInfo
    {
        public static readonly string ApplicationName = "Opus3D";
        public static readonly int ApplicationVersion = 1; //Major m*1000 + Minor n, i.e. 1022 = v1.022
        public static readonly DateTime ApplicationDate = DateTime.Parse("2014-07-31");
        public static readonly Icon ApplicationIcon = Resources.opus3d;
    }
}
