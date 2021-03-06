﻿using O3DBase.Utils;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gradient = System.Tuple<SharpDX.Vector2, SharpDX.Vector2, O3DRender.GradientStop[]>;
using SD = System.Drawing;

namespace O3DRender
{
    public sealed class Canvas2D : Canvas
    {
        public const int GradientQuality = 1024;

        public class GradientStopComparer : IEqualityComparer<GradientStop>
        {
            public bool Equals(GradientStop left, GradientStop right)
            {
                if (left == null || right == null)
                {
                    return left == right;
                }
                return left.Color == right.Color && left.Offset == right.Offset;
            }

            public int GetHashCode(GradientStop key)
            {
                if(key == null)
                {
                    throw new ArgumentException("key");
                }
                return key.Color.ToBgra() + (int)(key.Offset * 100f);
            }
        }

        public class GradientStopArrayComparer : IEqualityComparer<GradientStop[]>
        {
            public bool Equals(GradientStop[] left, GradientStop[] right)
            {
                if (left == null || right == null)
                {
                    return left == right;
                }
                return left.SequenceEqual(right, new GradientStopComparer());
            }

            public int GetHashCode(GradientStop[] key)
            {
                if(key == null)
                {
                    throw new ArgumentException("key");
                }
                return (int)key.Sum(o => (long)o.Color.ToBgra() + (long)(o.Offset * 100f));
            }
        }

        private enum FillMode
        {
            Solid,
            LinearGradient,
            RadialGradient,
            ConeGradient,
            SquareGradient,
            TexturePattern,
        }

        private FillMode CurrentFillMode = FillMode.Solid;
        private Color CurrentSolidFill = Color.White;
        private Dictionary<GradientStop[], Texture> gradients = new Dictionary<GradientStop[], Texture>(new GradientStopArrayComparer());
        private Dictionary<SD.Image, Texture> textures = new Dictionary<SD.Image, Texture>();
        private Dictionary<SD.Font, Font> fonts = new Dictionary<SD.Font, Font>();
        private Texture CurrentGradient = null;
        private Vector2 CurrentGradientBegin;
        private Vector2 CurrentGradientEnd;
        private Texture CurrentTexturePattern = null;
        private Color CurrentMultiplyColor = Color.White;

        internal Canvas2D()
        {
        }

        public static Canvas2D FromControl(Control control)
        {
            return Canvas.FromControl<Canvas2D>(control);
        }

        public void SetAlphaBlend()
        {
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState<BlendOperation>(RenderState.BlendOperation, BlendOperation.Add);
            device.SetRenderState<Blend>(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState<Blend>(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
        }

        public void SetColorMultiply(Color color)
        {
            PreventNonRenderCalls();

            CurrentMultiplyColor = color;
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

        public void SetLinearGradientFill(Vector2 begin, Vector2 end, params GradientStop[] stops)
        {
            PreventNonRenderCalls();

            SetGradient(begin, end, stops);
            CurrentFillMode = FillMode.LinearGradient;
        }

        public void SetRadialGradientFill(Vector2 begin, Vector2 end, params GradientStop[] stops)
        {
            PreventNonRenderCalls();

            SetGradient(begin, end, stops);
            CurrentFillMode = FillMode.RadialGradient;
        }

        public void SetConeGradientFill(Vector2 begin, Vector2 end, params GradientStop[] stops)
        {
            PreventNonRenderCalls();

            SetGradient(begin, end, stops);
            CurrentFillMode = FillMode.ConeGradient;
        }

        public void SetSquareGradientFill(Vector2 begin, Vector2 end, params GradientStop[] stops)
        {
            PreventNonRenderCalls();

            SetGradient(begin, end, stops);
            CurrentFillMode = FillMode.SquareGradient;
        }

        public unsafe void SetTexturePatternFill(Vector2 begin, Vector2 end, SD.Image texture)
        {
            PreventNonRenderCalls();

            CurrentGradientBegin = begin;
            CurrentGradientEnd = end;

            CurrentFillMode = FillMode.TexturePattern;

            if(textures.ContainsKey(texture))
            {
                CurrentTexturePattern = textures[texture];
            }
            else
            {
                Texture tex = new Texture(device, texture.Width, texture.Height, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);                
                var r = tex.LockRectangle(0, LockFlags.Discard);

                var bd = (texture as SD.Bitmap).LockBits(new SD.Rectangle(0, 0, texture.Width, texture.Height), SD.Imaging.ImageLockMode.ReadOnly, SD.Imaging.PixelFormat.Format32bppArgb);

                for (int lb = 0; lb < texture.Height;lb++)
                {
                    for(int la=0;la<texture.Width;la++)
                    {
                        var dst = (int*)(r.DataPointer + la * 4 + lb * r.Pitch);
                        var src = (int*)(bd.Scan0 + la * 4 + lb * bd.Stride);
                        *dst = *src;
                    }
                }

                (texture as SD.Bitmap).UnlockBits(bd);
                tex.UnlockRectangle(0);

                textures.Add(texture, tex);
            }
        }

        private unsafe void SetGradient(Vector2 begin, Vector2 end, params GradientStop[] stops)
        {            
            CurrentGradientBegin = begin;
            CurrentGradientEnd = end;

            if(gradients.ContainsKey(stops))
            {
                CurrentGradient = gradients[stops];
            }
            else
            {
                Texture gtex = new Texture(device, GradientQuality, 1, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
                DataStream ds = null;
                gtex.LockRectangle(0, LockFlags.Discard, out ds);
                var sorted = stops.OrderBy(o => o.Offset).ToArray();
                for (int la = 0; la < GradientQuality;la++)
                {
                    float offset = O3DHelper.Scale(la, 0, GradientQuality - 1, 0, 1);
                    GradientStop first = null;
                    GradientStop last = null;
                    for(int lb=0;lb<sorted.Length;lb++)
                    {
                        if(sorted[lb].Offset > offset)
                        {
                            last = sorted[lb];
                            first = sorted[Math.Max(0, lb - 1)];
                            break;
                        }
                    }
                    if (last == null)
                    {
                        last = sorted[sorted.Length - 1];
                        first = sorted[Math.Max(0, sorted.Length - 2)];
                    }
                    Color interpolated = Color.Lerp(first.Color, last.Color, O3DHelper.Scale(offset, first.Offset, last.Offset, 0, 1));
                    ds.Write(interpolated.ToBgra());
                }
                gtex.UnlockRectangle(0);
                gradients.Add(stops, gtex);
                if(gradients.Count > 1000)
                {
                    gradients.Remove(gradients.First().Key);
                }
            }
        }

        public void DrawText(float x, float y, float width, float height, SD.Font font, string text)
        {
            PreventNonRenderCalls();

            Font f = null;
            if (!fonts.ContainsKey(font))
            {
                f = new Font(device, font);
                fonts.Add(font, f);
            }
            else
            {
                f = fonts[font];
            }

            device.ScissorRect = new Rectangle((int)x, (int)y, (int)width, (int)height);

            SetProperTechnique();
            SetShaderValues();

            sprite.Begin(SpriteFlags.AlphaBlend);            
            
            f.DrawText(sprite, text, (int)x, (int)y, CurrentSolidFill);
                        
            sprite.End();
            device.ScissorRect = new Rectangle(0, 0, (int)ControlWidth, (int)ControlHeight);
        }

        public void DrawRectangle(float x, float y, float width, float height)
        {
            PreventNonRenderCalls();

            resources.Shader.SetValue("World", Matrix.Scaling(width / ControlWidth * 2f, -height / ControlHeight * 2f, 1) * Matrix.Translation(-1, 1, 0) * Matrix.Translation(x / ControlWidth * 2f, -y / ControlHeight * 2f, 0));
            SetShaderValues();
            SetProperTechnique();
            resources.Shader.Begin();
            resources.Shader.BeginPass(0);
            
            device.VertexDeclaration = Vertex.GetVertexDeclaration(device);
            device.Indices = resources.RecangleIBuffer;
            device.SetStreamSource(0, resources.RectangleVBuffer, 0, Vertex.SizeInBytes);
            device.DrawIndexedPrimitive(PrimitiveType.LineList, 0, 0, 4, 0, 4);

            resources.Shader.EndPass();
            resources.Shader.End();
        }

        public void DrawEllipse(float x, float y, float width, float height)
        {
            PreventNonRenderCalls();

            resources.Shader.SetValue("World", Matrix.Scaling(width / ControlWidth * 2f, -height / ControlHeight * 2f, 1) * Matrix.Translation(-1, 1, 0) * Matrix.Translation(x / ControlWidth * 2f, -y / ControlHeight * 2f, 0));
            SetShaderValues();
            SetProperTechnique();
            resources.Shader.Begin();
            resources.Shader.BeginPass(0);

            device.VertexDeclaration = Vertex.GetVertexDeclaration(device);
            device.Indices = resources.CircleIBuffer;
            device.SetStreamSource(0, resources.CircleVBuffer, 0, Vertex.SizeInBytes);
            device.DrawIndexedPrimitive(PrimitiveType.LineList, 0, 0, RenderResources.CircleQuality, 0, RenderResources.CircleQuality);

            resources.Shader.EndPass();
            resources.Shader.End();
        }

        private void SetShaderValues()
        {
            resources.Shader.SetValue("ViewProj", Matrix.Identity);
            resources.Shader.SetValue("Color", CurrentSolidFill.ToVector4());
            resources.Shader.SetTexture("Gradient", CurrentGradient);
            resources.Shader.SetValue("GradientBegin", CurrentGradientBegin);
            resources.Shader.SetValue("GradientEnd", CurrentGradientEnd);
            resources.Shader.SetValue("Size", new Vector2(ControlWidth, ControlHeight));
            resources.Shader.SetTexture("Texture", CurrentTexturePattern);
            resources.Shader.SetValue("ColorMul", CurrentMultiplyColor.ToVector4());
        }

        public void FillRectangle(float x, float y, float width, float height)
        {
            PreventNonRenderCalls();

            resources.Shader.SetValue("World", Matrix.Scaling(width / ControlWidth * 2f, -height / ControlHeight * 2f, 1) * Matrix.Translation(-1, 1, 0) * Matrix.Translation(x / ControlWidth * 2f, -y / ControlHeight * 2f, 0));
            SetShaderValues();
            SetProperTechnique();
            resources.Shader.Begin();
            resources.Shader.BeginPass(0);

            device.VertexDeclaration = Vertex.GetVertexDeclaration(device);
            device.Indices = resources.FillRectangleIBuffer;
            device.SetStreamSource(0, resources.RectangleVBuffer, 0, Vertex.SizeInBytes);
            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

            resources.Shader.EndPass();
            resources.Shader.End();
        }

        public void FillEllipse(float x, float y, float width, float height)
        {
            PreventNonRenderCalls();

            resources.Shader.SetValue("World", Matrix.Scaling(width / ControlWidth * 2f, -height / ControlHeight * 2f, 1) * Matrix.Translation(-1, 1, 0) * Matrix.Translation(x / ControlWidth * 2f, -y / ControlHeight * 2f, 0));
            SetShaderValues();
            SetProperTechnique();
            resources.Shader.Begin();
            resources.Shader.BeginPass(0);

            device.VertexDeclaration = Vertex.GetVertexDeclaration(device);
            device.Indices = resources.FillCircleIBuffer;
            device.SetStreamSource(0, resources.CircleVBuffer, 0, Vertex.SizeInBytes);
            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, RenderResources.CircleQuality + 1, 0, RenderResources.CircleQuality);

            resources.Shader.EndPass();
            resources.Shader.End();
        }

        private void SetProperTechnique()
        {
            switch(CurrentFillMode)
            { 
                case FillMode.Solid:
                    resources.Shader.Technique = resources.Shader.GetTechnique("SolidColor");
                    break;
                case FillMode.LinearGradient:
                    resources.Shader.Technique = resources.Shader.GetTechnique("LinearGradient");
                    break;
                case FillMode.RadialGradient:
                    resources.Shader.Technique = resources.Shader.GetTechnique("RadialGradient");
                    break;
                case FillMode.ConeGradient:
                    resources.Shader.Technique = resources.Shader.GetTechnique("ConeGradient");
                    break;
                case FillMode.SquareGradient:
                    resources.Shader.Technique = resources.Shader.GetTechnique("SquareGradient");
                    break;
                case FillMode.TexturePattern:
                    resources.Shader.Technique = resources.Shader.GetTechnique("TexturePattern");
                    break;
            }
        }
    }
}
