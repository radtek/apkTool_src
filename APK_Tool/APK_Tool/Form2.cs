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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
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
            String files = dragDrop(e);
            textBox.Text = files;
        }

        public string dragDrop(DragEventArgs e)
        {
            StringBuilder filesName = new StringBuilder("");
            Array files = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            List<String> apkList = new List<string>();
            foreach (object iteam in files)
            {
                Dictionary<String, String> apk_dirs = Form3.getApk_FileOrDir(iteam.ToString());    // 获取目录下的apk文件或解压目录

                foreach(String apk in apk_dirs.Values.ToArray<String>())
                {
                    if (!apkList.Contains(apk)) apkList.Add(apk);
                }
            }

            return ToString(apkList);
       }

        // 转化Array为字符串形式
        private String ToString(List<String> A)
        {
            String tmp = "";
            foreach (object iteam in A)
                tmp += ";" + iteam.ToString();

            tmp = tmp.Substring(1);
            return tmp;
        }


        /// <summary>
        /// 当输入的路径均为apk的解包路径时，可以合并
        /// </summary>
        private void apkDir_TextChanged(object sender, EventArgs e)
        {
            //Combine.Enabled = (Apktool.isApkDir(dirDest.Text) || Apktool.isApkFile(dirDest.Text)) && 
            //    (Apktool.isApkDir(dirAdd.Text) || Apktool.isApkFile(dirAdd.Text));

            Combine.Enabled = isApkDir(dirDest.Text) && isApkDir(dirAdd.Text);
        }

        private bool isApkDir(String paths)
        {
            String[] A = paths.Split(';');
            foreach (String path in A)
                if (Apktool.isApkDir(path) || Apktool.isApkFile(path)) return true;

            return false;
        }

        /// <summary>
        /// 界面初始化载入
        /// </summary>
        private void Form2_Load(object sender, EventArgs e)
        {
            Form1.loadSigns(comboBox_sign); // 载入签名文件信息
        }

        /// <summary>
        /// 此函数用于实时显示cmd输出信息,在richTextBox1中显示输出信息
        /// </summary>
        private void OutPut(String line)
        {
            Apktool.OutPut(line, richTextBox1, this);
        }

        /// <summary>
        /// 执行apk解包文件混合逻辑
        /// </summary>
        private void Combine_Click(object sender, EventArgs e)
        {
            Apktool.clearDirInfo();
            Cmd.ThreadRun(Combine_Logic, this, Combine, "执行中...");
        }

        private void Combine_Logic()
        {
            String[] apkDest = dirDest.Text.Split(';');
            foreach (String pathDest in apkDest)
            {
                String[] apkAdd = dirAdd.Text.Split(';');
                foreach (String pathAdd in apkAdd)
                {
                    String Path = pathDest.Replace(".apk", "");
                    Apktool.outputAPK_name = Path.Substring(Path.LastIndexOf("\\")+1) + "_sign.apk";
                    CombineApk(pathAdd, pathDest, comboBox_sign.Text, OutPut, false);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Load(dirAdd.Text.Replace(".apk", "")); 
        }


        //==========================
        // apk解包打包逻辑封装

        /// <summary>
        /// 若为apk文件，则先行解包
        /// </summary>
        public static String apkUnpack(String apkFile, Cmd.Callback call, bool deletPublicXML = false)
        {
            // 若输入的为apk文件，则自动进行解包
            if (Apktool.isApkFile(apkFile))
            {
                if (call != null) call("【I】" + Path.GetFileName(apkFile));
                if (call != null) call("【I】apk解包开始...");

                string dir = Apktool.unPackage(apkFile, call, deletPublicXML);   // 使用apktool进行apk的解包
                if (dir.Contains("【E】")) return dir;

                if (call != null) call("【I】apk解包结束！\r\n");

                // 拷贝apk目录下的配置文件到解包文件所在目录
                String configTxt = apkFile.Replace(".apk", ".txt");
                String configTxt2 = apkFile.Replace(".apk", "-" + Settings.gameId + ".txt");    // 可按游戏获取游戏id对应的配置文件
                if (File.Exists(configTxt2) && !File.Exists(dir + ".txt"))
                {
                    File.Copy(configTxt2, dir + ".txt", false);
                }
                else if (File.Exists(configTxt) && !File.Exists(dir + ".txt"))
                {
                    File.Copy(configTxt, dir + ".txt", false);
                }

                if (File.Exists(dir + ".txt"))
                {
                    // 添加游戏附加配置信息到渠道包，打包配置中
                    String gameAttachConfig = ToolSetting.Instance().serverRoot + "游戏附加配置\\" + Settings.gameId + ".txt";
                    Settings.AppendSettingsTo(gameAttachConfig, dir + ".txt");
                }

                return dir;
            }
            else return apkFile;
        }

        /// <summary>
        /// 获取.keystore签名对应的alias和password信息
        /// </summary>
        public static String[] getKestoreInfo(String keysotreName)
        {
            String InfoPath = Form1.SinPath() + "\\" + "keystoreInfo.txt";
            string data = "";

            // 若keystore对应的信息记录文件不存在、或无此签名则提示输入并记录
            if (!File.Exists(InfoPath))
            {
                new InputForm_keyStore(keysotreName, InfoPath).ShowDialog();
            }
            else
            {
                data = FileProcess.fileToString(InfoPath);
                if (data.Equals("")) new InputForm_keyStore(keysotreName, InfoPath).ShowDialog();
            }
            
            // 读取keystoreName对应的签名配置信息
            data = FileProcess.fileToString(InfoPath);
            string SotreInfo = WebSettings.getNodeData(data, keysotreName, false);
            String alias = WebSettings.getNodeData(SotreInfo, "alias", true);
            String password = WebSettings.getNodeData(SotreInfo, "password", true);

            return new String[] { alias, password };
        }

        /// <summary>
        /// 执行apk打包逻辑
        /// </summary>
        public static String apkPackage(String apkUnPackageDir, String signFileName, Cmd.Callback call)
        {
            // 重新打包apk,并签名
            if (call != null) call("【I】apk打包开始...");
            String result = Apktool.package(apkUnPackageDir, call);     // 使用apktool进行打包
            if (result.Contains("【E】")) return result;

            if (call != null) call("【I】apk未签名文件已生成！\r\n");

            // 若有签名文件，则进行签名
            if (!signFileName.Equals(""))
            {
                if (call != null) call("【I】apk签名中...");
                String apkName = apkUnPackageDir + "..apk";

                if (signFileName.EndsWith(".keystore"))
                {
                    String keysotreName = Form1.SinPath() + "\\" + signFileName;

                    String[] I = getKestoreInfo(signFileName);  // 获取对应的签名信息
                    String alias = I[0];
                    String password = I[1];

                    result = Apktool.SignKeyStore(apkName, keysotreName, alias, password, call);
                }
                else
                {
                    String pem = Form1.SinPath() + "\\" + signFileName + ".x509.pem";
                    String pk8 = Form1.SinPath() + "\\" + signFileName + ".pk8";
                    String psw = "letang123";
                    result = Apktool.Sign(apkName, pem, pk8, psw, call);
                }
                if (result.Contains("【E】")) return result;

                if (call != null) call("【I】apk签名、对齐 完成！\r\n");

                // 删除打包生成的未签名文件
                if (File.Exists(apkName)) File.Delete(apkName);
            }

            if (call != null) call("【I】apk打包结束！\r\n");

            return "";
        }

        /// <summary>
        /// 添加游戏附加资源
        /// </summary>
        private static void addGameAttachs(ToolSetting toolSet, Cmd.Callback call, String dirTarget, String gameAttachDir, String Set_gameId, String Set_channelId, String tittle="")
        {
            // 获取选中的游戏包所在的目录的附加资源
            //toolSet.gameAPK_dir + "\\" + Settings.gameId + "\\"
            String selectGameDir = toolSet.gameAPK_dir + "\\" + Set_gameId;
            //String selectVersionDir = Form4.SelectVersionDir;
            //String[] attachDirs = { selectGameDir + "\\游戏Icon或Logo", selectVersionDir + "\\附加资源" };
            //foreach (String gameAttachDir0 in attachDirs)
            {
                String gameAttachDir2 = selectGameDir + gameAttachDir;     // 生成附加资源路径

                // 复制游戏附加目录中的文件
                List<String> list_channel = new List<string>();
                list_channel.Add("所有渠道");
                list_channel.Add(Set_channelId);

                bool isEmptyDir = ApkCombine.isEmptyDirectorty(gameAttachDir2);
                if (!isEmptyDir && call != null) call("【I】");
                if (!isEmptyDir && call != null) call("【I】复制，" + gameAttachDir + (tittle.Equals("") ? "" : ("到" + tittle)));
                foreach (String channelId in list_channel)
                {
                    String Dir = gameAttachDir2 + "\\" + channelId;
                    isEmptyDir = ApkCombine.isEmptyDirectorty(Dir);

                    // 拷贝游戏附加资源目录
                    if (channelId != null && !channelId.Equals("") && Directory.Exists(Dir) && !ApkCombine.isEmptyDirectorty(Dir))
                    {
                        if (!isEmptyDir && call != null) call("【I】复制，" + gameAttachDir + "\\" + channelId);
                        ApkCombine.CopyFolderTo(Dir, dirTarget, true, call);
                        if (!isEmptyDir && call != null) call("【I】复制，" + gameAttachDir + "\\" + channelId + " 完成");
                    }
                }
            }

            // 复制游戏Icon、Logo
            if (gameAttachDir.Equals("\\游戏Icon或Logo") && StartParam.AutoRun && Form4.channelIconDir.ContainsKey(Set_channelId))
            {
                String ICONDIR = Form4.channelIconDir[Set_channelId];
                if (!ICONDIR.Equals("") && Directory.Exists(ICONDIR)) ApkCombine.CopyFolderTo(ICONDIR, dirTarget + @"\res\drawable", true, call);
            }
        }

        /// <summary>
        /// 添加渠道附加资源
        /// </summary>
        private static void addChannelAttachs(ToolSetting toolSet, Cmd.Callback call, String dirSource, String channelAttachDir, String Set_channelId)
        {
            // 获取渠道附加目录资源路径
            String channelDir = toolSet.chargeAPK_dir + "\\" + Set_channelId;
            //String channelAttachDir = channelDir + "\\附加资源";
            String channelAttachDir2 = channelDir + channelAttachDir;

            // 复制渠道附加目录中的文件
            if (Directory.Exists(channelAttachDir2) && !ApkCombine.isEmptyDirectorty(channelAttachDir2))
            {
                bool isEmptyDir = ApkCombine.isEmptyDirectorty(channelAttachDir);

                if (!isEmptyDir && call != null) call("【I】");
                if (!isEmptyDir && call != null) call("【I】复制，" + channelAttachDir);
                ApkCombine.CopyFolderTo(channelAttachDir2, dirSource, true, call);
                if (!isEmptyDir && call != null) call("【I】复制，" + channelAttachDir + "完成");
            }
        }

        /// <summary>
        /// 执行apk解包文件混合逻辑
        /// </summary>
        public static void CombineApk(String dirSource, String dirTarget, String signFileName, Cmd.Callback call, bool deletTXT = true)
        {
            if (call != null) call("【I】");

            ToolSetting toolSet = ToolSetting.Instance();                               // 载入配置信息
            String Set_gameId = Settings.gameId, Set_channelId = Settings.channelId;    // 获取游戏id、渠道id

            //-------------------------1
            // 游戏apk解包
            dirTarget = apkUnpack(dirTarget, call/*, true*/);       // 解包游戏包并删除res\values\public.xml文件
            if (dirTarget.Contains("【E】")) return;
            

            //-------------------------2
            // 渠道apk解包
            dirSource = apkUnpack(dirSource, call);
            //ReplaceValues.ReplaceFileContent(dirSource, "0x7f0", "0x7ff");
            //if (call != null) call("【I】替换渠道解包文件下，所有0x7f0 为 0x7ff");
            if (dirSource.Contains("【E】")) return;
            

            // 添加游戏Icon或Logo到游戏解包文件
            addGameAttachs(toolSet, call, dirTarget, "\\游戏Icon或Logo", Set_gameId, Set_channelId, "游戏解包目录");

            // 添加游戏Icon或Logo到渠道解包文件
            addGameAttachs(toolSet, call, dirSource, "\\游戏Icon或Logo", Set_gameId, Set_channelId, "渠道解包目录");

            //-------------------------3
            if (call != null) call("【I】");
            if (call != null) call("【I】执行apk混合逻辑...");
            if (call != null) call("【I】添加" + dirSource + "\r\n到" + dirTarget + "中");

            // 进行apk文件的混合
            ApkCombine.Combine(dirSource, dirTarget, call);
            if (call != null) call("【I】apk混合完成！\r\n");

            //-------------------------4
            // 添加渠道附加资源
            addChannelAttachs(toolSet, call, dirTarget, "\\附加资源", Set_channelId);

            // 添加游戏包附加资源
            addGameAttachs(toolSet, call, dirTarget, "\\附加资源", Set_gameId, Set_channelId);

            //-------------------------5
            // 执行R文件修改逻辑
            if (toolSet.R_Process) R_process.Start(dirTarget, dirSource, call);

            //String path = R_process.create_publicXML(dirTarget, call);
            //String package = Settings.channel_param["package"];                     // 获取包名信息
            //R_process.rebuidR_smali(path, dirTarget + "\\smali", package, call);    // 重新生成包名路径下的R文件
            //File.Copy(path, dirTarget + @"\res\values\public.xml");                 // 替换原有public.xml

            //-------------------------6
            // apk重新打包
            String result = apkPackage(dirTarget, signFileName, call);
            if (result.Contains("【E】")) return;

            // 打包完成后清除缓存中的解包文件
            if (call != null) call("【I】清除缓存目录，" + dirSource);
            Apktool.DeletDir(dirSource);

            if (call != null) call("【I】清除缓存目录，" + dirTarget);
            Apktool.DeletDir(dirTarget);

            if (deletTXT)
            {
                if (call != null) call("【I】清除缓存文件，" + dirSource + ".txt");
                Apktool.DeletFile(dirSource + ".txt");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string xmlTarget = FileProcess.fileToString(dirDest.Text);
            xmlNode N = xmlNode.Parse(xmlTarget)[1];
            String tmp = "";

            List<String> cmds = new List<String>();
            cmds.Add("remove GAME_MAIN_ACTIVITY/intent-filter");
            cmds.Add("add THIS_MAIN_ACTIVITY to GAME_MAIN_ACTIVITY");
            cmds.Add("remove THIS_MAIN_ACTIVITY");
            xmlNode.Combine(dirAdd.Text, dirDest.Text, dirDest.Text.Replace(".xml", "_合并.xml"), true, cmds);
        }

    }
}
