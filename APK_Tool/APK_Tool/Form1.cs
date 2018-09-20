using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //判断拖来的是否是文件  
                e.Effect = DragDropEffects.Link;                //是则将拖动源中的数据连接到控件  
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Array files = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            if (files.Length > 0) textBox.Text = files.GetValue(0).ToString();
        }

        /// <summary>
        /// 判定执行对应逻辑
        /// </summary>
        private void file_TextChanged(object sender, EventArgs e)
        {
            if (Apktool.isApkFile(file.Text)) unPack.Text = "apk解包";     // 若为apk文件则，可解包
            else if (Apktool.isApkDir(file.Text)) unPack.Text = "apk打包"; // 若为apk解包文件夹，则可打包
            else unPack.Text = "执行cmd";

            comboBox_sign.Visible = Apktool.isApkDir(file.Text);           // 若为打包操作，可选择签名
        }

        /// <summary>
        /// 此函数用于实时显示cmd输出信息,在richTextBox1中显示输出信息
        /// </summary>
        private void OutPut(String line)
        {
            Apktool.OutPut(line, richTextBox1, this);
        }


        /// <summary>
        /// apk解包、打包
        /// </summary>
        private void unPack_Click(object sender, EventArgs e)
        {
            Apktool.clearDirInfo();
            Cmd.ThreadRun(UnPack_Logic, this, unPack, "执行中...");
        }

        private void UnPack_Logic()
        {
            OutPut("【I】");

            if (Apktool.isApkFile(file.Text))       // 解包
            {
                OutPut("【I】apk解包开始...");
                String result = Apktool.unPackage(file.Text, OutPut, false, false);   // 使用apktool进行apk的解包
                if (result.Contains("【E】")) return;
                OutPut("【I】apk解包结束！\r\n");
            }
            else if (Apktool.isApkDir(file.Text))   // 打包
            {
                OutPut("【I】apk打包开始...");
                String result = Apktool.package(file.Text, OutPut);     // 使用apktool进行打包
                if (result.Contains("【E】")) return;
                OutPut("【I】apk未签名文件已生成！\r\n");

                // 若有签名文件，则进行签名
                if(!comboBox_sign.Text.Equals(""))
                {
                    OutPut("【I】apk签名中...");
                    String apkName = file.Text + "..apk";
                    String pem = SinPath() + "\\" + comboBox_sign.Text + ".x509.pem";
                    String pk8 = SinPath() + "\\" + comboBox_sign.Text + ".pk8";
                    String psw = "letang123";
                    result = Apktool.Sign(apkName, pem, pk8, psw, OutPut);
                    if (result.Contains("【E】")) return;
                    OutPut("【I】apk签名、对齐 完成！\r\n");

                    // 删除打包生成的未签名文件
                    if (System.IO.File.Exists(apkName)) System.IO.File.Delete(apkName);
                }

                OutPut("【I】apk打包结束！\r\n");
            }
            else Cmd.Run(file.Text, OutPut);// 执行cmd命令  
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadSigns(comboBox_sign);
        }

        /// <summary>
        /// 获取签名文件所在目录
        /// </summary>
        public static String SinPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory + "tools\\signs";
        }

        /// <summary>
        /// 载入签名文件信息
        /// </summary>
        public static void loadSigns(ComboBox comboBox_sign)
        {
            string preStr = comboBox_sign.Text;     //记录之前选中的信息
            if (StartParam.AutoRun && !StartParam.SIGN.Equals("")) preStr = StartParam.SIGN;

            comboBox_sign.Items.Clear();

            //所有签名文件
            string[] files = System.IO.Directory.GetFiles(SinPath());
            foreach (string file in files)
            {
                if (file.EndsWith(".pk8"))
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(file);
                    comboBox_sign.Items.Add(name);
                }
                else if (file.EndsWith(".keystore"))
                {
                    string name = System.IO.Path.GetFileName(file);
                    comboBox_sign.Items.Add(name);
                }
            }

            // 默认选中签名文件letang
            int index = comboBox_sign.Items.IndexOf(preStr.Equals("") ? "120" : preStr);
            if (index == -1 && comboBox_sign.Items.Count > 0) index = 0;
            if (index != -1)comboBox_sign.SelectedIndex = index;
        }

    }
}
