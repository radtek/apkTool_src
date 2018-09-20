using APK_Tool.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APK_Tool
{
    class DependentFiles
    {
        /// <summary>
        /// 获取当前运行路径
        /// </summary>
        public static string curDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 检测目录是否存在，若不存在则创建
        /// </summary>
        public static void checkDir(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 保存Byte数组为文件
        /// </summary>
        public static void SaveFile(Byte[] array, string path, bool repalce=false)
        {
            if (repalce && System.IO.File.Exists(path)) System.IO.File.Delete(path);    // 若目标文件存在，则替换
            if (!System.IO.File.Exists(path))
            {
                // 创建输出流
                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

                //将byte数组写入文件中
                fs.Write(array, 0, array.Length);
                fs.Close();
            }
        }

        //=============================================

        /// <summary>
        /// 检测所有依赖文件，没有则生成
        /// </summary>
        public static void checksAll()
        {
            check("Newtonsoft.Json");
            check("cmd");
            check("apktool");
            check("signapk");
            check("signer");
            check("zipalign");
            check("Update");
            checkSigns();
            check("Empty.apk"); 
            check("ClearDrectory.exe");
        }

        /// <summary>
        /// 检测文件是否存在，不存在则创建
        /// </summary>
        public static void check(string name)
        {
            string tools = curDir() + "tools";
            checkDir(tools);
            tools += "\\";

            if (name.Equals("apktool")) SaveFile(Resources.apktool, tools + "apktool.jar");
            else if (name.Equals("signapk")) SaveFile(Resources.signapk, tools + "signapk.jar");
            else if (name.Equals("signer")) SaveFile(Resources.signer, tools + "signer.jar");
            else if (name.Equals("zipalign")) SaveFile(Resources.zipalign, tools + "zipalign.exe");
            else if (name.Equals("Newtonsoft.Json")) SaveFile(Resources.Newtonsoft_Json, curDir() + "Newtonsoft.Json.dll");
            else if (name.Equals("cmd")) SaveFile(Resources.cmd, tools + "cmd.exe");
            else if (name.Equals("Update")) SaveFile(Resources.Update, tools + "Update.exe", true);

            else if (name.Equals("Empty.apk")) SaveFile(Resources.Empty, tools + "Empty.apk", true);    // 空apk
            else if (name.Equals("ClearDrectory.exe")) SaveFile(Resources.ClearDirectory, tools + "ClearDrectory.exe");
        }

        /// <summary>
        /// 检查签名是否存在，不存在则创建
        /// </summary>
        public static void checkSigns()
        {
            string signs = curDir() + "\\tools\\signs";
            checkDir(signs);
            signs += "\\";

            SaveFile(Resources._120, signs + "120.pk8");
            SaveFile(Resources._120_x509, signs + "120.x509.pem");
            SaveFile(Resources.letang, signs + "letang.pk8");
            SaveFile(Resources.letang_x509, signs + "letang.x509.pem");
        }

        // "%~dp0Update.exe" "[CONFIG]https://git.oschina.net/joymeng/apkTool/raw/master/files/updateFiles.txt" "E:\tmp2\Update_Files\\" "渠道计费包\0000001\\"
        /// <summary>
        /// 调用Update.exe，更新以perfix为前缀的配置文件
        /// </summary>
        public static void updateFiles(string perfix)
        {
            string update_EXE = curDir() + "tools\\" + "Update.exe";
            //string url0 = "https://git.oschina.net/joymeng/apkTool/raw/master/files/updateFiles.txt";
            string url0 = Update.apkMd5Url;
            string url = "[CONFIG]" + url0;
            string path = ToolSetting.Instance().serverRoot;

            // 设置"渠道计费包\0000001\\"为默认渠道配置
            string configInfo = WebSettings.getWebData(url0);    // 获取更新配置信息 示例：scimence( Name1(6JSO-F2CM-4LQJ-JN8P) )scimence
            if (configInfo.Equals("")) return;
            if(!configInfo.Contains(perfix) && !configInfo.Contains(perfix.Replace("\\", "/")))
                perfix = @"渠道计费包/0000001/->" + perfix;

            url = AddQuotation(url);
            path = AddQuotation(path);
            perfix = AddQuotation(perfix);
            update_EXE = AddQuotation(update_EXE);

            // 调用更新插件执行软件更新逻辑
            String arg = url + " "  + path + " " + perfix;
            System.Diagnostics.Process.Start(update_EXE, arg);
        }

        /// <summary>
        /// 清除指定的目录或文件
        /// </summary>
        public static void clearDrectory(string path)
        {
            string EXE = curDir() + "tools\\" + "ClearDrectory.exe";

            // 调用更新插件,清除指定的目录
            String arg = AddQuotation(path);
            System.Diagnostics.Process.Start(EXE, arg);
        }

        /// <summary>
        /// 为arg添加引号
        /// </summary>
        private static string AddQuotation(string arg)
        {
            if (arg.EndsWith("\\") && !arg.EndsWith("\\\\")) arg += "\\";
            arg = "\"" + arg + "\"";

            return arg;
        }
    }
}
