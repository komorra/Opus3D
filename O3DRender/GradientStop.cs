using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DRender
{
    public class GradientStop
    {
        public float Offset;
        public Color Color;

        public GradientStop(float offset, Color color)
        {
            Offset = offset;
            Color = color;
        }
    }
}
