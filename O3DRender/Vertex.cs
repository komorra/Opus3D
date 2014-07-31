using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace O3DRender
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Color Color;

        public Vertex(Vector3 position)
        {
            Position = position;
            Normal = Vector3.Zero;
            UV = Vector2.Zero;
            Color = Color.White;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 uv) : this(position)
        {
            Normal = normal;
            UV = uv;
            Color = Color.White;
        }

        public Vertex(Vector3 position, Color color) : this(position)
        {
            Normal = Vector3.Zero;
            UV = Vector2.Zero;
            Color = color;
        }

        static VertexDeclaration vdecl;

        public const int SizeInBytes = sizeof(float) * 9;

        public static VertexDeclaration GetVertexDeclaration(Device device)
        {
            return vdecl ?? (vdecl = new VertexDeclaration(device, new VertexElement[]{
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, sizeof(float) * 3, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, sizeof(float) * 6, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, sizeof(float) * 8, DeclarationType.UByte4N, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            }));
        }
    }
}
