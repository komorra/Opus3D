using O3DGUI;
using O3DRender;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SD = System.Drawing;

namespace DefaultSkin
{
    public class Skin : O3DSkin
    {
        public override void DrawWindowBackground(Canvas2D canvas, Color color, float x, float y, float w, float h)
        {
            var bottomColor = Color.Lerp(color, Color.Black, 0.5f);

            canvas.SetLinearGradientFill(new Vector2(x, y), new Vector2(x, y + h), new GradientStop(0, color), new GradientStop(1, bottomColor));
            canvas.FillRectangle(x, y, w, h);
            canvas.SetLinearGradientFill(new Vector2(x + 5, y + 5), new Vector2(x - 5, y + h - 5), new GradientStop(0, Color.Transparent), new GradientStop(0.2f, Color.Transparent), new GradientStop(1f, new Color(1, 1, 1, 0.2f)));
            canvas.FillRectangle(x + 5, y + 5, w - 10, h - 10);
        }

        public override void DrawWindowBorder(Canvas2D canvas, Color color, float x, float y, float w, float h)
        {
            var topColor = Color.Lerp(color, Color.White, 0.7f);

            canvas.SetLinearGradientFill(new Vector2(x, y), new Vector2(x, y + h), new GradientStop(0, topColor), new GradientStop(1, color));
            canvas.DrawRectangle(x, y, w, h);
        }

        public override void DrawWindowShadow(Canvas2D canvas, float x, float y, float w, float h)
        {
            canvas.SetLinearGradientFill(new Vector2(x + 5, y + 5), new Vector2(x + 5, y + h + 5), new GradientStop(0, new Color(0, 0, 0, 0.3f)), new GradientStop(1, new Color(0, 0, 0, 0.1f)));
            canvas.FillRectangle(x + 5, y + 5, w, h);
        }

        public override void DrawWindowTopBar(Canvas2D canvas, float x, float y, float w, float h)
        {
            canvas.SetLinearGradientFill(new Vector2(x + 8, y + 8), new Vector2(x + w - 8, y + 8), new GradientStop(0, new Color(0f,0f,0f, 0.5f)), new GradientStop(1, new Color(0f,0f,0f, 0.2f)));
            canvas.FillRectangle(x + 8, y + 8, w - 16, h - 16);
        }

        public override void DrawWindowTitle(Canvas2D canvas, float x, float y, float w, float h, string title)
        {
            canvas.SetSolidFill(Color.White);
            canvas.DrawText(x + 12, y + 12, w - 12, h - 12, SD.SystemFonts.CaptionFont, title);
        }       
    }
}
