using O3DRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DGUI
{
    public abstract class GUIControl
    {
        private float left;

        public float Left
        {
            get { return left; }
            set { left = value; }
        }


        private float top;

        public float Top
        {
            get { return top; }
            set { top = value; }
        }

        private float width;

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        private float height;

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        private int zOrder;

        public int ZOrder
        {
            get { return zOrder; }
            set { zOrder = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }



        protected O3DSkin skin;

        internal void SetSkin(O3DSkin skin)
        {
            this.skin = skin;
        }

        internal virtual void Render(Canvas2D canvas) { }
     
    }
}
