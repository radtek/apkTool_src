using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = Tools.ConvertUnicodeStringToChinese(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = FileProcess.fileToString(textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileProcess.SaveProcess(textBox1.Text, textBox2.Text.Replace(".txt", "_.txt"));
        }


        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //判断拖来的是否是文件  
                e.Effect = DragDropEffects.Link;                //是则将拖动源中的数据连接到控件  
            else e.Effect = DragDropEffects.None;
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Array files = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            if (files.Length > 0) textBox.Text = files.GetValue(0).ToString();

            string fileSource = files.GetValue(0).ToString();
            string fileTarget = files.GetValue(1).ToString();
            string destFile = @"C:\Users\wangzhongyuan\Desktop\1\3.xml";
            xmlNode.Combine(fileSource, fileTarget, destFile, false, null, null);

            //string xmlSource = FileProcess.fileToString(textBox.Text);
            //string tmp = xmlSource;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String dir = textBox2.Text;
            if (!System.IO.Directory.Exists(dir)) return;

            //String package = "com.ltgame.xiyou.g44937", package2 = "com.ltgame_test.xiyou.g44937";
            String package = textBox3.Text, package2 = textBox4.Text;
            ReplaceDir_ChannelParams(dir, package.Replace(".", "/"), package2.Replace(".", "/"));
            ReplaceDir_ChannelParams(dir, package, package2);

            MessageBox.Show("替换所有包资源完成！");
        }

        /// <summary>
        /// 载入path对应的文件，并替换文件中所有key为value
        /// </summary>
        private void ReplaceFile_ChannelParams(String path, String key, String value, String Base_Dir = null)
        {
            if (File.Exists(path))
            {
                String data = FileProcess.fileToString(path);   // 获取文件内容

                if (data.Contains(key))
                {
                    // 替换文件中的所有关键字信息
                    data = data.Replace(key, value);
                    FileProcess.SaveProcess(data, path);           // 保存对文件的修改
                }
            }
        }

        /// <summary>
        /// 载入dirPath目录和其子目录下的所有文件，并替换文件中所有key为value
        /// </summary>
        public void ReplaceDir_ChannelParams(String dirPath, String key, String value, String Base_Dir = null)
        {
            //检查是否存在目的目录
            if (!Directory.Exists(dirPath)) return;

            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            FileInfo[] files = directoryInfo.GetFiles();

            //替换文件中所有文件的所有key值
            foreach (FileInfo file in files)
            {
                ReplaceFile_ChannelParams(file.FullName, key, value, Base_Dir);
            }

            //替换子目录中的所有文件中的key值
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                ReplaceDir_ChannelParams(dir.FullName, key, value, Base_Dir);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<xmlNode> nodes = xmlNode.Parse(textBox5.Text);
            String tmp = nodes[0].name;

            String Str = nodes[0].ToString();
            textBox5.Text = Str;   
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String path = ToolSetting.Instance().ProcessTmp_dir + "\\截屏.jpg";
            IconProcesser.getScreen(0,0,-1,-1,path);
            System.Diagnostics.Process.Start(path);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //String path = @"D:\sci\网游打包工具2\apk输出目录\1001\0000565\1001_0000565_1.1.00_20_downjoy.keystore_指挥官联盟_当乐网_v2.0.1\AndroidManifest2.xml";
            //Manifest manifest = new Manifest(path);                // 创建Manifest对象

            ////FileProcess.SaveProcess(xmlNode.ToString(list), outFile.Replace(".xml", "_缓存.xml"));

            //// 执行manifest修改逻辑
            //string[] cmd = { "get activity:com.downjoy.activity.SdkLoadActivity/meta-data:APP_ID set android:value=downjoy_appid@XXX}", 
            //                   "get activity:com.downjoy.activity.SdkActivity/intent-filter/data set android:scheme=dcnngsdk@XXXX}" 
            //               };

            String path = @"D:\sci\网游打包工具2\apk输出目录\1001\0002870\ltsdk_112_v2.0.0_hanfeng\assets\hfnsdk\config\sdkconf.xml";
            Manifest manifest = new Manifest(path);                // 创建Manifest对象
            string[] cmd = { "replace assets/hfnsdk/config/sdkconf.xml:landscape value=\"true\"  to landscape value=\"{is_landscape}\"" };

            manifest.runCMD(cmd);

            // 保存manifest
            manifest.save();
        }
    }
}
