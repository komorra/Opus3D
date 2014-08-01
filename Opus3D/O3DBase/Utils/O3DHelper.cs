using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DBase.Utils
{
    public static class O3DHelper
    {
        public static float Scale(float x, float a, float b, float c, float d)
        {
            float s = (x - a) / (b - a);
            return s * (d - c) + c;
        }
    }
}
