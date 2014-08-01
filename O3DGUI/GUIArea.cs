using O3DRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DGUI
{
    public class GUIArea
    {
        Canvas2D canvas;
        private Dictionary<int, GUIControl> controls = new Dictionary<int, GUIControl>();
        private int nextId = 1;

        public GUIArea(Canvas2D canvas)
        {
            this.canvas = canvas;
        }

        public int AddGUIControl(GUIControl control)
        {
            int id = nextId;
            nextId++;

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
