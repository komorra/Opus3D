using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace O3DRender
{
    public sealed class Canvas3D : Canvas
    {
        internal Canvas3D()
        {
        }

        public static Canvas3D FromControl(Control control)
        {
            return Canvas.FromControl<Canvas3D>(control);
        }
    }
}
