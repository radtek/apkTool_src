using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    public partial class Form4 : Form
    {

        Dictionary<String, String> gameList = new Dictionary<string, string>(); // 游戏列表信息
        List<String> gameIds = new List<string>();                              // 存储所有游戏id信息

        ToolSetting settting = null;    // 配置信息
        String selectGameId = "";       // 选择的游戏id

        DirectoryInfo[] versionDirs = null; // 游戏对应所有版本目录
        String selectGameApk = "";      // 选择的游戏apk包路径


        Dictionary<String, String> channelList = new Dictionary<string, string>(); // 渠道列表信息
        List<String> channelIds = new List<string>();                              // 存储所有渠道id信息
        List<String> selectChannelIds = new List<string>();                        // 选中的渠道id信息
        Dictionary<String, String> selectChannelApks = new Dictionary<string, string>();            // 选中的所有渠道apk包或解包路径

        public static Dictionary<String, String> channelIconDir = new Dictionary<string, string>(); // 记录渠道号，Icon目录映射信息
        bool initLoading = true;        // 标识当前界面是否为初始载入
        public Form4()
        {
            InitializeComponent();
        }

        // 界面载入完成后，载入配置信息
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            load();
        }

        private void setStartButtonEnable(bool enable, String errorInfo)
        {
            Combine.Enabled = enable;
            Combine.Text = enable ? "打包" : errorInfo;
        }

        // 载入所需信息
        private void load()
        {
            ToolSetting.instance = null;                    // 请除原有配置信息
            settting = ToolSetting.Instance();              // 载入配置信息
            if (StartParam.AutoRun && !StartParam.OUTDIR.Equals(""))    // 设置apk输出目录
            {
                settting.outputAPK_dir = StartParam.OUTDIR;
            }

            Form1.loadSigns(comboBox_sign);                 // 载入签名文件信息

            gameList = gameList_Data.getGameList(OutPut);   // 获取游戏列表信息
            gameIds = gameList.Keys.ToList();               // 记录游戏id信息

            if (StartParam.AutoRun)
            {
                // 设置打包的游戏id
                int index = gameIds.Contains(StartParam.GAMEID) ? gameIds.IndexOf(StartParam.GAMEID) : 0;
                Form3.setGameList(comboBox_selectGame, gameList, index);
            }
            else Form3.setGameList(comboBox_selectGame, gameList);

            channelList = channelList_Data.getChannelList(OutPut);   // 获取游戏列表信息
            settting.AppendLocalChannels(channelList);      // 向渠道列表信息中添加本地的渠道列表信息

            channelIds = channelList.Keys.ToList();         // 记录渠道id信息

            setStartButtonEnable(gameList.Count > 0 && channelList.Count > 0, "网络异常");  // 设置打包按钮是否可用


            // 从左侧渠道列表中移除右侧已选中的渠道信息
            if (selectChannelIds.Count > 0)
            {
                foreach (string id in selectChannelIds)
                    if (channelIds.Contains(id)) channelIds.Remove(id);
            }

            //channelIds = getSubDirNames(settting.chargeAPK_dir);    // 获取所有计费渠道信息
            ShowChannelList(listBox2.Items.Count > 0);

            // 载入通用配置信息
            if (initLoading)
            {
                initLoading = false;

                DependentFiles.updateFiles("渠道计费包/通用配置.txt");
                DependentFiles.updateFiles("渠道计费包/所有渠道/");
                DependentFiles.updateFiles("游戏裸包/1000/v1.test/");
                DependentFiles.updateFiles("游戏附加配置/");

                if (StartParam.AutoRun)
                {
                    // 设置选择的渠道id
                    String[] channels = StartParam.CHANNELID.Replace('，', ',').Replace(';', ',').Replace('；', ',').Split(',');
                    String[] ICONDIRs = StartParam.ICONDIR.Replace('，', ',').Replace(';', ',').Replace('；', ',').Split(',');    // 渠道id对应的Icon

                    for(int i=0; i<channels.Length; i++)
                    {
                        String channel = channels[i];
                        if (channelIds.Contains(channel))
                        {
                            int index = channelIds.IndexOf(channel);
                            listBox1.SetSelected(index, true);
                        }

                        if (i < ICONDIRs.Length && !channelIconDir.ContainsKey(channel)) channelIconDir.Add(channel, ICONDIRs[i]);
                    }
                    button1_Click(null, null);
                }
            }

        }

        // 显示渠道列表信息
        private void ShowChannelList(bool load = true)
        {
            channelIds.Sort();
            selectChannelIds.Sort();

            setListBox(listBox1, ToChannelInfo(channelIds).ToArray(), -1);
            setListBox(listBox2, ToChannelInfo(selectChannelIds).ToArray(), -1);

            if (load) loadChannelApks();  // 载入所有选择渠道的计费文件信息
        }

        // 转化channelId为对应形式的信息串
        private List<String> ToChannelInfo(List<String> Ids)
        {
            List<String> list = new List<String>();
            foreach (String id in Ids)
                list.Add(id + " " + channelList[id]);
            return list;
        }

        // 载入所有选择渠道的计费文件信息
        private void loadChannelApks()
        {
            selectChannelApks.Clear();

            OutPut("【I】 载入所有选择渠道的计费文件信息...");
            foreach (String channelId in selectChannelIds)
            {
                String path = settting.chargeAPK_dir + "\\" + channelId;
                ToolSetting.confirmDir(path);

                Dictionary<string, string> dic = Form3.getApk_FileOrDir(path);
                if (dic.Count > 0)
                {
                    String channelApk = dic.ElementAt(0).Value;
                    selectChannelApks.Add(channelId, channelApk);
                    OutPut("【I】 渠道" + channelId + "_" + channelList[channelId] + " ,存在计费文件:\r\n" + channelApk);
                }
                else
                {
                    OutPut("【E】 渠道" + channelId + "_" + channelList[channelId] + " ,无计费包！请添加渠道计费文件至目录:\r\n" + path);
                }
            }
            OutPut("【I】 载入选择渠道的计费文件信息完成");
            
        }


        delegate void OutPut_Call(String line);

        /// <summary>
        /// 此函数用于实时显示cmd输出信息,在richTextBox1中显示输出信息
        /// </summary>
        private void OutPut(String line)
        {
            Apktool.OutPut(line, richTextBox1, this);
        }


        // 选择游戏包变动
        private void comboBox_selectGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox_selectGame.SelectedIndex;
            if (index != -1)
            {
                selectGameId = gameIds[index];  // 选择游戏时，记录对应的游戏id信息

                // 获取游戏裸包下的对应游戏目录，没有则创建
                String gameDir = settting.gameAPK_dir + "\\" + selectGameId;
                ToolSetting.confirmDir(gameDir);

                // 载入游戏目录下的版本目录信息
                DirectoryInfo[] dirs = getDirectories(gameDir);
                if (dirs.Length == 0)    // 若版本目录为空，则创建
                {
                    ToolSetting.confirmDir(gameDir + "\\v1.0.0");
                    dirs = getDirectories(gameDir);
                }
                versionDirs = dirs;

                // 获取所有版本目录，忽略 游戏Icon或Logo 目录
                List<String> ingnore = new List<string>();
                ingnore.Add("游戏Icon或Logo");
                ingnore.Add("附加资源");
                String[] names = DirNames(dirs, ingnore).ToArray();
                setCombox(comboBox_version, names);
            }
        }


        // 设置combox的显示内容，并默认选中第select项
        public static void setCombox(ComboBox combox, String[] iteams, int select = 0)
        {
            string preStr = combox.Text;     //记录之前选中的信息

            combox.Items.Clear();
            foreach (String iteam in iteams)
            {
                combox.Items.Add(iteam);
            }

            if (combox.Items.Contains(preStr)) combox.SelectedItem = preStr;    // 默认选择之前选中的版本目录信息
            else
            {
                if (select >= 0 && combox.Items.Count > 0 && select < combox.Items.Count)
                    combox.SelectedIndex = select;
            }
        }

        // 设置combox的显示内容，并默认选中第select项
        public static void setListBox(ListBox listBox, String[] iteams, int select = 0)
        {
            listBox.Items.Clear();
            foreach (String iteam in iteams)
            {
                listBox.Items.Add(iteam);
            }

            if (select >= 0 && listBox.Items.Count > 0 && select < listBox.Items.Count)
                listBox.SelectedIndex = select;
        }

        // 获取dirPath下的子目录信息
        public static DirectoryInfo[] getDirectories(String dirPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            return directoryInfo.GetDirectories();
        }

        // 获取目录名称, 并忽略ingnore中指定的所有项
        public static List<String> DirNames(DirectoryInfo[] dirs, List<String> ingnore = null)
        {
            List<String> name = new List<string>();
            foreach (DirectoryInfo dir in dirs)
            {
                if (ingnore != null && ingnore.Contains(dir.Name)) continue;

                name.Add(dir.Name);
            }

            return name;
        }

        // 获取目录下的所有子目录信息
        public static List<String> getSubDirNames(String dirPath)
        {
            DirectoryInfo[] dirs = getDirectories(dirPath);
            List<String> name = new List<string>();

            if (dirs != null && dirs.Length > 0)
            {
                foreach (DirectoryInfo dir in dirs)
                    name.Add(dir.Name);
            }

            return name;
        }

        public static String SelectVersionDir = "";      // 选中的游戏裸包对应的版本目录
        public bool versionNextUpdate = false;           // 忽略一次刷新逻辑
        public bool showLoadApkInfo = true;
        // 选择版本变动
        private void comboBox_version_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (versionNextUpdate)
            {
                versionNextUpdate = false;
                return;
            }

            int index = comboBox_version.SelectedIndex;
            if (index != -1)
            {
                // 选取的版本目录
                String VersionDir = versionDirs[index].FullName;
                SelectVersionDir = VersionDir;

                // 选取目录下的第一个apk文件
                List<String> apks = Form3.getFileNameListByExt(VersionDir, ".apk");
                if (apks.Count > 0) linkLabel_gameApk.Text = apks[0];
                else linkLabel_gameApk.Text = VersionDir;
                
                // 设置游戏裸包
                if (StartParam.AutoRun) linkLabel_gameApk.Text = StartParam.APK;

                if (linkLabel_gameApk.Text.EndsWith(".apk"))
                {
                    selectGameApk = linkLabel_gameApk.Text;
                    if(showLoadApkInfo) OutPut("【I】 载入游戏裸包 " + Path.GetFileName(linkLabel_gameApk.Text));
                }
                else
                {
                    selectGameApk = "";
                    if (showLoadApkInfo) OutPut("【E】 没有裸包！请点击“+”按钮，添加游戏裸包");
                }

                if (!showLoadApkInfo) showLoadApkInfo = true;
            }
        }

        /// <summary>
        /// 打开裸包所在目录，或在文件浏览器中查看裸包
        /// </summary>
        private void linkLabel_gameApk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String name = linkLabel_gameApk.Text;
            ShowFileInExplorer(name);
        }

        /// <summary>
        /// 在文件浏览器中显示指定的文件
        /// </summary>
        public static void ShowFileInExplorer(String file)
        {
            if (File.Exists(file))
            {
                if (file.EndsWith(".apk")) System.Diagnostics.Process.Start("explorer.exe", "/e,/select, " + file);
                else System.Diagnostics.Process.Start("explorer.exe", "/e, " + file);
            }
        }

        // 选择打包渠道
        private void button1_Click(object sender, EventArgs e)
        {
            move(channelIds, selectChannelIds, listBox1.SelectedIndices, true);
            ShowChannelList(false);
            //loadChannelApks();  // 载入所有选择渠道的计费文件信息

            // 定时器中，循环等待渠道计费包下载更新完成
            if (!timerDownload.Enabled)
            {
                Combine.Enabled = false;
                Combine.Text = "载入中...";

                timerDownload.Start();
            }
        }

        // 延时判断选中的渠道计费包是否下载完成，待下载完成后打包按钮可用
        private void timerDownload_Tick(object sender, EventArgs e)
        {
            // 获取Update.exe后台进程
            System.Diagnostics.Process[] processes1 = System.Diagnostics.Process.GetProcessesByName("Update");

            // 无更新进程Update.exe，表示更新完成
            if (processes1 == null || processes1.Length == 0)
            {
                timerDownload.Stop();   // 不再执行计费文件更新检测逻辑
                loadChannelApks();      // 载入所有选择渠道的计费文件信息

                Combine.Enabled = true;
                Combine.Text = "打包";

                if (StartParam.AutoRun) Combine_Click(null, null);  // 执行打包
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            move(selectChannelIds, channelIds, listBox2.SelectedIndices);
            ShowChannelList();
        }

        // 将Source中的选中项添加到Target中
        private void move(List<String> Source, List<String> Target, ListBox.SelectedIndexCollection collection, bool updateFile = false)
        {
            List<String> iteams = new List<string>();
            foreach (int index in collection)
            {
                if (!Target.Contains(Source[index]))
                {
                    Target.Add(Source[index]);

                    // 更新渠道包配置信息到本地
                    if(updateFile) DependentFiles.updateFiles("渠道计费包/" + Source[index] + "/");
                }
                iteams.Add(Source[index]);
            }

            foreach (String iteam in iteams)
            {
                Source.Remove(iteam);
            }
        }

        /// <summary>
        /// 界面在执行打包时，锁定界面中的部分功能，防止误操作
        /// </summary>
        private void FormFunctionLock(bool enable)
        {
            刷新ToolStripMenuItem.Enabled = enable;
            comboBox_selectGame.Enabled = enable;
            comboBox_version.Enabled = enable;
            button1.Enabled = enable;
            button2.Enabled = enable;
            richTextBox1.ReadOnly = !enable;
            listBox2.Enabled = enable;
            pictureBox_addGameApk.Enabled = enable;
        }

        /// <summary>
        /// 对选择的游戏包和选择的所有渠道，执行打包
        /// </summary>
        private void Combine_Click(object sender, EventArgs e)
        {
            //String dirTarget = @"D:\sci\网游打包工具2\工具缓存目录\奥特曼OL无推送底包12.22_@_2016_12_26_09.44.33";
            //String packageName = "com.ltgame.atmollt.leshi";
            ////R_process.getRes_typeList(dirTarget, packageName);

            //R_process.generateResTypeList = R_process.getRes_typeList(dirTarget, packageName);          // 获取新的包名路径下，现有的R文件类型信息
            //R_process.CreateR_smali(R_process.generateResTypeList, dirTarget + "\\smali", packageName, OutPut);     // 根据现有的R资源类型生成对应R.smali文件

            //String dirTarget = @"E:\SVN\workSpace\apkTool\files\APK_Base\工具缓存目录\ltsdk_56_leshi_v2.2.1";
            //R_process.appendunknown(dirTarget, OutPut); 

            //Combine_Logic();


            //String dir = @"C:\Users\wangzhongyuan\Desktop\vtlanmen\1000_0002643_1\smali";
            //R_process.getR_smaliPath(dir);

            //String dirTarget = "D:\\sci\\网游打包工具2\\工具缓存目录\\unity1_@_2017_01_24_10.43.58\\smali\\android\\support\\v7\\appcompat";
            //R_process.getRes_typeList(dirTarget);

            //string ctXML = @"C:\Users\wangzhongyuan\Desktop\电信_移动\AndroidManifestCT.xml";
            //string unityXML = @"C:\Users\wangzhongyuan\Desktop\电信_移动\AndroidManifestUnity.xml";
            //string outputXML = @"C:\Users\wangzhongyuan\Desktop\电信_移动\AndroidManifestOutput.xml";
            //xmlNode.Combine(ctXML, unityXML, outputXML, true, null, null);

            //string ctXML = @"C:\Users\wangzhongyuan\Desktop\电信_移动\AndroidManifestCT2.xml";
            //string outputXML = @"C:\Users\wangzhongyuan\Desktop\电信_移动\AndroidManifestOutput.xml";
            //xmlNode.ParseTest(ctXML, outputXML);

            Cmd.ThreadRun(Combine_Logic, this, Combine, "打包中...");  // 在新的线程中执行打包


            //String publicXML = @"D:\sci\网游打包工具2\工具缓存目录\public_空工程生成.xml";
            //String targetDir = @"D:\sci\网游打包工具2\工具缓存目录\base";
            //R_process.rebuidR_smali(publicXML, targetDir, "com.game.test.pac", OutPut);
        }

        //delegate void Combine_LogicCall();

        // apk混合逻辑
        private void Combine_Logic()
        {
            //if (this.InvokeRequired)
            //{
            //    Combine_LogicCall F = new Combine_LogicCall(Combine_Logic);
            //    this.BeginInvoke(F);
            //}
            //else
            {
                // 打包前载入游戏裸包信息
                showLoadApkInfo = false;
                comboBox_version_SelectedIndexChanged(null, null);
                if (!File.Exists(selectGameApk))
                {
                    OutPut("【E】 游戏裸包不存在，请先添加游戏裸包");
                    return;
                }

                FormFunctionLock(false);

                String gameName = gameList[selectGameId];   // 游戏名称
                String packageErrorMessage = "";
                if (selectChannelIds.Count == 0) MessageBox.Show("请选择要打包的游戏渠道！");
                foreach (String channelId in selectChannelIds)
                {
                    try
                    {
                        if (!selectChannelApks.Keys.Contains(channelId)) continue;

                        Settings.gameId = selectGameId;     // 记录当前的游戏id
                        Settings.channelId = channelId;     // 记录当前的渠道id
                        String chargeAPK = selectChannelApks[channelId];
                        String channelName = channelList[channelId];

                        OutPut("");
                        OutPut("【T】" + "-----------------------------------------------------------");
                        OutPut("【T】" + selectGameId + "_" + channelId + "_" + gameName + "_" + channelName);
                        OutPut("【T】" + "-----------------------------------------------------------");
                        if (!File.Exists(chargeAPK))
                        {
                            OutPut("【E】 当前渠道无计费包，暂不支持打包");
                            continue;
                        }

                        // 获取游戏对应的渠道参数
                        Settings.getChannelparams(selectGameId, channelId, OutPut);
                        Dictionary<String, String> param = Settings.channel_param;
                        String versionName = "", versionCode = "";

                        // 设置版本名称和版本号
                        if (param.Keys.Contains("version_name") && param.Keys.Contains("version_code"))
                        {
                            if (!textBox_versionName.Text.Equals(""))
                            {
                                if (param.Keys.Contains("version_name")) param["version_name"] = textBox_versionName.Text.Trim();
                                else param.Add("version_name", textBox_versionName.Text.Trim());
                            }

                            if (!textBox_versionCode.Text.Equals(""))
                            {
                                if (param.Keys.Contains("version_code")) param["version_code"] = textBox_versionCode.Text.Trim();
                                else param.Add("version_code", textBox_versionCode.Text.Trim());
                            }
                            versionName = param["version_name"];
                            versionCode = param["version_code"];
                        }
                        else continue;


                        // 设置apk工具的输出目录、缓存文件目录、和输出文件名
                        Apktool.outputAPK_dir = settting.outputAPK_dir + "\\" + selectGameId + "\\" + channelId;
                        ToolSetting.confirmDir(Apktool.outputAPK_dir);  // 确保输出目录存在
                        Apktool.outputAPK_name = selectGameId + "_" + channelId + "_" + versionName + "_" +
                            versionCode + "_" + comboBox_sign.Text + "_" + gameName + "_" + channelName + "_" + comboBox_version.Text.Trim() + ".apk";

                        if (Apktool.outputAPK_name.Contains(":") || Apktool.outputAPK_name.Contains("："))
                        {
                            Apktool.outputAPK_name = Apktool.outputAPK_name.Replace(":", "_").Replace("：", "_");
                        }
                         
                        Apktool.ProcessTmp_dir = settting.ProcessTmp_dir;


                        // 执行apk打包混合逻辑
                        Form2.CombineApk(chargeAPK, selectGameApk, comboBox_sign.Text, OutPut);

                        Apktool.outputAPK_dir = "";
                        Apktool.outputAPK_name = "";
                        Apktool.ProcessTmp_dir = "";
                    }
                    catch (Exception ex) { packageErrorMessage += "\r\n" + ex.Message + "\r\n"; }

                    // 打完一个包后等待3秒
                    //Thread.Sleep(3000);
                }

                if (!packageErrorMessage.Equals("")) MessageBox.Show("打包异常信息：" + packageErrorMessage);

                OutPut("【T】" + "-------------------------打包结束------------------------");

                if (StartParam.AutoRun)
                {
                    //Application.Exit(); // 打包结束自动退出
                    System.Environment.Exit(0);
                }
                FormFunctionLock(true);
            }
        }

        // 检索
        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            String text = textBox_search.Text;
            if (text.Equals("")) return;

            List<String> list = ToChannelInfo(channelIds);
            for (int i = list.Count-1; i >= 0; i--)
            {
                //bool march = (list[i].StartsWith(text) || list[i].Contains(" " + text));
                bool march = (list[i].Contains(text));
                listBox1.SetSelected(i, march);
            }
        }

        /// <summary>
        /// 打开指定的目录
        /// </summary>
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem iteam = sender as ToolStripMenuItem;
            String dir = "";
            ToolSetting settting = ToolSetting.Instance();  // 载入设置信息

            if (iteam == 裸包目录ToolStripMenuItem) dir = settting.gameAPK_dir;
            if (iteam == 渠道目录ToolStripMenuItem) dir = settting.chargeAPK_dir;
            if (iteam == 缓存目录ToolStripMenuItem) dir = settting.ProcessTmp_dir;
            if (iteam == 输出目录ToolStripMenuItem) dir = settting.outputAPK_dir;

            if (!dir.Equals(""))
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                System.Diagnostics.Process.Start("explorer.exe", "/e, " + dir);
            }
        }

        private void 打包解包工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new Form1().Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString());  }
        }

        private void apk混合工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new Form2().Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void uToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new Form5().Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void 游戏Icon或LogoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String selectGameDir = settting.gameAPK_dir + "\\" + selectGameId + "\\游戏Icon或Logo";

            List<String> list = new List<string>();

            String gameName = selectGameId + "_" + gameList[selectGameId];
            String tittle = "为游戏“" + gameName + "”添加Icon或Logo";
            DialogResult result = MessageBox.Show(this, "添加的Icon或Logo，是否用于所有渠道？\r\n\r\n否，用于选中的渠道\r\n取消，不操作", tittle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.Yes))
            {
                list.Add("所有渠道");
            }
            else if (result.Equals(DialogResult.No))
            {
                if (selectChannelIds.Count == 0)
                {
                    MessageBox.Show(this, "您当前尚未选择任何渠道！\r\n请先选择需要操作的渠道，再进行此操作", tittle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else foreach (String channelId in selectChannelIds) list.Add(channelId);
            }
            else if (result.Equals(DialogResult.Cancel)) return;

            foreach (String channelId in list)
            {
                String Dir = selectGameDir + "\\" + channelId + "\\res\\drawable";
                ToolSetting.confirmDir(Dir);
                System.Diagnostics.Process.Start("explorer.exe", "/e, " + Dir);
            }

            showTip(list.Count, tittle, "请在当前弹出的", "目录下，", "添加游戏Icon或Logo \r\n\r\n如：\r\nGAME_ICON.png 游戏icon \r\nGAME_LOGO.png 游戏logo");
            
        }

        private void 添加游戏附加资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SelectVersionDir.Equals(""))
            {
                // 获取选中的游戏包所在的目录的附加资源
                //String attachDir = SelectVersionDir + "\\附加资源";
                String attachDir = settting.gameAPK_dir + "\\" + selectGameId + "\\附加资源";

                List<String> list = new List<string>();

                String gameName = selectGameId + "_" + gameList[selectGameId];
                String tittle = "为游戏“" + gameName + "”添加附加资源";
                DialogResult result = MessageBox.Show(this, "添加的资源，是否用于所有渠道？\r\n\r\n否，用于选中的渠道\r\n取消，不操作", tittle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result.Equals(DialogResult.Yes))
                {
                    list.Add("所有渠道");
                }
                else if (result.Equals(DialogResult.No))
                {
                    if (selectChannelIds.Count == 0)
                    {
                        MessageBox.Show(this, "您当前尚未选择任何渠道！\r\n请先选择需要操作的渠道，再进行此操作", tittle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else foreach (String channelId in selectChannelIds) list.Add(channelId);
                }
                else if (result.Equals(DialogResult.Cancel)) return;

                foreach (String channelId in list)
                {
                    String Dir = attachDir + "\\" + channelId;
                    ToolSetting.confirmDir(Dir);
                    System.Diagnostics.Process.Start("explorer.exe", "/e, " + Dir);
                }

                showTip(list.Count, tittle, "请在当前弹出的", "目录下，", "添加游戏附加资源\r\n\r\n如：res\\drawable-hdpi-v4\\icon.png");
                
            }
        }

        private void 渠道附加资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<String> list = new List<string>();
            ToolSetting.confirmDir(settting.chargeAPK_dir + "\\所有渠道\\附加资源");

            String tittle = "添加，渠道附加资源";

            if (selectChannelIds.Count == 0)
            {
                //list.Add("所有渠道");
                MessageBox.Show("您当前尚未选择任何渠道！\r\n请先选择需要操作的渠道，再进行此操作", tittle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                foreach (String channelId in selectChannelIds) list.Add(channelId);

                foreach (String channelId in list)
                {
                    String Dir = settting.chargeAPK_dir + "\\" + channelId + "\\附加资源";
                    ToolSetting.confirmDir(Dir);
                    System.Diagnostics.Process.Start("explorer.exe", "/e, " + Dir);
                }

                showTip(list.Count, tittle, "请在当前弹出的", "目录下，", "添加渠道附加资源\r\n\r\n如：res\\drawable-hdpi-v4\\icon.png");
            }
        }

        // 自动判定显示对应的提示信息
        private void showTip(int count, String tittle, String text1, String text2, String text3)
        {
            String info1 = count > 1 ? count + "个" : "";
            String info2 = count > 1 ? "分别" : "";
            MessageBox.Show(this, text1 + info1 + text2 + info2 + text3, tittle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 打开选择的游戏对应目录
        private void label_gamList_Click(object sender, EventArgs e)
        {
            int index = comboBox_selectGame.SelectedIndex;
            if (index != -1)
            {
                selectGameId = gameIds[index];  // 选择游戏时，记录对应的游戏id信息
                String gameDir = settting.gameAPK_dir + "\\" + selectGameId;
                System.Diagnostics.Process.Start("explorer.exe", "/e, " + gameDir);
            }
        }

        // 打开选择的游戏对应版本目录
        private void label_version_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/e, " + SelectVersionDir);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/e, " + Form1.SinPath());
        }


        private void 添加游戏apk包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new HelpForm(Properties.Resources.添加游戏apk包).Show();
        }

        private void 打包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new HelpForm(Properties.Resources.网游打包).Show();
        }

        private void 游戏Icon_Logo_MenuItem_Click(object sender, EventArgs e)
        {
            new HelpForm(Properties.Resources.添加游戏Icon_Logo).Show();
        }


        private void 渠道计费包_Item_Click(object sender, EventArgs e)
        {
            new HelpForm(Properties.Resources.添加渠道计费包).Show();
        }

        private void 游戏附加资源_Item_Click(object sender, EventArgs e)
        {
            new HelpForm(Properties.Resources.游戏和渠道附加资源).Show();
        }

        private void 渠道计费包参数配置_Item_Click(object sender, EventArgs e)
        {
            new HelpForm(Properties.Resources.渠道计费包参数配置).Show();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);    //识别出超链接，点击后可以访问
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            timer1.Enabled = true;
        }

        # region 添加渠道计费包

        String selectChannelPath = "";
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox2.SelectedIndex;
            if (index < 0) return;

            String channelId = selectChannelIds[index];

            String path = settting.chargeAPK_dir + "\\" + channelId;
            selectChannelPath = path; 
            ToolSetting.confirmDir(path);

            Dictionary<string, string> dic = Form3.getApk_FileOrDir(path);
            if (dic.Count == 0)
            {
                label_addApk.Text = "当前选中的渠道无计费文件\r\n" + channelId + "_" + channelList[channelId];
                panel_addApk.Visible = true;
            }
            else
            {
                panel_addApk.Visible = false;
                textBox_addApk.Text = "拖动渠道计费包至此";
            }
        }

        // 添加选中的渠道apk包，到对应渠道计费apk目录
        private void button_addApk_Click(object sender, EventArgs e)
        {
            String file = textBox_addApk.Text;
            if (file.Equals("") || !Directory.Exists(selectChannelPath)) return;

            String fileName = file.Substring(file.LastIndexOf("\\") + 1);   // 获取文件或目录名称
            bool isChannelApk_dir = true;
            if (file.EndsWith(".apk") && File.Exists(file))
            {
                // 复制apk文件
                File.Copy(file, selectChannelPath + "\\" + fileName);
            }
            else if (Directory.Exists(file) && Apktool.isApkDir(file))
            {
                // 复制apk目录
                ApkCombine.CopyFolderTo(file, selectChannelPath + "\\" + fileName, true);
            }
            else isChannelApk_dir = false;


            if (isChannelApk_dir)
            {
                // 为复制的渠道表添加通用配置文件
                String Name = fileName.Replace(".apk", "").TrimEnd('.');                // 获取文件名称
                String configPath = selectChannelPath + "\\" + Name + ".txt";           // 生成对应的配置文件名
                DependentFiles.SaveFile(Properties.Resources.CommonConfig, configPath); // 写入通用配置信息

                // 打开生成的渠道配置文件
                if (File.Exists(configPath)) System.Diagnostics.Process.Start("explorer.exe", "/e,/select, " + configPath);

                panel_addApk.Visible = false;
                ShowChannelList();
            }
            else MessageBox.Show("请拖入渠道计费apk或apk的解包目录！");
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
        }

        # endregion

        private void form3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().Show();
        }

        private void 渠道通用配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String path = settting.ProcessTmp_dir + "\\通用配置.txt";
            if (!File.Exists(path)) DependentFiles.SaveFile(Properties.Resources.CommonConfig, path);
            System.Diagnostics.Process.Start(path);
        }

        private void aPKTool配置说明txtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String path = settting.ProcessTmp_dir + "\\APK_Tool配置说明.txt";
            if (!File.Exists(path)) DependentFiles.SaveFile(Properties.Resources.tipInfo, path);
            System.Diagnostics.Process.Start(path);
        }

        private void 工具配置信息txtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ToolSetting.filePath);
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (StartParam.AutoRun) return;

            if (!Combine.Enabled)
            {
                MessageBox.Show("打包正在进行中，请等待打包结束后再退出！");
                e.Cancel = true;
                
                return;
            }
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 退出工具后，自动清理工具缓存目录
            APK_Tool.DependentFiles.clearDrectory(ToolSetting.Instance().ProcessTmp_dir);
        }

        private void 网游后台ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://netunion.joymeng.com/index.php");
        }

        private void pictureBox_addGameApk_Click(object sender, EventArgs e)
        {
            panel_addGameApk.Visible = true;
            textBox_addGameApk.Text = "请拖动游戏裸包至此";
        }

        // 生成新的游戏版本目录
        private String New_versionDir(String versionName)
        {
            // 获取游戏裸包下的对应游戏目录，没有则创建
            String gameDir = settting.gameAPK_dir + "\\" + selectGameId + "\\" + versionName;
            //if (!ApkCombine.isEmptyDirectorty(gameDir)) Directory.Delete(gameDir, true);
            ToolSetting.confirmDir(gameDir);

            return gameDir;
        }

        // 将当前时间创建版本名
        private String versionString(DateTime time)
        {
            return "v" + time.ToString("MMdd.") + ((time.Hour < 10 ? "0" : "") + time.Hour) + time.ToString("mm");
        }

        /// <summary>
        /// 添加游戏裸包
        /// </summary>
        private void button_addGameApk_Click(object sender, EventArgs e)
        {
            String file = textBox_addGameApk.Text;

            //String versionName = "v" + DateTime.Now.ToString("MMdd.hhmm.ss");
            String versionName = versionString(DateTime.Now);
            String gameApkDir = New_versionDir(versionName);            // 生成新的游戏裸包版本目录

            if (file.EndsWith(".apk") && File.Exists(file))
            {
                panel_addGameApk.Visible = false;                       // 隐藏添加游戏裸包

                String fileName = file.Substring(file.LastIndexOf("\\") + 1);   // 获取文件或目录名称
                File.Copy(file, gameApkDir + "\\" + fileName, true);    // 复制apk文件

                // 选择新的版本
                versionNextUpdate = true;
                comboBox_selectGame_SelectedIndexChanged(null, null);   // 载入新的版本路径信息
                comboBox_version.SelectedItem = versionName;            // 选中新的版本
            }
            else
            {
                String tittle = "添加新的游戏裸包";
                MessageBox.Show(this, "您添加的“" + file + "”不是有效的游戏裸包\r\n请添加游戏裸包，如 D:\\卡丁车.apk", tittle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 隐藏添加裸包pannel
        private void label_addGameApk_Click(object sender, EventArgs e)
        {
            panel_addGameApk.Visible = false;
        }

        // 设置其它菜单栏可用
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control) 其它ToolStripMenuItem.Visible = !其它ToolStripMenuItem.Visible;
        }

        

    }
}
