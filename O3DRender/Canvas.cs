﻿using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace O3DRender
{
    public class Canvas
    {
        public const int RenderInterval = 15;
        protected const string notInRenderEventCallMessage = "Always call draw & render state functions during Render event!";

        public event Action Render = delegate { };

        protected bool isRenderEvent = false;

        protected static Device device;
        protected Sprite sprite;
        internal static RenderResources resources;
        private Timer timer;
        protected Control control;

        internal Canvas()
        {
            if(device == null)
            {
                var pp = new PresentParameters(2048, 2048);
                device = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, pp);

                resources = new RenderResources(device);
            }
            sprite = new Sprite(device);
        }

        public float ControlWidth
        {
            get
            {
                return control.ClientSize.Width;
            }
        }

        public float ControlHeight
        {
            get
            {
                return control.ClientSize.Height;
            }
        }
        
        internal static T FromControl<T>(Control control) where T : Canvas
        {
            T canvas = null;
            if (typeof(T) == typeof(Canvas2D))
            {
                canvas = new Canvas2D() as T;
            }
            else if(typeof(T) == typeof(Canvas3D))
            {
                canvas = new Canvas3D() as T;
            }

            canvas.control = control;

            canvas.timer = new Timer();
            canvas.timer.Tick += canvas.RenderTick;
            canvas.timer.Interval = RenderInterval;
            canvas.timer.Start();            

            return canvas;
        }

        private void RenderTick(object sender, EventArgs e)
        {
            isRenderEvent = true;

            device.Viewport = new Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);
            device.BeginScene();
            device.SetRenderState(RenderState.ZEnable, this is Canvas3D);
            var c2d = this as Canvas2D;
            if(c2d!=null)
            {
                c2d.SetAlphaBlend();
            }
            Render();
            device.EndScene();
            var rect = new Rectangle(0,0,control.ClientSize.Width,control.ClientSize.Height);
            device.Present(rect, rect, control.Handle);

            isRenderEvent = false;
        }        

        [DebuggerStepThrough]
        protected void PreventNonRenderCalls()
        {
            if (!isRenderEvent) throw new O3DRenderException(notInRenderEventCallMessage);
        }
    }
}
