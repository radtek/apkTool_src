using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    /// <summary>
    /// 此类用于实现，对两个apk包解包后的文件进行混合(包含文件的选择性复制和替换)
    /// </summary>
    class ApkCombine
    {
        static bool showCopyInfo = false;  // 是否显示复制信息

        /// <summary>
        /// 对两个apk解包后的文件进行混合，复制目录dirSource下的文件到dirDest中
        /// </summary>
        public static void Combine(String dirSource, String dirDest, Cmd.Callback call)
        {
            if (Apktool.isApkDir(dirDest) && Apktool.isApkDir(dirSource))
            {
                Cmd.Callback tmp = Settings.call;   // 记录call

                Settings.call = call;
                Settings setting = Settings.Load(dirSource, dirDest);

                //setting.GetPackageName_GAMEPRE_CHANNEL(dirSource, dirDest);      // 更新渠道参数信息

                // 若设置为使用所有渠道通用基础配置，则拷贝混合基础配置信息到游戏包
                if (setting.useAllChannelCommon)
                {
                    ToolSetting toolSet = ToolSetting.Instance();                               // 载入配置信息
                    //String AllChannelCommon = toolSet.chargeAPK_dir + "\\所有渠道\\附加资源";   // 获取所有渠道配置目录
                    //ToolSetting.confirmDir(AllChannelCommon);

                    //// 混合所有渠道公用配置
                    //if (!ApkCombine.isEmptyDirectorty(AllChannelCommon))
                    //{
                    //    if (call != null) call("【L】拷贝所有渠道附加资源：");
                    //    Combine(AllChannelCommon, dirDest, call);
                    //    if (call != null) call("【I】拷贝所有渠道附加资源完成！");
                    //}

                    String commonDir = toolSet.chargeAPK_dir + "\\所有渠道";                    // 获取所有渠道配置目录
                    Dictionary<String, String> apk_dirs = Form3.getApk_FileOrDir(commonDir);    // 获取目录下的apk文件或解压目录
                    if (apk_dirs.Count > 0)
                    {
                        String apk_dir = apk_dirs.Values.ToArray<String>()[0];
                        if (call != null) call("【I】");
                        if (call != null) call("【I】---------------------------------");
                        if (call != null) call("【L】混合，所有渠道，通用基础包资源：\r\n" + apk_dir);

                        // 所有渠道通用apk解包
                        String AllChannelCommon = Form2.apkUnpack(apk_dir, call);     // 获取所有渠道配置目录
                        if (AllChannelCommon.Contains("【E】")) return;

                        // 混合所有渠道公用配置
                        if (!ApkCombine.isEmptyDirectorty(AllChannelCommon))
                        {
                            if (call != null) call("【L】拷贝所有渠道附加资源：");
                            Combine(AllChannelCommon, dirDest, call);
                            if (call != null) call("【I】拷贝所有渠道附加资源完成！");
                        }
                        if (call != null) call("【I】---------------------------------\r\n");

                        // 拷贝所有渠道配置中的【其他渠道参数替换文件列表】
                        string str = FileProcess.fileToString(AllChannelCommon + ".txt");
                        Settings allChannel = Settings.Parse(str);
                        setting.addSettings(allChannel, 5);
                        setting.ReplaceLateParams();            // 替换设置的关键字信息为在线获取的参数

                        // 清除所有渠道通用配置缓存
                        Apktool.DeletDir(AllChannelCommon);
                        Apktool.DeletFile(AllChannelCommon + ".txt");   // 清除配置文件信息
                    }
                }


                if (call != null) call("【L】1、拷贝文件前，清除游戏包中指定的文件：");
                RemoveDirFile(dirDest, call, setting);


                if (call != null) call("【L】2、复制所有文件，并忽略忽略列表中的文件和目录：");
                CopyFolderTo_Ignore(dirSource, dirDest, call, setting);    // 复制所有文件，并忽略忽略列表中的文件和目录


                if (call != null) call("【L】3、附加拷贝，附加列表中的文件和目录：");
                CopyFolderTo_addCopyDir(dirSource, dirDest, call, setting);// 附加拷贝，附加列表中的文件和目录
                RemoveDirFile(dirDest, call, setting);                     // 清除复制时，原有目录


                String ManifestPath = dirDest + "\\" + "AndroidManifest.xml";
                if (File.Exists(ManifestPath))
                {
                    if (call != null) call("【L】4、修改Manifest.xml文件:");

                    Manifest manifest = new Manifest(ManifestPath);        // 创建Manifest对象
                    Settings.gameLabel = manifest.label;                   // 获取游戏显示名称，如： @string/app_name

                    manifest.runCMD(setting.ManifestCMD);                  // 执行manifest修改逻辑
                    manifest.save();                                       // 保存manifest
                    Settings.gameIcon = manifest.icon;                     // 获取游戏图标名称，如： @drawable/icon
                }


                setting.ReplaceFiles_ChannelParams(dirDest);                // 替换其他文件中配置的渠道参数变量
                setting.ProcessIcon(dirDest);                               // 处理icon


                Settings.call = tmp;    //还原call
            }
        }

        /// <summary>
        /// 获取dirSource相对于dirBase的路径
        /// </summary>
        public static string relativePath(string dirSource, string dirBase)
        {
            if (!dirSource.StartsWith(dirBase)) return dirSource;
            else
            {
                string relativeDir = dirSource.Substring(dirBase.Length);
                if (relativeDir.StartsWith("\\")) relativeDir = relativeDir.TrimStart('\\');
                if (relativeDir.EndsWith("\\")) relativeDir = relativeDir.TrimEnd('\\');
                
                return relativeDir;
            }
        }

        // 忽略后缀
        private static string IgnoreExtension(string relativeDir)
        {
            if (relativeDir.Contains("$")) relativeDir = relativeDir.Substring(0, relativeDir.IndexOf("$"));  // 获取前缀名SplashActivity（LtsdkAdapter$14$1$1.smali）
            if (relativeDir.Contains(".")) relativeDir = relativeDir.Substring(0, relativeDir.LastIndexOf("."));

            return relativeDir;
        }

        /// <summary>
        /// 获取pathStr的父目录
        /// </summary>
        private static string parentDir(string pathStr)
        {
            if (!pathStr.Contains("\\")) return pathStr;
            else return pathStr.Substring(0, pathStr.LastIndexOf("\\"));
        }

        /// <summary>
        /// 获取pathStr的最里层目录或文件名
        /// </summary>
        private static string getLastName(string pathStr)
        {
            if (!pathStr.Contains("\\")) return pathStr;
            else return pathStr.Substring(pathStr.LastIndexOf("\\") + "\\".Length);
        }

        /// <summary>
        /// 从dirSource中拷贝附加目录文件到dirTarget中
        /// </summary>
        public static void CopyFolderTo_addCopyDir(string dirSource, string dirTarget, Cmd.Callback call, Settings setting)
        {
            foreach (string dirAdd0 in setting.addCopyDir)
            {
                string dirAdd = dirAdd0;    // 记录原有路径
                string destDir = "";        // 记录目标路径

                // "GAMEDIR:smali\{GAMEPRE_package_PATH1}->smali\{package_PATH1}[IGNORE:]assets\bin\Data\mainData,res\2.bin"
                // 忽略,不执行smali引用修改逻辑的文件列表
                List<string> Ignores = new List<string>();  
                if (dirAdd.Contains("[IGNORE:]"))   // 执行smali引用修改逻辑时，不修改的文件
                {
                    int index = dirAdd.IndexOf("[IGNORE:]");
                    string Str = dirAdd.Substring(index + "[IGNORE:]".Length);
                    dirAdd = dirAdd.Substring(0, index).Trim();
                    string[] A = Str.Replace("，", ",").Split(',');
                    foreach (string a0 in A)
                    {
                        string a = a0.Trim();
                        if (!a.Equals("") && !Ignores.Contains(a)) Ignores.Add(a);
                    }
                    
                }

                if(dirAdd.Contains("->"))
                {
                    int index = dirAdd.IndexOf("->");
                    destDir = dirAdd.Substring(index + "->".Length);
                    dirAdd = dirAdd.Substring(0, index);
                }

                string pathSource = Path.Combine(dirSource, dirAdd);

                bool isGameDir = false;             // 标识是否为游戏解包目录
                if (dirAdd.StartsWith("GAMEDIR:"))  // 从游戏解包路径中开始检索
                {
                    isGameDir = true;
                    dirAdd = dirAdd.Substring("GAMEDIR:".Length);
                    pathSource = Path.Combine(dirTarget, dirAdd);
                }
                
                string pathTarget = Path.Combine(dirTarget, destDir.Equals("") ? dirAdd : destDir);

                //// 相同目录则不进行复制和移动
                //if(pathTarget.Equals(pathSource) || (pathTarget.Contains(pathSource) && pathTarget.StartsWith(pathSource + "\\"))) continue;

                string parent = parentDir(pathSource);      // 获取文件的父目录
                if (!Directory.Exists(parent)) continue;    // 若父目录不存在

                //先来复制文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(parent);
                FileInfo[] files = directoryInfo.GetFiles();

                //复制所有文件
                foreach (FileInfo file in files)
                {
                    String fileSource = Path.Combine(file.DirectoryName, file.Name);                    // 获取目录下的文件
                    String relative = relativePath(fileSource, isGameDir ? dirTarget : setting.Path);   // 获取相对路径名
                    if (dirAdd.Equals(relative) || dirAdd.Equals(IgnoreExtension(relative)))            // 获取的文件与dirAdd匹配
                    {
                        if (!destDir.Equals(""))       // 目标路径非空，重新计算相对路径
                        {
                            String name1 = getLastName(dirAdd);
                            String name2 = getLastName(destDir);

                            String destTmp = destDir;
                            if (name1.Equals(name2)) destTmp = parentDir(destDir);

                            relative = destTmp + "\\" + getLastName(fileSource);
                        }

                        String fileTarget = Path.Combine(dirTarget, relative);              // 获取对应路径下的目标文件

                        if (File.Exists(fileTarget))
                        {
                            // 替换已存在的文件
                            if (fileSource.Equals(fileTarget)) 
                                if (call != null) call("【I2】忽略原路径下文件的替换：" + pathSource);
                            else  replaceFile(fileSource, fileTarget, relative, call, setting);
                        }
                        else
                        {
                            string ParentDirTarget = parentDir(fileTarget);      // 获取文件的父目录
                            if (!Directory.Exists(ParentDirTarget))              // 若父目录不存在先创建
                                Directory.CreateDirectory(ParentDirTarget);

                            if (showCopyInfo && call != null) call("【I】复制文件：" + relative);
                            file.CopyTo(fileTarget, true);
                        }
                    }
                }

                // 复制目录
                if (Directory.Exists(pathSource))
                {
                    if (Directory.Exists(pathTarget))
                    {
                        if (!pathSource.Equals(pathTarget)) // 源目录与目标目录不相同时，才进行合并
                        {
                            if (call != null) call("【I3】合并目录：" + pathSource + "到" + pathTarget);
                            CopyFolderTo(pathSource, pathTarget, true, call, setting);
                        }
                    }
                    else
                    {
                        string relative = relativePath(pathSource, setting.Path);
                        if (call != null) call("【I】复制目录：" + relative);
                        CopyFolderTo(pathSource, pathTarget, true);
                    }
                }

                // smali\com\ltgame\cs\vivo\wxapi->smali\game\wxapi
                // 执行所有文件的smali和包名路径，引用修改逻辑
                if (!destDir.Equals("") && destDir.StartsWith(@"smali\") && dirAdd.StartsWith(@"smali\") && setting != null)
                {
                    String key = dirAdd.Substring(@"smali\".Length);    // 获取原有路径名
                    String value = destDir.Substring(@"smali\".Length); // 获取新的路径名

                    if (!key.Equals(value))
                    {
                        // 替换dirTarget所有文件中的keykey值为Value
                        if (call != null) call("【I】执行smali引用修改逻辑，修改所有文件中的 " + key + " 为 " + value);
                        setting.ReplaceDir_ChannelParams(dirTarget, key.Replace("\\", "."), value.Replace("\\", "."), dirTarget, Ignores);
                        setting.ReplaceDir_ChannelParams(dirTarget, key.Replace("\\", "/"), value.Replace("\\", "/"), dirTarget, Ignores);

                        // 将原有文件目录作为删除目录处理，设为清除目录
                        if (!setting.deletGameDir.Contains(dirAdd))
                        {
                            if (pathTarget.StartsWith(pathSource))
                            {
                                call("【I】目标路径 " + destDir + " 为原路径 " + dirAdd + "的子路径，忽略对该路径的删除操作");
                                List<string> list = GetSubIteams(pathTarget, dirTarget, destDir);
                                foreach (string iteam in list) 
                                    if (!setting.deletGameDir.Contains(iteam)) setting.deletGameDir.Add(iteam);
                            }
                            else setting.deletGameDir.Add(dirAdd);   // 删除原路径下文件
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取dir目录下所有子文件或文件夹，以相对于BaseDir的路径形式输出, 添加时忽略ignore_iteam指定的路径
        /// </summary>
        public static List<string> GetSubIteams(string dir, string BaseDir, string ignore_iteam)
        {
            List<string> list = new List<string>();

            if (Directory.Exists(dir))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                FileInfo[] files = directoryInfo.GetFiles();
                DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();

                // 获取dir路径下，所有非忽略项的文件信息
                foreach (FileInfo file in files)
                {
                    string relative = relativePath(file.FullName, BaseDir);              // 获取相对路径名
                    if (!list.Contains(relative) && !ignore_iteam.StartsWith(relative))  // relative非忽略项的父路径
                        list.Add(relative);
                }

                //获取dir路径下，所有非忽略项的目录和子文件信息
                foreach (DirectoryInfo d in directoryInfoArray)
                {
                    string subDir = Path.Combine(dir, d.Name);

                    string relative = relativePath(subDir, BaseDir);                        // 获取相对路径名
                    if (!list.Contains(relative))  
                    {
                        if (!ignore_iteam.StartsWith(relative))     // relative非忽略项的父路径
                            list.Add(relative);
                        else if (!ignore_iteam.Equals(relative))    // relative是忽略项的父路径
                        {
                            List<string> L = GetSubIteams(subDir, BaseDir, ignore_iteam);   // 获取relative路径下相对于忽略项的所有非忽略项
                            foreach (string iteam in L) if (!list.Contains(iteam)) list.Add(iteam);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 执行xml文件混合逻辑
        /// </summary>
        public static void xmlCombine(String fileSource, String fileTarget, String relative, Cmd.Callback call, Settings setting)
        {
            bool isManifest = fileTarget.EndsWith("AndroidManifest.xml");       // 是否为AndroidManifest.xml
            if (call != null) 
                call((isManifest ? "【I3】Manifest.xml文件合并：" : "【I3】xml文件合并：") + relative);
            if (isManifest) Manifest.call = call;
            xmlNode.Combine(fileSource, fileTarget, fileTarget, isManifest, setting != null ? setting.ManifestCMD : null, call);
        }

        /// <summary>
        /// 判断list中是否存在,以path开头的串
        /// </summary>
        public static bool havePath(List<string> list, string path)
        {
            if (list.Contains(path)) return true;
            foreach (string iteam in list)
            {
                if (iteam.Contains(path) && iteam.StartsWith(path)) return true;
            }
            return false;
        }

        /// <summary>
        /// 添加Dir2中目录和文件到Dir中
        /// 若Dir中没有，该文件或目录，则直接复制
        /// 若有，则合并、替换、或不操作
        /// 
        /// 从一个目录将其内容复制到另一目录, 忽略列表中的文件或目录补拷贝
        /// </summary>
        public static void CopyFolderTo_Ignore(string dirSource, string dirTarget, Cmd.Callback call, Settings setting)
        {
            // 若当前拷贝的目录为忽略的目录，则不拷贝
            string relative = relativePath(dirSource, setting.Path);    // 获取当前拷贝目录的相对路径
            if (setting.ingnoreDir.Contains(relative))                  // 若在忽略列表中，则不复制
            {
                if (call != null) call("【I2】忽略目录：" + dirSource);
                return;          
            }
            else if (call != null) call("【I】复制目录：" + relative);

            if (!havePath(setting.ingnoreDir, relative))    // 若拷贝的目录不包含忽略列表中的文件，则拷贝整个文件夹
            {
                CopyFolderTo(dirSource, dirTarget, true, call, setting);
                return;
            }
            else
            {
                // 检查是否存在目的目录
                if (!Directory.Exists(dirTarget)) Directory.CreateDirectory(dirTarget);


                //先来复制文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(dirSource);
                FileInfo[] files = directoryInfo.GetFiles();

                //复制所有文件
                foreach (FileInfo file in files)
                {
                    String fileSource = Path.Combine(file.DirectoryName, file.Name);
                    String fileTarget = Path.Combine(dirTarget, file.Name);
                    relative = relativePath(fileSource, setting.Path);  // 获取相对路径名
                    if (setting.ingnoreDir.Contains(relative) || setting.ingnoreDir.Contains(IgnoreExtension(relative)))
                    {
                        if (call != null) call("【I2】忽略文件：" + fileSource);
                        continue;
                    }
                    else
                    {
                        if (File.Exists(fileTarget))
                        {
                            // 执行替换文件逻辑
                            replaceFile(fileSource, fileTarget, relative, call, setting);

                        }
                        else
                        {
                            if (showCopyInfo && call != null) call("【I】复制文件：" + relative);
                            file.CopyTo(fileTarget, true);
                        }
                    }
                }

                //最后复制目录
                DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dir in directoryInfoArray)
                {
                    CopyFolderTo_Ignore(Path.Combine(dirSource, dir.Name), Path.Combine(dirTarget, dir.Name), call, setting);
                }
            }
        }

        /// <summary>
        /// 从一个目录dirTarget中移除文件或目录
        /// </summary>
        public static void RemoveDirFile(string dirTarget, Cmd.Callback call = null, Settings setting = null)
        {
            //检查是否存在目的目录
            if (!Directory.Exists(dirTarget)) return;

            // 删除所有目录
            foreach (String iteam in setting.deletGameDir)
            {
                // 删除目录
                String subDir = Path.Combine(dirTarget, iteam);
                if (Directory.Exists(subDir))
                {
                    if (call != null) call("【I2】删除目录：" + subDir);
                    Directory.Delete(subDir, true);
                }

                // 删除文件
                String parentDir = subDir.Substring(0, subDir.LastIndexOf("\\"));
                if (Directory.Exists(parentDir))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(parentDir);
                    FileInfo[] files = directoryInfo.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        String relative = relativePath(file.FullName, dirTarget);  // 获取相对路径名
                        if (setting.deletGameDir.Contains(relative) || setting.deletGameDir.Contains(IgnoreExtension(relative)))
                        {
                            if (call != null) call("【I2】删除文件：" + file.FullName);
                            file.Delete();
                        }
                    }
                }
            }

            // 移除文件或目录后，清空信息
            setting.deletGameDir.Clear();
        }


        /// <summary>
        /// 判断目录是否为空
        /// </summary>
        public static bool isEmptyDirectorty(String dir)
        {
            if (!Directory.Exists(dir)) return true;

            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            return ((directoryInfo.GetFiles() == null || directoryInfo.GetFiles().Length == 0) && 
                (directoryInfo.GetDirectories() == null || directoryInfo.GetDirectories().Length == 0));
        }

        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        public static void CopyFolderTo(string dirSource, string dirTarget, bool overwirite, Cmd.Callback call = null, Settings setting = null)
        {
            // 先获取Source目录下，当前的文件目录信息。在复制前先读取文件和目录信息，避免父目录向子目录复制时出现的无限复制循环，而只执行一次复制
            DirectoryInfo directoryInfo = new DirectoryInfo(dirSource);
            FileInfo[] files = directoryInfo.GetFiles();
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();

            // 存在文件或目录
            bool haveFileOrDir = ((files != null && files.Length > 0) || (directoryInfoArray != null && directoryInfoArray.Length > 0));

            //检查目标路径是否存在目的目录
            if (!Directory.Exists(dirTarget))
            {
                Directory.CreateDirectory(dirTarget);
            }

            //先来复制所有文件  
            foreach (FileInfo file in files)
            {
                string fileSource = Path.Combine(file.DirectoryName, file.Name);
                string fileTarget = Path.Combine(dirTarget, file.Name);
                if (call != null && setting != null && overwirite)
                {
                    string relative = relativePath(fileSource, setting.Path);  // 获取相对路径名

                    if (File.Exists(fileTarget))
                        replaceFile(fileSource, fileTarget, relative, call, setting);
                    else
                    {
                        if(showCopyInfo) call("【I】复制文件：" + relative);
                        file.CopyTo(fileTarget, overwirite);
                    }
                }
                else
                {
                    if (fileTarget.EndsWith(".xml")) xmlCombine(fileSource, fileTarget, "", call, setting);
                    else
                    {
                        if (overwirite && call != null) call("【I3】替换文件：" + fileTarget);
                        file.CopyTo(fileTarget, overwirite);
                    }
                }
            }

            //最后复制目录
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                CopyFolderTo(Path.Combine(dirSource, dir.Name), Path.Combine(dirTarget, dir.Name), overwirite, call, setting);
            }

        }

        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        public static void CopyFolderTo(string dirSource, string dirTarget, bool overwirite)
        {
            // 先获取Source目录下，当前的文件目录信息。在复制前先读取文件和目录信息，避免父目录向子目录复制时出现的无限复制循环，而只执行一次复制
            DirectoryInfo directoryInfo = new DirectoryInfo(dirSource);
            FileInfo[] files = directoryInfo.GetFiles();
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();

            //检查目标路径是否存在目的目录
            if (!Directory.Exists(dirTarget)) Directory.CreateDirectory(dirTarget);

            //先来复制所有文件  
            foreach (FileInfo file in files)
            {
                string fileSource = Path.Combine(file.DirectoryName, file.Name);
                string fileTarget = Path.Combine(dirTarget, file.Name);
                file.CopyTo(fileTarget, overwirite);
            }

            //最后复制目录
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                CopyFolderTo(Path.Combine(dirSource, dir.Name), Path.Combine(dirTarget, dir.Name), overwirite);
            }
        }


        /// <summary>
        /// 替换文件逻辑
        /// </summary>
        private static void replaceFile(string fileSource, string fileTarget, string relative, Cmd.Callback call = null, Settings setting = null)
        {
            if (fileTarget.EndsWith(".xml"))
            {
                // 若为xml文件，则执行xml文件混合逻辑
                xmlCombine(fileSource, fileTarget, relative, call, setting);
            }
            else if (fileTarget.EndsWith("config.txt") && relative.Equals(@"assets\ltsdk_res\config.txt"))
            {
                LT_config.Config_Combine(fileSource, fileTarget, call, false);         // 混合dirSource目录下的config.txt到dirDest目录下
            }
            else
            {
                if (call != null) call("【I3】替换文件：" + relative);
                System.IO.File.Copy(fileSource, fileTarget, true);
            }
        }

    }
}



