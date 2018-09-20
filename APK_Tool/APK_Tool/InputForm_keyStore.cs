using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace APK_Tool
{
    /// <summary>
    /// 输入keystoreName签名对应信息到InfoPath
    /// </summary>
    public partial class InputForm_keyStore : Form
    {
        String InfoPath = "";       // keystore对应的签名文件信息路径
        String keysotreName = "";   // keyStore名称
        String alias = "";          
        String password = "";

        public InputForm_keyStore(String keysotreName, String InfoPath)
        {
            InitializeComponent();

            this.InfoPath = InfoPath;
            this.keysotreName = keysotreName;

            label3.Text = "请输入" + keysotreName + "的签名信息";
            this.ControlBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            alias = textBox1.Text.Trim();
            password = textBox2.Text.Trim();

            String content = "";
            if (File.Exists(InfoPath)) content = FileProcess.fileToString(InfoPath);

            content += keysotreName + "(" + "alias(" + alias + ")" + " " + "password(" + password + ")" + ")" + keysotreName + "\r\n";
            FileProcess.SaveProcess(content, InfoPath);

            this.Close();
        }
    }
}
