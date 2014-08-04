using O3DRender;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DGUI
{
    public class GUIWindow : GUIControl
    {
        private Color backgroundColor = Color.Gray;

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        private Color borderColor = new Color(0.1f, 0.1f, 0.1f);

        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }


        public GUIWindow()
        {
            Left = 0;
            Top = 0;
            Width = 330;
            Height = 300;
            Text = "Window";
        }

        internal override void Render(Canvas2D canvas)
        {
            skin.DrawWindowShadow(canvas, Left, Top, Width, Height);
            skin.DrawWindowBackground(canvas, BackgroundColor, Left, Top, Width, Height);
            skin.DrawWindowBorder(canvas, BorderColor, Left, Top, Width, Height);
            skin.DrawWindowTopBar(canvas, Left, Top, Width, 38);
            skin.DrawWindowTitle(canvas, Left, Top, Width, 40, Text);
        }
    }
}
