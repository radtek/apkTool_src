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
    public partial class Form3 : Form
    {
        Dictionary<string, string> gameApkDic = new Dictionary<string, string>();
        Dictionary<string, string> channelApkDic = new Dictionary<string, string>();
        Dictionary<String, String> gameList = new Dictionary<string, string>();

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Form1.loadSigns(comboBox_sign); // 载入签名文件信息

            // 载入游戏裸包、和计费包列表
            ToolSetting settting = ToolSetting.Instance();

            gameApkDic = getApk_FileOrDir(settting.gameAPK_dir);
            loadApks(gameListBox, gameApkDic);

            channelApkDic = getApk_FileOrDir(settting.chargeAPK_dir);
            loadApks(channelListBox, channelApkDic);

            gameList = gameList_Data.getGameList(); // 获取游戏列表信息
            setGameList(comboBox_selectGame, gameList, 0, true);
        }

        /// <summary>
        /// 设置游戏列表信息
        /// </summary>
        public static void setGameList(ComboBox comboBox, Dictionary<String, String> gameList, int select = 0, bool all = false)
        {
            string preStr = comboBox.Text;     //记录之前选中的信息

            comboBox.Items.Clear();
            if(all) comboBox.Items.Add("（全部）");
            foreach(String gameId in gameList.Keys)
                comboBox.Items.Add(gameId + " " + gameList[gameId]);

            if (comboBox.Items.Contains(preStr)) comboBox.SelectedItem = preStr;
            else
            {
                if (select > 0 && comboBox.Items.Count > select) comboBox.SelectedIndex = select;
                else if (comboBox.Items.Count > 0) comboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 设置游戏列表信息
        /// </summary>
        public static void setChannelList(ListBox listBox, Dictionary<String, String> List, int select = 0, bool all = false)
        {
            listBox.Items.Clear();
            if (all) listBox.Items.Add("（全部）");
            foreach (String Id in List.Keys)
                listBox.Items.Add(Id + " " + List[Id]);
            
            if (select > 0 && listBox.Items.Count > select) listBox.SelectedIndex = select;
            else if (listBox.Items.Count > 0) listBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 从path目录下，载入apk文件信息显示到checklist中
        /// </summary>
        public static void loadApks(CheckedListBox checklist, Dictionary<string, string> dic)
        {
            checklist.Items.Clear();    // 清空

            foreach (string dir in dic.Keys)
            {
                checklist.Items.Add(dir);
            }
        }

        /// <summary>
        /// 获取路径path下的所有apk文件，或apk解包根目录信息
        /// </summary>
        public static Dictionary<string, string> getApk_FileOrDir(String path)
        {
            Dictionary<string, string> FileDir = new Dictionary<string, string>();

            // 若载入的为apk文件
            if (Apktool.isApkDir(path) || Apktool.isApkFile(path))
            {
                string name = System.IO.Path.GetFileName(path);
                FileDir.Add(name, path);
            }
            else
            {
                Dictionary<string, string> apks = getFileNameByExt(path, ".apk");         // 获取所有apk文件信息
                Dictionary<string, string> apkDirs = getApkDir(path);                     // 获取所有apk解包路径信息

                // 优先添加apk文件的解包目录
                foreach (string dir in apkDirs.Keys)
                {
                    FileDir.Add(dir, apkDirs[dir]);
                }

                // 再添加没有解包目录的apk文件
                foreach (string apk in apks.Keys)
                {
                    String dir = apk.Replace(".apk", "").TrimEnd('.');
                    if (!apkDirs.Keys.Contains(dir)) FileDir.Add(apk, apks[apk]);
                }
            }

            return FileDir;
        }

        
        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //判断拖来的是否是文件  
                e.Effect = DragDropEffects.Link;                //是则将拖动源中的数据连接到控件  
            else e.Effect = DragDropEffects.None;
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            CheckedListBox checklist = sender as CheckedListBox;
            Array files = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            if (files.Length > 0)
            {
                String file_dir = files.GetValue(0).ToString();
                loadApkDir(checklist, file_dir);
            }
        }

        // 拖拽载入的apk所在目录文件夹信息
        private void loadApkDir(CheckedListBox checklist, String file_dir)
        {
            // 获取文件所在目录
            if (File.Exists(file_dir))
            {
                file_dir = Path.GetDirectoryName(file_dir);
            }

            ToolSetting settting = ToolSetting.Instance();
            if (checklist == gameListBox)
            {
                settting.gameAPK_dir = file_dir;
                gameApkDic = getApk_FileOrDir(settting.gameAPK_dir);
                loadApks(gameListBox, gameApkDic);
            }
            else if (checklist == channelListBox)
            {
                settting.chargeAPK_dir = file_dir;
                channelApkDic = getApk_FileOrDir(settting.chargeAPK_dir);
                loadApks(channelListBox, channelApkDic);
            }
            settting.save();
        }


        /// <summary>
        /// 全选或全不选游戏包
        /// </summary>
        private void checkAllGame_CheckedChanged(object sender, EventArgs e)
        {
            selectAll(gameListBox, checkAllGame.Checked);
        }

        /// <summary>
        /// 全选或全不选渠道包
        /// </summary>
        private void checkAllChannel_CheckedChanged(object sender, EventArgs e)
        {
            selectAll(channelListBox, checkAllChannel.Checked);
        }

        /// <summary>
        /// 设置CheckedListBox的选中状态
        /// </summary>
        private void selectAll(CheckedListBox checkList, bool value)
        {
            for (int i = 0; i < checkList.Items.Count; i++)
            {
                checkList.SetItemChecked(i, value);
            }
        }

        /// <summary>
        /// 此函数用于实时显示cmd输出信息,在richTextBox1中显示输出信息
        /// </summary>
        private void OutPut(String line)
        {
            Apktool.OutPut(line, richTextBox1, this);
        }

        // ===========================================
        // 辅助功能函数

        /// <summary>
        /// 获取目录下，指定拓展名的所有文件名
        /// </summary>
        public static Dictionary<string, string> getFileNameByExt(string path, string extension)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (Directory.Exists(path))
            {
                //所有签名文件
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    if (file.EndsWith(extension))
                    {
                        string name = System.IO.Path.GetFileName(file);
                        dic.Add(name, file);
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 获取目录下，指定拓展名的所有文件名
        /// </summary>
        public static List<String> getFileNameListByExt(string path, string extension)
        {
            List<String> list = new List<String>();

            if (Directory.Exists(path))
            {
                //所有文件
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    if (file.EndsWith(extension))
                    {
                        list.Add(file);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取path目录下的所有apk解包目录信息
        /// </summary>
        public static Dictionary<string, string> getApkDir(string path)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                DirectoryInfo[] dirs = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    if (dir.Name.Equals("附加资源")) continue;    // 忽略附加资源目录

                    // 若为apk解包目录或文件，则直接添加
                    if (Apktool.isApkDir(dir.FullName))
                        dic.Add(dir.Name, dir.FullName);
                    // 若非apk解包目录，则检索其目录下的apk文件和子文件夹
                    else
                    {
                        Dictionary<string, string> listInner = getApk_FileOrDir(dir.FullName);
                        foreach (String subApk in listInner.Keys)
                        {
                            if(!dic.Keys.Contains(subApk)) dic.Add(subApk, listInner[subApk]);
                        }
                    }
                }
            }

            return dic;
        }


        /// <summary>
        /// 执行打包处理逻辑
        /// </summary>
        private void Combine_Click(object sender, EventArgs e)
        {
            ToolSetting settting = ToolSetting.Instance();  // 载入设置信息

            foreach (object iteamGame in gameListBox.CheckedItems)
            {
                String gameApkName = iteamGame.ToString();  // 游戏包名称， 如：1000_卡丁车最新通用包.apk
                String gameApk = gameApkDic[gameApkName];   // 游戏包路径
                String gameId = getNumber(gameApk);         // 提取游戏id
                Settings.gameId = gameId;

                foreach (object iteamChannel in channelListBox.CheckedItems)
                {
                    String chargeApkName = iteamChannel.ToString(); // 计费包名称， 如：0000694_lennovo_V2.6.1_float.apk
                    String chargeAPK = channelApkDic[chargeApkName];// 计费包路径
                    String channelId = getNumber(chargeAPK);        // 渠道id
                    Settings.channelId = channelId;

                    // 获取游戏对应的渠道参数
                    Settings.getChannelparams(gameId, channelId, OutPut); 

                    // 设置apk工具的输出目录、缓存文件目录、和输出文件名
                    Apktool.outputAPK_dir = settting.outputAPK_dir + "\\" + gameId;
                    String name = gameApkName.Replace(".apk", "").TrimEnd('.');
                    Apktool.outputAPK_name = gameId + "_" + channelId + "_" + comboBox_sign.Text + "_" + name + ".apk";
                    Apktool.ProcessTmp_dir = settting.ProcessTmp_dir;

                    // 执行apk打包混合逻辑
                    Form2.CombineApk(chargeAPK, gameApk, comboBox_sign.Text, OutPut);

                    Apktool.outputAPK_dir = "";
                    Apktool.outputAPK_name = "";
                    Apktool.ProcessTmp_dir = "";
                }
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

            if (iteam == 游戏目录ToolStripMenuItem) dir = settting.gameAPK_dir;
            if (iteam == 打开计费包目录ToolStripMenuItem) dir = settting.chargeAPK_dir;
            if (iteam == 打开缓存目录ToolStripMenuItem) dir = settting.ProcessTmp_dir;
            if (iteam == 打开输出目录ToolStripMenuItem) dir = settting.outputAPK_dir;

            if (!dir.Equals(""))
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                System.Diagnostics.Process.Start("explorer.exe", "/e, " + dir);
            }
        }


        //=======================================================
        // 提示信息
        //=======================================================

        /// <summary>
        /// 显示提示信息str,并选择是否打开文件目录Dir
        /// </summary>
        public void MessageWithOpen(String str, String Dir)
        {
            bool ok = (MessageBox.Show(str, "打开？", MessageBoxButtons.OKCancel) == DialogResult.OK);
            if (ok) System.Diagnostics.Process.Start("explorer.exe", "/e,/select, " + Dir);
        }


        /// <summary>
        /// 获取目录下的  APK_Base\游戏裸包\1002\* 或 APK_Base\游戏裸包\1002_*的数字部分
        /// </summary>
        public static String getNumber(String apkPath)
        {
            String id = "";
            if (apkPath.Contains("\\"))
            {
                int index = apkPath.LastIndexOf("\\");

                String name = apkPath.Substring(index + 1);     // 获取名称
                id = getNameNumber(name);                       // 从名称中获取id信息

                if (id.Equals(""))
                {
                    String path = apkPath.Substring(0, index);  // 获取所在目录
                    id = ToNumber(new DirectoryInfo(path).Name);// 从目录名中获取id信息  
                }
            }

            return id;
        }

        /// <summary>
        /// 获取 1002_*的数字部分
        /// </summary>
        public static String getNameNumber(String name)
        {
            String id = "";

            if (name.Contains("_")) id = name.Split('_')[0];
            else id = name;

            id = ToNumber(id);

            return id;
        }

        /// <summary>
        /// 提取str中的数字字符串部分
        /// </summary>
        public static String ToNumber(String str)
        {
            String Num = "";
            foreach (char C in str)
            {
                if ('0' <= C && C <= '9') Num += C;
            }

            return Num;
        }
    }
}
