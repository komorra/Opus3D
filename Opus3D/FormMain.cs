using O3DBase.Configuration;
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
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Text = O3DInfo.ApplicationName + " v" + (O3DInfo.ApplicationVersion / 1000) + "." + (O3DInfo.ApplicationVersion % 1000).ToString("000");
            Icon = O3DInfo.ApplicationIcon;
        }
    }
}
