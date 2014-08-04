using O3DRender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace O3DGUI
{
    public class GUIArea
    {
        Canvas2D canvas;
        private Dictionary<int, GUIControl> controls = new Dictionary<int, GUIControl>();
        private int nextId = 1;
        private O3DSkin skin;

        public GUIArea(Canvas2D canvas)
        {
            this.canvas = canvas;
            string[] dlls = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "*.dll", SearchOption.TopDirectoryOnly);
            foreach(var dll in dlls)
            {
                var ass = Assembly.LoadFile(dll);
                var types = ass.GetTypes();
                foreach(var t in types)
                {
                    if(t.IsSubclassOf(typeof(O3DSkin)))
                    {
                        skin = Activator.CreateInstance(t) as O3DSkin;
                        goto found;
                    }
                }
            }
        found: ;
        canvas.Render += canvas_Render;
        }

        void canvas_Render()
        {
            foreach(var ctrl in controls.Values.OrderBy(o=>o.ZOrder))
            {
                ctrl.Render(canvas);
            }
        }

        public int AddGUIControl(GUIControl control)
        {
            int id = nextId;
            nextId++;

            control.SetSkin(skin);
            controls.Add(id, control);
            return id;
        }

        public void InputMouseDown(int x, int y, int button)
        {

        }

        public void InputMouseMove(int x,int y, int button)
        {

        }

        public void InputMouseUp(int x, int y, int button)
        {

        }
    }
}
