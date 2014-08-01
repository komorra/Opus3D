using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DRender
{
    internal class RenderResources
    {
        public readonly VertexBuffer RectangleVBuffer;
        public readonly IndexBuffer RecangleIBuffer;
        
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

            Shader = Effect.FromString(device, Resources.Shader, ShaderFlags.None);
        }
    }
}
