using O3DRender;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DGUI
{
    public abstract class O3DSkin
    {
        public abstract void DrawWindowBackground(Canvas2D canvas, Color color, float x, float y, float w, float h);
        public abstract void DrawWindowBorder(Canvas2D canvas, Color color, float x, float y, float w, float h);
        public abstract void DrawWindowShadow(Canvas2D canvas, float x, float y, float w, float h);
        public abstract void DrawWindowTopBar(Canvas2D canvas, float x, float y, float w, float h);
        public abstract void DrawWindowTitle(Canvas2D canvas, float x, float y, float w, float h, string title);
    }
}
