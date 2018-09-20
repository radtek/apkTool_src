using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        bool isload = false;
        public HelpForm(Bitmap image)
        {
            InitializeComponent();

            this.BackgroundImage = image;
            this.Width = image.Width + this.Width - this.ClientRectangle.Width + 10;
            this.Height = image.Height + this.Height - this.ClientRectangle.Height + 10;
            this.BackgroundImageLayout = ImageLayout.Center;

            isload = true;
        }

        private void HelpForm_SizeChanged(object sender, EventArgs e)
        {
            if (isload)
            {
                this.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }
    }
}
