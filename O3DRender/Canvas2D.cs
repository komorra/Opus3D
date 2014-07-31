using SharpDX;
using SharpDX.Direct3D9;
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
        private enum FillMode
        {
            Solid,
            LinearGradient,
            RadialGradient,
            ConeGradient,
            TexturePattern,
        }

        private FillMode CurrentFillMode = FillMode.Solid;
        private Color CurrentSolidFill = Color.White;

        internal Canvas2D()
        {
        }

        public static Canvas2D FromControl(Control control)
        {
            return Canvas.FromControl<Canvas2D>(control);
        }

        public void Clear()
        {
            PreventNonRenderCalls();

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1, 0);
        }
                
        public void SetSolidFill(Color color)
        {
            PreventNonRenderCalls();

            CurrentSolidFill = color;
            CurrentFillMode = FillMode.Solid;
        }

        public void DrawRectangle(float x, float y, float width, float height)
        {
            PreventNonRenderCalls();

            resources.Shader.SetValue("World", Matrix.Scaling(width / ControlWidth, -height / ControlHeight, 1) * Matrix.Translation(-1, 1, 0) * Matrix.Translation(x / ControlWidth * 2f, -y / ControlHeight * 2f, 0));
            resources.Shader.SetValue("ViewProj", Matrix.Identity);
            resources.Shader.SetValue("Color", CurrentSolidFill.ToVector4());
            resources.Shader.Technique = resources.Shader.GetTechnique("SolidColor");
            resources.Shader.Begin();
            resources.Shader.BeginPass(0);
            
            device.VertexDeclaration = Vertex.GetVertexDeclaration(device);
            device.Indices = resources.RecangleIBuffer;
            device.SetStreamSource(0, resources.RectangleVBuffer, 0, Vertex.SizeInBytes);
            device.DrawIndexedPrimitive(PrimitiveType.LineList, 0, 0, 4, 0, 4);

            resources.Shader.EndPass();
            resources.Shader.End();
        }
    }
}
