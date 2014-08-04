using O3DBase.Configuration;
using O3DGUI;
using O3DRender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Opus3D
{
    public partial class FormMain : Form
    {
        private Point m1, m2;
        private GUIArea guiArea;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Text = O3DInfo.ApplicationName + " v" + (O3DInfo.ApplicationVersion / 1000) + "." + (O3DInfo.ApplicationVersion % 1000).ToString("000");
            Icon = O3DInfo.ApplicationIcon;

            MouseDown += (a, b) => m1 = b.Location;
            MouseMove += (a, b) => { if (b.Button == System.Windows.Forms.MouseButtons.Left) m2 = b.Location; };

            Image img = Image.FromFile(@"C:\Users\User\Pictures\baringtonia01L.png");

            Canvas2D canvas = Canvas2D.FromControl(this);
            canvas.Render += ()=>
                {
                    //canvas.Clear();
                    //canvas.SetSolidFill(SharpDX.Color.Red);
                    //canvas.SetSquareGradientFill(new SharpDX.Vector2(m1.X,m1.Y), new SharpDX.Vector2(m2.X,m2.Y), new GradientStop(0, SharpDX.Color.Red), new GradientStop(0.5f, SharpDX.Color.Yellow), new GradientStop(1f, SharpDX.Color.Green));
                    
                    //canvas.FillRectangle(0, 0, ClientSize.Width, ClientSize.Height);

                    //canvas.SetColorMultiply(new SharpDX.Color(1f, 0.5f, 0.1f, 0.5f));
                    //canvas.SetTexturePatternFill(new SharpDX.Vector2(m1.X, m1.Y), new SharpDX.Vector2(m2.X, m2.Y), img);
                    //canvas.FillEllipse(0, 0, 200, 200);
                    //canvas.DrawText(10, 10, 1000, 10, SystemFonts.DefaultFont, "123 Lorem ipsum test");
                };
            guiArea = new GUIArea(canvas);
            guiArea.AddGUIControl(new GUIWindow() { Left = 50, Top = 60, Width = 1000, Height = 800 });
            guiArea.AddGUIControl(new GUIWindow() { Left = 150, Top = 100 });
            
        }
    }
}
