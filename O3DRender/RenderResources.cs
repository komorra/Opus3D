using O3DBase.Utils;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DRender
{
    internal class RenderResources
    {
        public const int CircleQuality = 48;
        public readonly VertexBuffer RectangleVBuffer;
        public readonly VertexBuffer CircleVBuffer;
        public readonly IndexBuffer RecangleIBuffer;
        public readonly IndexBuffer CircleIBuffer;
        public readonly IndexBuffer FillCircleIBuffer;
        public readonly IndexBuffer FillRectangleIBuffer;

        public readonly Effect Shader;

        public RenderResources(Device device)
        {
            Vertex[] rectangle = new Vertex[]{
                new Vertex(0,0,0),
                new Vertex(1,0,0),
                new Vertex(0,1,0),
                new Vertex(1,1,0),
            };

            int[] rectanglei = new int[] { 0, 1, 1, 3, 3, 2, 2, 0 };

            RectangleVBuffer = new VertexBuffer(device, rectangle.Length * Vertex.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            var dsv = RectangleVBuffer.Lock(0, 0, LockFlags.Discard);
            dsv.WriteRange(rectangle);
            RectangleVBuffer.Unlock();

            RecangleIBuffer = new IndexBuffer(device, sizeof(int) * rectanglei.Length, Usage.WriteOnly, Pool.Default, false);
            var dsi = RecangleIBuffer.Lock(0, 0, LockFlags.Discard);
            dsi.WriteRange(rectanglei);
            RecangleIBuffer.Unlock();

            int[] fillrecti = new int[] { 0, 1, 2, 3, 2, 1 };

            FillRectangleIBuffer = new IndexBuffer(device, sizeof(int) * fillrecti.Length, Usage.WriteOnly, Pool.Default, false);
            dsi = FillRectangleIBuffer.Lock(0, 0, LockFlags.Discard);
            dsi.WriteRange(fillrecti);
            FillRectangleIBuffer.Unlock();

            Vertex[] circle = new Vertex[CircleQuality + 1];
            for (int la = 0; la < circle.Length;la++)
            {
                var angle = O3DHelper.Scale(la,0,circle.Length,0,MathUtil.TwoPi);
                float x = (float)Math.Sin(angle) * 0.5f + 0.5f;
                float y = (float)Math.Cos(angle) * 0.5f + 0.5f;
                circle[la] = new Vertex(x, y, 0);
            }
            circle[CircleQuality] = new Vertex(0.5f, 0.5f, 0);

            CircleVBuffer = new VertexBuffer(device, circle.Length * Vertex.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            dsv = CircleVBuffer.Lock(0, 0, LockFlags.Discard);
            dsv.WriteRange(circle);
            CircleVBuffer.Unlock();

            int[] circlei = new int[CircleQuality * 2];
            for (int la = 0; la < CircleQuality;la++)
            {
                circlei[la * 2 + 0] = la;
                circlei[la * 2 + 1] = (la + 1) % CircleQuality;
            }

            CircleIBuffer = new IndexBuffer(device, sizeof(int) * circlei.Length, Usage.WriteOnly, Pool.Default, false);
            dsi = CircleIBuffer.Lock(0, 0, LockFlags.Discard);
            dsi.WriteRange(circlei);
            CircleIBuffer.Unlock();

            int[] fcircle = new int[CircleQuality * 3];
            for (int la = 0; la < CircleQuality; la++)
            {
                fcircle[la * 3 + 0] = (la + 1) % CircleQuality;
                fcircle[la * 3 + 1] = la;
                fcircle[la * 3 + 2] = CircleQuality;
            }

            FillCircleIBuffer = new IndexBuffer(device, sizeof(int) * fcircle.Length, Usage.WriteOnly, Pool.Default, false);
            dsi = FillCircleIBuffer.Lock(0, 0, LockFlags.Discard);
            dsi.WriteRange(fcircle);
            FillCircleIBuffer.Unlock();

            Shader = Effect.FromString(device, Resources.Shader, ShaderFlags.None);
        }
    }
}
