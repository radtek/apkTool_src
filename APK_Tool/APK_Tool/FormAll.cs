using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace APK_Tool
{
    public partial class FormAll : Form
    {
        public FormAll()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Form3().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //xmlNode N = xmlNode.ParseOne("<manifest/>");
            //N.runCMD("add NEWNODE=<action android:name=android.intent.action.MAIN/> to THIS/CHILDS");
            //N.runCMD("add NEWNODE=<action android:name=android.intent.action.MAIN2/> to THIS/CHILDS");
            
            //N.runCMD("remove THIS/CHILDS");
            //String str = N.ToString();


            //online_Data data = Settings.getOnlineSettings("1000", "0000843");

            //List<string> list = Form3.getApkDir(@"C:\Users\wangzhongyuan\Desktop\tmp");
            string tmp = "";
        }
    }
}
