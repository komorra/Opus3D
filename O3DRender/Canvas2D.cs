using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace O3DRender
{
    public sealed class Canvas2D : Canvas
    {
        internal Canvas2D()
        {
        }

        public static Canvas2D FromControl(Control control)
        {
            return Canvas.FromControl<Canvas2D>(control);
        }
                
        public void SetSolidFill(Color color)
        {
            PreventNonRenderCalls();
        }

        public void DrawRectangle(float x, float y, float width, float height)
        {
            PreventNonRenderCalls();
        }
    }
}
