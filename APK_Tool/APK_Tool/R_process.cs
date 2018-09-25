using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace APK_Tool
{
    // 用于R文件处理
    class R_process
    {
        public static Dictionary<String, List<xmlNode>> gameResDic;     // 游戏原有资源id列表信息
        public static Dictionary<String, List<xmlNode>> channelResDic;  // 渠道包原有资源id列表信息
        public static Dictionary<String, List<xmlNode>> genrateResDic;  // 游戏包和渠道包混合后的资源id列表信息

        public static List<String> gameResTypeList;         // 游戏R资源类型
        public static List<String> channelResTypeList;      // 渠道R资源类型
        public static List<String> generateResTypeList;     // 混合资源R类型


        // 执行R文件资源id生成逻辑
        public static bool Start(String dirTarget, String dirSource, Cmd.Callback call, bool apktool_yml_process=false)
        {
            if (call != null) call("【L】5、R$*smali资源编译,逻辑开始...");
            String GAMEPRE_package = Settings.channel_param["GAMEPRE_package"];     // 游戏原有包名
            String CHANNEL_package = Settings.channel_param["CHANNEL_package"];     // 渠道包原有包名
            String package = Settings.channel_param["package"];                     // 获取包名信息

            // 获取游戏包和渠道包，R资源id信息
            if (call != null) call("【I】- 1.获取游戏包和渠道包，R资源id信息");
            R_process.gameResDic = R_process.getResDic(dirTarget + @"\res\values\public.xml", call);
            R_process.channelResDic = R_process.getResDic(dirSource + @"\res\values\public.xml", call);
            
            //获取游戏包和渠道包，解包路径下的R文件类型信息
            if (call != null) call("【I】-   获取游戏包和渠道包，解包路径下的R文件类型信息");
            R_process.gameResTypeList = R_process.getRes_typeList(dirTarget, GAMEPRE_package);
            R_process.channelResTypeList = R_process.getRes_typeList(dirSource, CHANNEL_package);
            

            // 根据现有资源，生成新的R$type.smali文件
            bool R_result = R_process.UpdatePublicXML(dirTarget, call);                                     // 从游戏解包资源生成新的public.xml
            if (!R_result) return false;

            R_process.genrateResDic = R_process.getResDic(dirTarget + @"\res\values\public.xml", call);  // 获取新生成的R资源id信息

            if (call != null) call("【I】- 6.根据public.xml中资源id信息，生成新的R$*.smali文件...");
            R_process.rebuidR_smali(genrateResDic, dirTarget + "\\smali", package, call);   // 重新生成包名路径下的R文件
            R_process.generateResTypeList = R_process.getRes_typeList(dirTarget, package);  // 获取新的包名路径下，现有的R文件类型信息

            // 复制原有包名路径和渠道包名路径下，其它R.smali文件到新的包名路径下
            copyR_smali(dirTarget, dirTarget, GAMEPRE_package, package, gameResTypeList, generateResTypeList, gameResDic, genrateResDic, call, true);
            copyR_smali(dirSource, dirTarget, CHANNEL_package, package, channelResTypeList, generateResTypeList, channelResDic, genrateResDic, call);

            R_process.generateResTypeList = R_process.getRes_typeList(dirTarget, package);  // 重新获取新的包名路径下，现有的R文件类型信息
            CreateR_smali(generateResTypeList, dirTarget + "\\smali", package, call);       // 根据现有的R资源类型生成对应R.smali文件

            // 修改smal路径下，原有包名路径信息到新的包名路径信息
            ReplaceSmaliPackagePath(dirTarget, GAMEPRE_package, package);
            ReplaceSmaliPackagePath(dirTarget, CHANNEL_package, package);

            // 修改所有非包名路径下的R文件中的id信息
            if (call != null) call("【I】- 7.修改所有非包名路径下的R文件中的id信息");
            //if (call != null) call("【I】- 7.暂时屏蔽");
            updateALL_RIds(dirTarget, package, channelResDic, genrateResDic, call);

            if (call != null) call("【I】R$*.smali文件处理结束\r\n");

            if(apktool_yml_process) appendunknown(dirTarget, call);     // 修改apktool.yml中的Unknown文件列表

            return true;
        }

        // 替换除包名路径下，所有R文件中的id信息
        public static void updateALL_RIds(String unpackDir, String package, Dictionary<String, List<xmlNode>> DicSrc, Dictionary<String, List<xmlNode>> DicTar, Cmd.Callback call)
        {
            String packagePath = unpackDir + "\\smali\\" + package.Trim('.').Replace('.', '\\');
            List<String> R_path = getR_smaliPath(unpackDir);

            int i = 0;
            foreach (String R_smaliDir in R_path)
            {
                if (!R_smaliDir.Equals(packagePath))                    // 非包名路径下的R文件所在路径
                {
                    //string relative = ApkCombine.relativePath(R_smaliDir, unpackDir);
                    //if (relative.StartsWith("smali\\android\\")) continue;
                    //else if (relative.StartsWith("smali\\com\\helpshift")) continue;

                    //string ortherR = ApkCombine.relativePath(R_smaliDir, unpackDir);
                    //if (ortherR.Contains(@"\android\")) continue;     // 忽略android路径下的R文件修改

                    if (call != null) call("【I】- 7." + ++i + " 修改路径" + ApkCombine.relativePath(R_smaliDir, unpackDir) + "下所有R文件id信息");
                    update_RDirIds(R_smaliDir, DicSrc, DicTar, call); 
                }
            }
        }

        // 检索指定路径下所有R.smali所在R文件路径信息
        public static List<String> getR_smaliPath(String dirTarget)
        {
            List<String> list = new List<string>();
            List<String> L = ReplaceValues.getDir_List(dirTarget, "R.smali");
            foreach(String file in L)
            {
                if (file.EndsWith("\\R.smali"))
                {
                    String path = file.Substring(0, file.Length - "\\R.smali".Length);
                    list.Add(path);
                }
            }
            return list;

            // 修改R.smali所在路径下，所有以R$*smali标识的文件中所有原有id信息为新的id信息
        }

        // 更新指定路径下所有R文件中的所有id信息
        public static void update_RDirIds(String R_smaliDir, Dictionary<String, List<xmlNode>> DicSrc, Dictionary<String, List<xmlNode>> DicTar, Cmd.Callback call)
        {
            List<String> types = getRes_typeList(R_smaliDir);       // 获取当前路径下的R文件类型信息列表

            foreach (String type in types)
            {
                List<xmlNode> listSrc = new List<xmlNode>();
                List<xmlNode> listTar = new List<xmlNode>();

                if (DicSrc.ContainsKey(type)) listSrc = DicSrc[type];
                else listSrc = DicSrc["AllType"];
                //{
                //    listSrc = DicSrc["AllType"];
                //    if (call != null) call("【I】渠道 public.xml 中不含有type类型：" + type);
                //    continue;
                //}

                if (DicTar.ContainsKey(type)) listTar = DicTar[type];
                else listTar = DicTar["AllType"];
                //{
                //    if (call != null) call("【I】 新生成的 public.xml 中不含有type类型：" + type);
                //    continue;
                //}

                if (call != null) call("【I】修改，" + "R$" + type + ".smali");
                String R_file = R_smaliDir + "\\R$" + type + ".smali";
                update_RFileIds(R_file, listSrc, listTar, call);
            }
        }

        // 更新指定的R文件中的所有id信息
        public static void update_RFileIds(String R_smaliPath, List<xmlNode> listSrc, List<xmlNode> listTar, Cmd.Callback call)
        {
            String data = FileProcess.fileToString(R_smaliPath);
            data = updateIds(data, listSrc, listTar, call);         // 修改id信息

            FileProcess.SaveProcess(data, R_smaliPath);             // 保存修改后的smali文件
            //if (call != null) call("【I】保存修改到文件，" + R_smaliPath);
        }

        public static void updateIds(String fileSource, String fileTarget, String pNameSrc, String pNameTar, List<xmlNode> listSrc, List<xmlNode> listTar, Cmd.Callback call, bool delet = false)
        {
            String data = FileProcess.fileToString(fileSource);
            bool isSameFile = fileSource.Equals(fileTarget);               // 源文件与目标文件为同一个文件

            String packageNameSrc = pNameSrc.Trim('.').Replace('.', '/');  // 原有包名，如 com/ltsdk_56_base/leshi
            String packageNameTar = pNameTar.Trim('.').Replace('.', '/');

            if (data.Contains(packageNameSrc) && !isSameFile)
            {
                data = data.Replace(packageNameSrc, packageNameTar);// 修改包名信息
                if (call != null) call("【I】修改包名信息，" + packageNameSrc + "->" + packageNameTar);
            }
            data = updateIds(data, listSrc, listTar, call);         // 修改id信息
        }

        // 替换解包smali路径下所有R文件路径，从sourcePackage到targetPackage
        private static void ReplaceSmaliPackagePath(String unpackDir, String sourcePackage, String targetPackage, Cmd.Callback call = null)
        {
            unpackDir += "\\smali";
            sourcePackage = sourcePackage.Replace(".", "/") + "/R";
            targetPackage = targetPackage.Replace(".", "/") + "/R";

            ReplaceValues.ReplaceFileContent(unpackDir, sourcePackage, targetPackage, call);
        }

        // 根据type类型从Src拷贝仅存在与Src中的项到Target目录下
        private static void copyR_smali(String source, String target, String pNameSrc, String pNameTar, List<String> typeListSrc, List<String> typeListTar, Dictionary<String, List<xmlNode>> DicSrc,Dictionary<String, List<xmlNode>> DicTar, Cmd.Callback call, bool delet = false)
        {
            String SourcePath = source + "\\smali\\" + pNameSrc.Trim('.').Replace('.', '\\');    // 形如 com\ltsdk_56_base\leshi
            String TargetPath = target + "\\smali\\" + pNameTar.Trim('.').Replace('.', '\\');
            DependentFiles.checkDir(TargetPath);

            foreach (String type in typeListSrc)
            {
                String name = "R$" + type + ".smali";
                String fileSource = SourcePath + "\\" + name;          // 获取type类型对应的smali文件
                String fileTarget = TargetPath + "\\" + name;
                if (File.Exists(fileSource))
                {
                    if (!typeListTar.Contains(type))                   // 该smali文件之前未曾生成，则自动复制
                    {
                        List<xmlNode> listSrc = new List<xmlNode>();
                        List<xmlNode> listTar = new List<xmlNode>();

                        if (DicSrc.ContainsKey(type)) listSrc = DicSrc[type];
                        else listSrc = DicSrc["AllType"];
                        //{
                        //    if (call != null) call("【I】" + SourcePath + "对应的public.xml不含有type类型：" + type);
                        //    //listSrc = ToListNode(DicSrc);   // 获取DicSrc中所有节点id信息
                        //}

                        if (DicTar.ContainsKey(type)) listTar = DicTar[type];
                        else listTar = DicTar["AllType"];
                        //{
                        //    if (call != null) call("【I】" + TargetPath + "对应的public.xml不含有type类型：" + type);
                        //    //listTar = ToListNode(DicTar);   // 获取DicTar中所有节点id信息
                        //}

                        if (call != null) call("【I】复制" + name + "从" + SourcePath + "->" + TargetPath);
                        //File.Copy(fileSource, fileTarget, true);
                        MoveR_smali(fileSource, fileTarget, pNameSrc, pNameTar, listSrc, listTar, call, delet);
                    }
                    else if (delet && !fileSource.Equals(fileTarget)) // 原文件与目标文件不是相同文件，则删除
                    {
                        File.Delete(fileSource);
                        if (call != null) call("【I】删除文件，" + fileSource);
                    }
                }
                else if (call != null) call("【I】文件丢失，" + fileSource + "...");
            }

            if (delet) // 删除原有的R.smali
            {
                String fileSource = SourcePath + "\\R.smali";
                if (File.Exists(fileSource))
                {
                    File.Delete(fileSource);
                    if (call != null) call("【I】删除文件，" + fileSource);
                }
            }
        }

        // 将Dic转存到list中，及public.xml中的全部节点值
        private static List<xmlNode> ToListNode(Dictionary<String, List<xmlNode>> Dic)
        {
            List<xmlNode> list = new List<xmlNode>();
            foreach (String key in Dic.Keys)
            {
                List<xmlNode> L = Dic[key];
                foreach (xmlNode node in L)
                {
                    if (!list.Contains(node)) list.Add(node);
                }
            }
            return list;
        }

        // 移动Source路径下的smali文件，并执行修改
        // 修改包名、修改资源id信息
        private static void MoveR_smali(String fileSource, String fileTarget, String pNameSrc, String pNameTar, List<xmlNode> listSrc, List<xmlNode> listTar, Cmd.Callback call, bool delet = false)
        {
            String data = FileProcess.fileToString(fileSource);
            bool isSameFile = fileSource.Equals(fileTarget);               // 源文件与目标文件为同一个文件

            String packageNameSrc = pNameSrc.Trim('.').Replace('.', '/');  // 原有包名，如 com/ltsdk_56_base/leshi
            String packageNameTar = pNameTar.Trim('.').Replace('.', '/');

            if (data.Contains(packageNameSrc) && !isSameFile)
            {
                data = data.Replace(packageNameSrc, packageNameTar);// 修改包名信息
                if (call != null) call("【I】修改包名信息，" + packageNameSrc + "->" + packageNameTar);
            }
            data = updateIds(data, listSrc, listTar, call);         // 修改id信息

            if (!isSameFile)    // 不是同一个文件，执行替换或删除逻辑
            {
                if (File.Exists(fileTarget))
                {
                    FileProcess.SaveProcess(data, fileTarget);
                    if (call != null) call("【W】替换文件，" + fileSource + "->" + fileTarget);
                }
                else
                {
                    FileProcess.SaveProcess(data, fileTarget);         // 保存修改后的smali文件
                    if (call != null) call("【I】保存修改到文件，" + fileTarget);
                }

                if (delet)
                {
                    File.Delete(fileSource);
                    if (call != null) call("【I】删除文件，" + fileSource);
                }
            }
        }

        //<public type="attr" name="circle_radius" id="0x7f010000" />
        //<public type="attr" name="password_length" id="0x7f010001" />
        // 修改data中所有Src中的id信息到Target根据相同name值进行修改
        private static String updateIds(String data, List<xmlNode> listSrc, List<xmlNode> listTar, Cmd.Callback call)
        {
            Dictionary<String, String> dicSrc = toIdDic(listSrc, call); // 转化为name、id映射表
            Dictionary<String, String> dicTar = toIdDic(listTar, call);

            bool showRsmaliModify = ToolSetting.Instance().showRsmaliModify;    // 显示R文件id信息修改逻辑
            bool contains0000 = false;                  // 文件中是否含有****0000的id值信息
            foreach (String name in dicSrc.Keys)
            {
                String id = dicSrc[name];
                if (data.Contains(id))
                {
                    if (id.EndsWith("0000")) contains0000 = true;
                    if(dicTar.ContainsKey(name))        // 替换data中同名name对应的id值
                    {
                        String idTar = dicTar[name];    // 目标id值
                        if (!id.Equals(idTar))
                        {
                            String idTarTmp = "0x_@_" + idTar.Substring("0x".Length);  // 替换0x7f00001这样的id串为0x_@_7f00001，避免重复，文件中全部id替换完成后，在统一替换回0x7f00001串
                            data = data.Replace(id, idTarTmp);
                            if (showRsmaliModify && call != null) call("【I】修改" + name + "的id值，" + id + "->" + idTar);
                        }
                    }
                    else if (call != null) call("【E】当前新生成的public.xml文件中，不含有资源" + name + "!");
                }
            }
            if (data.Contains("0x_@_")) data = data.Replace("0x_@_", "0x"); // 剔除附加进去的"_@_"
            if (contains0000 && data.Contains("const/high16"))
            {
                data = data.Replace("const/high16", "const");               // 剔除/high16的限制
                if (showRsmaliModify && call != null) call("【I】修改文件中所有\"const/high16\"为\"const\"");
            }

            return data;
        }

        // 将节点<public type="attr" name="circle_radius" id="0x7f010000" />转化为name到id的映射表
        private static Dictionary<String, String> toIdDic(List<xmlNode> listTar, Cmd.Callback call)
        {
            Dictionary<String, String> dic = new Dictionary<String, String>();
            foreach (xmlNode iteam in listTar)
            {
                String type = iteam.attributes.Get("type");
                String name = type + ":" + iteam.attributes.Get("name");    // 以type:name标识一个特定的id值
                String id = iteam.attributes.Get("id");
                if (!dic.ContainsKey(name)) dic.Add(name, id);
                else if (call != null) call("【E】public.xml文件中存在重复的同名节点：" + name);
            }

            return dic;
        }

        // 从指定的包名、路径下获取R文件类型信息列表， 如： R$anim.smali R$attr.smali R$color.smali 解析为: anim、attr、color
        public static List<String> getRes_typeList(String dirTarget, String packageName)
        {
            String TargetPath = dirTarget + "\\smali\\" + packageName.Trim('.').Replace('.', '\\');    // 形如 com\ltsdk_56_base\leshi
            return getRes_typeList(TargetPath);
        }

        // 获取R文件所在路径下，所有R文件类型信息列表
        public static List<String> getRes_typeList(String TargetPath)
        {
            List<String> list = new List<String>();
            if (Directory.Exists(TargetPath))
            {
                DirectoryInfo info = new DirectoryInfo(TargetPath);
                FileInfo[] files = info.GetFiles();
                foreach (FileInfo file in files)
                {
                    String name = file.Name;
                    if (name.StartsWith("R$") && name.EndsWith(".smali"))
                    {
                        String type = name.Substring("R$".Length, name.Length - "R$.smali".Length);
                        if (!list.Contains(type)) list.Add(type);   // 记录资源文件类型信息
                    }
                }
            }
            return list;
        }
        // -------------------------------------------

        //unknownFiles:
        //com/tencent/mm/sdk/platformtools/rep5402863540997075488.tmp: '8'
        //com/fxlib/util/version.txt: '8'
        /// <summary>
        /// 修改apktool.yml中的Unknown文件列表
        /// </summary>
        public static void appendunknown(String dirTarget, Cmd.Callback call)
        {
            // 获取文件内容
            String apktoolYmlPath = dirTarget + "\\apktool.yml";
            String apktoolYml = FileProcess.fileToString(apktoolYmlPath);
            String apktoolYml0 = apktoolYml;

            // 获取文件原有unknown部分
            String unknownFiles = "";
            if (apktoolYml.Contains("unknownFiles"))
            {
                unknownFiles = apktoolYml.Substring(apktoolYml.IndexOf("unknownFiles"));
            }

            String unknown = dirTarget + "\\unknown";
            String[] files = getAllFiles(unknown).Split(';');
            if (files != null && files.Length > 0)
            {
                if (call != null) call("【I】- 8.修改apktool.yml中的Unknown文件列表:");

                // 生成新的unknown
                String appendUnknowns = "";
                foreach (String file in files)
                {
                    String relativeName = ApkCombine.relativePath(file, unknown).Replace('\\', '/');
                    appendUnknowns += "\n  " + relativeName + ": '8'";
                    if (call != null) call("【I】Unknown添加：" + relativeName);
                }
                if (!appendUnknowns.Equals("")) appendUnknowns = "unknownFiles:" + appendUnknowns + "\n";


                // 替换为新的unknown文件列表信息
                if (unknownFiles.Equals("")) apktoolYml = apktoolYml + appendUnknowns;
                else apktoolYml = apktoolYml.Replace(unknownFiles, appendUnknowns);
            }

            // 文件内容变动，则保存为新的apktoolYml
            if (!apktoolYml.Equals(apktoolYml0))
            {
                FileProcess.SaveProcess(apktoolYml, apktoolYmlPath);
                if (call != null) call("【I】对apktool.yml的修改已保存！\r\n");
            }
        }

        /// <summary>  
        /// 获取目录path下所有子文件名  
        /// </summary>  
        public static string getAllFiles(String path)
        {
            StringBuilder str = new StringBuilder("");
            if (System.IO.Directory.Exists(path))
            {
                //所有子文件名  
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (string file in files)
                    str.Append((str.Length == 0 ? "" : ";") + file);

                //所有子目录名  
                string[] Dirs = System.IO.Directory.GetDirectories(path);
                foreach (string dir in Dirs)
                {
                    string tmp = getAllFiles(dir);  //子目录下所有子文件名  
                    if (!tmp.Equals("")) str.Append((str.Length == 0 ? "" : ";") + tmp);
                }
            }
            return str.ToString();
        } 

        //从dirTarget目录下获取res资源，重新编译生成public.xml
        private static bool UpdatePublicXML(String dirTarget, Cmd.Callback call)
        {
            ToolSetting settting = ToolSetting.Instance();  // 载入设置信息

            if (call != null) call("【I】- 2.解包Empty.apk");
            String emptyApk = DependentFiles.curDir() + "\\tools\\Empty.apk";   // 空项目资源路径
            if (emptyApk.Contains("\\\\")) emptyApk = emptyApk.Replace("\\\\", "\\");
            String emptyDir = Apktool.unPackage(emptyApk, null, false);         // 解包空apk
            if (emptyDir.Contains("【E】") && call != null)
            {
                call("【E】  解包Empty.apk异常");
                return false;
            }

            if (call != null) call("【I】-   复制游戏res资源");
            String Res = emptyDir + "\\res";
            ApkCombine.CopyFolderTo(dirTarget + "\\res", Res, true);            // 复制Target目录到res目录，到空工程解包路径下

            Program.Delay(3000);   // 部分机器复制文件，存在异步延时，确保文件复制完成
            if (call != null) call("【I】- 3.使用新的res资源，生成新的Empty.apk");
            String apkFile = Apktool.package(emptyDir, null);                   // 使用apktool进行打包
            if (apkFile.Contains("【E】") && call != null)
            {
                call("【E】  打包Empty.apk异常");
                call("【E】  异常信息：" + apkFile);
                return false;
            }

            if (call != null) call("【I】- 4.解包Empty.apk");
            string unpackDir = Apktool.unPackage(apkFile, null, false);         // 使用apktool进行apk的解包，生成新的public.xml
            if (unpackDir.Contains("【E】") || unpackDir.Trim().Equals(""))
            {
                if (call != null) call("【E】  解包Empty.apk异常");
                return false;
            }
            
            if (call != null) call("【I】- 5.复制生成的public.xml文件，到游戏res目录中");
            String relativePath = @"\res\values\public.xml";
            File.Copy(unpackDir + relativePath, dirTarget + relativePath, true);      // 替换原有public.xml

            relativePath = @"\res\drawable\empty_ic_launcher.png";
            File.Copy(unpackDir + relativePath, dirTarget + relativePath, true);
            relativePath = @"\res\layout\empty_activity_main.xml";
            File.Copy(unpackDir + relativePath, dirTarget + relativePath, true);

            if (call != null) call("【I】-   清除Empty.apk相关缓存资源...");

            // 清除缓存资源
            Directory.Delete(emptyDir, true);       // 删除空项目解包文件
            File.Delete(apkFile);                   // 删除生成的临时文件
            Directory.Delete(unpackDir, true);      // 删除空工程解包目录

            return true;
        }

        // 从public.xml创建res资源索引信息
        public static Dictionary<String, List<xmlNode>> getResDic(String publicXML, Cmd.Callback call=null)
        {
            // 按type类型分类子节点。 如节点，<public type="attr" name="pstsIndicatorColor" id="0x7f010004" />
            Dictionary<String, List<xmlNode>> resDic = new Dictionary<String, List<xmlNode>>();

            if (File.Exists(publicXML))
            {
                try
                {
                    String xml = FileProcess.fileToString(publicXML);   // 获取新生成的public.xml文件内容
                    List<xmlNode> list = xmlNode.Parse(xml);            // 解析xml文件内容
                    if (!(list.Count > 1 || !list[1].name.Equals("resources"))) return resDic;     // 第二个节点不是<resources>，则不再解析

                    // 获取<resources>节点
                    xmlNode root = list[1];
                    foreach (xmlNode iteam in root.childs)
                    {
                        String typeName = iteam.attributes.Get("type"); // 获取节点的type类型
                        if (!resDic.ContainsKey(typeName))              // 生成新的list
                        {
                            List<xmlNode> newTypeList = new List<xmlNode>();
                            resDic.Add(typeName, newTypeList);
                        }

                        List<xmlNode> typeList = resDic[typeName];      // 获取type类型对应的节点list
                        if (!typeList.Contains(iteam))                  // 添加节点到对应的typeList中
                            typeList.Add(iteam);
                    }

                    resDic.Add("AllType", root.childs);                 // 记录所有节点id信息
                }
                catch (Exception ex) 
                {
                    if (call != null) call("【E】- " + publicXML + "文件解析异常！\r\n " + ex.ToString());
                }
            }
            else if (call != null) call("【E】- " + publicXML + "文件不存在，无资源id信息...");

            return resDic;
        }

        // 从resDic中的id信息生成R*smali文件
        public static void rebuidR_smali(Dictionary<String, List<xmlNode>> resDic, String TargetDir, String packageName, Cmd.Callback call)
        {
            if (resDic == null || resDic.Count == 0) return;

            // 创建包名对应的路径
            String TargetPath = TargetDir + "\\" + packageName.Trim('.').Replace('.', '\\');    // 形如 com\ltsdk_56_base\leshi
            DependentFiles.checkDir(TargetPath);
 
            // 按照res中的type类型，生成对应的R$type.smali文件资源到指定的包名、路径下
            foreach (String typeName in resDic.Keys)
            {
                if (typeName.Equals("AllType")) continue;

                List<xmlNode> typeList = resDic[typeName];
                createSmali(TargetPath, packageName, typeName, typeList, call);
            }

            // 生成BuildConfig.smali
            createBuildConfig(TargetPath, packageName, call);
        }

        //<public type="attr" name="circle_radius" id="0x7f010000" />
        //<public type="attr" name="password_length" id="0x7f010001" />
        //<public type="attr" name="has_pwd_color" id="0x7f010002" />
        // 从type对应的List<xmlNode>创建对应的R$type.smali文件， packageName为包名如"com.sdk.game" ，TargetDir为目标路径, 属性名typeName
        private static void createSmali(String TargetDir, String packageName, String typeName, List<xmlNode> typeList, Cmd.Callback call)
        {
            String FileName = "R$" + typeName;                              // 生成R文件名称如 R$attr
            String PackageStr = packageName.Trim('.').Replace('.', '/');    // 形如 com/ltsdk_56_base/leshi

            // R$type.smali文件顶部
            String head = 
                ".class public final L" + PackageStr + "/" + FileName + ";" + "\r\n" +
                ".super Ljava/lang/Object;" + "\r\n" +
                ".source \"R.java\"" + "\r\n" +
                "\r\n" +
                "\r\n" +
                "# annotations" + "\r\n" +
                ".annotation system Ldalvik/annotation/EnclosingClass;" + "\r\n" +
                "    value = L" + PackageStr + "/R;" + "\r\n" +
                ".end annotation" + "\r\n" +
                "\r\n" +
                ".annotation system Ldalvik/annotation/InnerClass;" + "\r\n" +
                "    accessFlags = 0x19" + "\r\n" +
                "    name = \"" + typeName + "\"" + "\r\n" +
                ".end annotation" + "\r\n" + "\r\n" + "\r\n";

            // R$type.smali文件顶部
            String tail =
                "# direct methods" + "\r\n" +
                ".method public constructor <init>()V" + "\r\n" +
                "    .locals 0" + "\r\n" +
                "\r\n" +
                "    .prologue" + "\r\n" +
                //"    .line 18" + "\r\n" +
                "    invoke-direct {p0}, Ljava/lang/Object;-><init>()V" + "\r\n" +
                "\r\n" +
                "    return-void" + "\r\n" +
                ".end method" + "\r\n" + "\r\n" + "\r\n";

            // R$type.smali文件属性值部分
            String body = "# static fields" + "\r\n";
            foreach (xmlNode iteam in typeList)
            {
                String attrName = iteam.attributes.Get("name");
                if(attrName.Contains(".")) attrName = attrName.Replace('.','$');    // 修改.为$

                body += ".field " + iteam.name + " static final " + attrName + ":I = " + iteam.attributes.Get("id") + "\r\n" + "\r\n";
            }
            body +=  "\r\n";

            // 保存为对应的smali文件到对应路径下
            String content = head + body + tail;
            FileProcess.SaveProcess(content, TargetDir + "\\" + FileName + ".smali");

            if (call != null) call("【I】"+ PackageStr + "/" + FileName + ".smali 已生成..");
        }

        // 生成包名对应的
        private static void createBuildConfig(String TargetDir, String packageName, Cmd.Callback call)
        {
            String PackageStr = packageName.Trim('.').Replace('.', '/');    // 形如 com/ltsdk_56_base/leshi
            String content =
                ".class public final L" + PackageStr + "/BuildConfig;" + "\r\n" +
                ".super Ljava/lang/Object;" + "\r\n" +
                ".source \"BuildConfig.java\"" + "\r\n" +
                "\r\n" +
                "\r\n" +
                "# static fields" + "\r\n" +
                ".field public static final DEBUG:Z = true" + "\r\n" +
                "\r\n" +
                "\r\n" +
                "# direct methods" + "\r\n" +
                ".method public constructor <init>()V" + "\r\n" +
                "    .locals 0" + "\r\n" +
                "\r\n" +
                "    .prologue" + "\r\n" +
                "    .line 4" + "\r\n" +
                "    invoke-direct {p0}, Ljava/lang/Object;-><init>()V" + "\r\n" +
                "\r\n" +
                "    return-void" + "\r\n" +
                ".end method" + "\r\n" + "\r\n";

            // 保存BuildConfig文件到指定包名目录
            FileProcess.SaveProcess(content, TargetDir + "\\BuildConfig.smali");
            if (call != null) call("【I】" + PackageStr + "/BuildConfig.smali 已生成..");
        }

        // 根据TypeList中的类型信息，在TargetDir目录下，生成新的R.smali文件
        public static void CreateR_smali(List<String> TypeList, String TargetDir, String packageName, Cmd.Callback call)
        {
            String PackageStr = packageName.Trim('.').Replace('.', '/');    // 形如 com/ltsdk_56_base/leshi
            String TargetPath = TargetDir + "\\" + packageName.Trim('.').Replace('.', '\\');

            String body = "";
            foreach(String type in TypeList)
            {
                String iteam = "        L" + PackageStr + "/R$" + type+";";
                if(body.Equals("")) body = iteam;
                else body += ",\r\n" + iteam;
            }

            String content =
                ".class public final L" + PackageStr + "/R;" + "\r\n" +
                ".super Ljava/lang/Object;" + "\r\n" +
                ".source \"R.java\"" + "\r\n" +
                "\r\n" +
                "\r\n" +
                "# annotations" + "\r\n" +
                ".annotation system Ldalvik/annotation/MemberClasses;" + "\r\n" +
                "    value = {" + "\r\n" +
                "\r\n" +
                body + "\r\n" +
                "    }" + "\r\n" +
                ".end annotation" + "\r\n" +
                "\r\n" +
                "\r\n" +
                "# direct methods" + "\r\n" +
                ".method public constructor <init>()V" + "\r\n" +
                "    .locals 0" + "\r\n" +
                "\r\n" +
                "    .prologue" + "\r\n" +
                "    invoke-direct {p0}, Ljava/lang/Object;-><init>()V" + "\r\n" +
                "\r\n" +
                "    return-void" + "\r\n" +
                ".end method" + "\r\n" + "\r\n";

            // 保存BuildConfig文件到指定包名目录
            FileProcess.SaveProcess(content, TargetPath + "\\R.smali");
            if (call != null) call("【I】" + PackageStr + "/R.smali 已生成..");
        }
    }


}


