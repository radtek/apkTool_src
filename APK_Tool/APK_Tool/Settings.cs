using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APK_Tool
{
    /// <summary>
    /// 此类用于为Apk_Tool附加一些外部配置信息对应的处理逻辑。
    /// 1、忽略路径，文件设置（忽略的文件或目录不复制）
    /// 2、复制指定的目录或文件（分为相对路径、和绝对路径文件）
    /// 3、配置信息替换逻辑，用于替换一些打包时的配置参数
    /// </summary>
    /// 
    // 设置信息,用于存储本工具的设置信息
    public class Settings
    {
        [JsonIgnore]
        public static Cmd.Callback call;

        [JsonProperty("【删除游戏目录文件列表】")]
        public List<string> deletGameDir = new List<string>();  // 删除游戏目录文件列表

        [JsonProperty("【忽略目录文件列表】")]
        public List<string> ingnoreDir = new List<string>();    // 忽略目录文件列表

        [JsonProperty("【附加拷贝目录文件列表】")]
        public List<string> addCopyDir = new List<string>();    // 附加拷贝目录文件列表

        [JsonProperty("【Manifest.xml附加处理逻辑】")]
        public List<string> ManifestCMD = new List<string>();   // Manifest附加处理逻辑

        [JsonProperty("【config配置信息添加或修改】")]
        public List<string> configs = new List<string>();       // config.txt附加添加或修改的信息内容

        [JsonProperty("【其他渠道参数替换文件列表】")]
        public List<string> keyReplaceFiles = new List<string>(); // 待替换渠道参数的所有文件

        public bool useCommonConfig = false;                     // 是否使用通用配置信息
        public bool useAllChannelCommon = false;                 // 是否使用所有渠道，基础配置信息
        public bool autoProcessIcon = true;                      // 是否自动对icon和logo进行处理

        [JsonIgnore]
        LT_config config_txt = new LT_config();                 // 创建一个config配置对象，用于修改配置信息

        public Settings(bool init)
        {
            if (init)
            {
                ingnoreDir.Add(@"smali\com\fxlib");
                ingnoreDir.Add(@"smali\com\unity3d");
                //ingnoreDir.Add(@"smali\com\ltsdk\union");           // 默认忽略目录
                ingnoreDir.Add(@"smali\com\ltsdk\union\common");
                ingnoreDir.Add(@"smali\com\ltsdk\union\util");
                ingnoreDir.Add(@"smali\com\ltsdk\union\GameMainActivity");
                ingnoreDir.Add(@"smali\com\ltsdk\union\LogoCallBack");
                ingnoreDir.Add(@"smali\com\ltsdk\union\Ltsdk");
                ingnoreDir.Add(@"smali\com\ltsdk\union\LtsdkExtkey");
                ingnoreDir.Add(@"smali\com\ltsdk\union\LtsdkKey");
                ingnoreDir.Add(@"smali\com\ltsdk\union\LtsdkListener");
                ingnoreDir.Add(@"smali\com\ltsdk\union\LtsdkUnity");
                ingnoreDir.Add(@"smali\com\ltsdk\union\R");
                ingnoreDir.Add(@"smali\com\ltsdk\union\SplashActivity");
                ingnoreDir.Add(@"smali\com\ltsdk\union\platform\LtsdkAdapter");
                ingnoreDir.Add(@"smali\com\ltsdk\union\platform\LtsdkDemo");
                
                ingnoreDir.Add(@"smali\com\ltsdk_entryDemo");
                //ingnoreDir.Add(@"AndroidManifest.xml");
                ingnoreDir.Add(@"apktool.yml");

                //addCopyDir.Add(@"smali\com\ltsdk\union\LtsdkApplication");     // 默认复制目录
                //addCopyDir.Add(@"smali\com\ltsdk\union\platform\LtsdkLenovo"); 

                // 添加默认的Manifest处理逻辑
                ManifestCMD.Add("remove GAME_MAIN_ACTIVITY/intent-filter");
                ManifestCMD.Add("add THIS_MAIN_ACTIVITY to GAME_MAIN_ACTIVITY");
                ManifestCMD.Add("remove THIS_MAIN_ACTIVITY");

                //configs.Add("ltsdk_debug=true");
            }
        }

        # region 配置保存与载入逻辑

        [JsonIgnore]           //忽略该属性  
        public string Path = "";

        [JsonIgnore]
        public static String gameId = "";
        [JsonIgnore]
        public static String channelId = "";

        [JsonIgnore]
        public static Dictionary<String, String> channel_param = new Dictionary<string,string>();   // 渠道相关参数信息


        /// <summary>
        /// 获取通用配置信息
        /// </summary>
        public static Settings getCommonConfig()
        {
            ToolSetting toolSet = ToolSetting.Instance();
            String common = toolSet.chargeAPK_dir + "\\通用配置.txt";

            Settings set;
            if (!File.Exists(common))
            {
                set = new Settings(true);   // 创建通用配置
                set.save();                 // 保存
            }
            else
            {
                // 载入通用配置信息
                string str = FileProcess.fileToString(common);
                set = Parse(str);
            }

            set.Path = common;          // 设置其保存路径

            return set;
        }

        /// <summary>
        /// 获取当前配置的所有配置列表信息
        /// </summary>
        public List<List<String>> getAllList()
        {
            List<List<String>> All = new List<List<string>>();
            All.Add(ingnoreDir);
            All.Add(deletGameDir);
            All.Add(addCopyDir);
            All.Add(ManifestCMD);
            All.Add(configs);
            All.Add(keyReplaceFiles);

            return All;
        }

        /// <summary>
        /// 添加setting中的所有配置信息到当前配置中
        /// </summary>
        public Settings addSettings(Settings setting, int index = -1)
        {
            if (setting == null) return this;

            List<List<String>> All = getAllList();
            List<List<String>> All2 = setting.getAllList();

            for (int j = 0; j < All.Count; j++)
            {
                if (index == j || index == -1)
                {
                    List<String> Iteam = All[j];
                    List<String> Iteam2 = All2[j];

                    // 添加Iteam2中的所有配置信息到当前配置中
                    foreach (String iteam in Iteam2)
                    {
                        if (!Iteam.Contains(iteam))
                        {
                            Iteam.Add(iteam);
                        }
                    }
                }
            }

            return this;
        }


        /// <summary>
        /// 载入设置信息
        /// </summary>
        public static Settings Load(String Path, String dirDest=null)
        {
            String dirSource = Path;

            Settings common = getCommonConfig();     // 载入通用配置信息

            Settings set;
            if (Path.EndsWith(".txt")) Path = Path.Substring(0, Path.LastIndexOf(".txt"));

            // 载入设置信息
            string str = FileProcess.fileToString(Path + ".txt");
            if (!str.Equals(""))        // 载入配置信息
            {
                set = Parse(str);
                set.Path = Path;

                if (set.useCommonConfig && set != null)
                    set = set.addSettings(common);
            }
            else
            {
                if (call != null) call("【I2】 未获取到渠道sdk配置信息！");

                // 载入默认配置
                set = common;
                
                if(set == null) set = new Settings(true);   // 未获取到配置，则生成
                set.useCommonConfig = false;

                set.Path = Path;            // 设置其保存路径
                set.save();                 // 保存
            }

            
            // 当gameId和channelId非空时，会从在线配置中载入相关配置信息到Settings中
            if (!gameId.Equals("") && !channelId.Equals(""))
            {
                // 获取在线配置信息
                online_Data webSet = getOnlineSettings(gameId, channelId);

                set.addDefaultModify();     // 添加默认修改逻辑

                if (dirDest != null)        // 获取游戏原有包名信息和渠道计费包名信息
                    set.GetPackageName_GAMEPRE_CHANNEL(dirSource, dirDest);

                set.ReplaceLateParams();    // 替换设置的关键字信息为在线获取的参数

                gameId = "";
                channelId = "";
            }
            else if (dirDest != null)        // 获取游戏原有包名信息和渠道计费包名信息
            {
                set.GetPackageName_GAMEPRE_CHANNEL(dirSource, dirDest);
            }

            // 载入config配置信息
            set.config_txt.load(Path + "\\assets\\ltsdk_res");
            if (set.configs.Count > 0)
            {
                set.config_txt.AddValues(set.configs);  // 添加配置中的属性设置
                set.config_txt.save();      // 保存配置信息到文件
            }

            set.formatDir(false);
            return set;
        }

        /// <summary>
        /// 获取游戏渠道参数
        /// </summary>
        public static void getChannelparams(String gameId, String channelId, Cmd.Callback call = null)
        {
            online_Data webSet = getOnlineSettings(gameId, channelId, call);
            channel_param = webSet.channel_param;    // 记录渠道参数信息
        }

        /// <summary>
        /// 获取关键字，游戏原有包名、渠道计费包名
        /// </summary>
        public void GetPackageName_GAMEPRE_CHANNEL(String dirSource, String dirDest)
        {
            String GAMEPRE_package = xmlNode.getPackageName(dirDest + "\\AndroidManifest.xml");   // 获取游戏原有包名信息 如:com.ltsdk_test.demo
            String CHANNEL_package = xmlNode.getPackageName(dirSource + "\\AndroidManifest.xml"); // 渠道包名信息 如: com.ltgame.xiyou.g44937


            if (!GAMEPRE_package.Equals("") && !channel_param.Keys.Contains("GAMEPRE_package"))
            {
                channel_param.Add("GAMEPRE_package", GAMEPRE_package);
                channel_param.Add("GAMEPRE_package_PATH1", GAMEPRE_package.Replace(".", "\\"));
            }

            if (!CHANNEL_package.Equals("") && !channel_param.Keys.Contains("CHANNEL_package"))
            {
                channel_param.Add("CHANNEL_package", CHANNEL_package);
                channel_param.Add("CHANNEL_package_PATH1", CHANNEL_package.Replace(".", "\\"));
            }

            //ReplaceLateParams();    // 替换设置的关键字信息为在线获取的参数

            if (call != null)
            {
                if (channel_param.Keys.Contains("GAMEPRE_package"))
                {
                    call("【I2】 ");
                    call("【I2】 " + "获取关键字，游戏原有包名信息");
                    call("【I2】 " + "GAMEPRE_package" + ":" + GAMEPRE_package);
                    call("【I2】 " + "GAMEPRE_package_PATH1" + ":" + GAMEPRE_package.Replace(".", "\\"));
                }

                if (channel_param.Keys.Contains("CHANNEL_package"))
                {
                    call("【I2】 ");
                    call("【I2】 " + "获取关键字，渠道计费包名信息");
                    call("【I2】 " + "CHANNEL_package" + ":" + CHANNEL_package);
                    call("【I2】 " + "CHANNEL_package_PATH1" + ":" + CHANNEL_package.Replace(".", "\\"));
                }
            }
        }

        /// <summary>
        /// 获取游戏的在线配置信息，用于修改游戏包名、以及一些其他的参数
        /// http://netunion.joymeng.com/index.php?m=Api&c=PackTool&a=channelParam&app_id=1000&channel_id=0000843
        /// </summary>
        public static online_Data getOnlineSettings(String gameId, String channelId, Cmd.Callback call = null)
        {
            string url = "http://netunion.joymeng.com/index.php?m=Api&c=PackTool&a=channelParam&app_id=" + gameId + "&channel_id=" + channelId;
            string str = WebSettings.getWebData(url);
            online_Data iteam = online_Data.Parse(str, channelId);  // 解析Json数据
            iteam.ParseParams(str);                                 // 解析渠道参数
            iteam.channel_param.Add("GAMEID", gameId);              // 在其中附加gameId
            iteam.channel_param.Add("CHANNELID", channelId);        // 在其中附加channelId
            if (iteam.channel_param.Keys.Contains("package"))
            {
                iteam.channel_param["package"] = iteam.channel_param["package"].Trim(); // 包名trim()去空格
                String package = iteam.channel_param["package"];
                iteam.channel_param.Add("package_PATH1", package.Replace(".", "\\"));
                iteam.channel_param.Add("package_PATH2", package.Replace(".", "/"));
            }

            if (iteam.channel_param.Keys.Contains("access_platform"))
            {
                String access_platform = iteam.channel_param["access_platform"];
                iteam.channel_param.Add("access_platform_UPPER", access_platform.Substring(0, 1).ToUpper() + access_platform.Substring(1));
            }

            // 输出当前获取到的渠道参数
            if (call != null)
            {
                call("【I2】 ");
                call("【I2】 " + "获取渠道参数\r\n" + url);
                if (iteam.status.Equals("0")) call("【E】 " + "未获取到当前游戏对应的渠道参数信息，请在“网游管理后台”中添加。\r\n点击菜单栏：文件->乐堂网游管理后台->打包渠道参数->添加\r\n" + url);

                foreach (string key in iteam.channel_param.Keys)
                {
                    call("【I2】 " + key + ":" + iteam.channel_param[key]);
                }
                call("【I2】 ");
            }

            return iteam;
        }

        /// <summary>
        /// 添加默认修改逻辑，参数从在线配置信息中获取
        /// </summary>
        public void addDefaultModify()
        {
            // 删除包名路径下的R文件
            //deletGameDir.Add(@"smali\{package_PATH1}\R");
            //deletGameDir.Add(@"smali\{package_PATH1}\BuildConfig");

            // 忽略渠道包名路径下的R文件和public.xml
            ingnoreDir.Add(@"res\values\public.xml");
            ingnoreDir.Add(@"smali\{CHANNEL_package_PATH1}\R");
            ingnoreDir.Add(@"smali\{CHANNEL_package_PATH1}\BuildConfig");

            // 修改包名
            ManifestCMD.Add("get MANIFEST set package={package}");

            // 修改游戏名称 @string/app_name
            if (gameLabel.StartsWith("@string/"))
            {
                string appName = gameLabel.Substring("@string/".Length);    // app_name
                string filePath = "res/values/strings.xml";
                string cmd = "get string:" + appName + " setValue {replace_app_name}";
                cmd = filePath + " AS_XML " + cmd;
                // 生成修改命令cmd = res/values/strings.xml AS_XML get string:app_name setValue {replace_app_name}

                keyReplaceFiles.Add(cmd);
            }
            else ManifestCMD.Add("get MANIFEST/application set android:label=\"{replace_app_name}\"");   

            ManifestCMD.Add("get MANIFEST/application set android:banner=ATTR_NULL");    // 移除android:banner
            ManifestCMD.Add("get MANIFEST/application set android:isGame=ATTR_NULL");    // 移除android:isGame

            //ManifestCMD.Add("get MANIFEST set platformBuildVersionName=ATTR_NULL");
            //ManifestCMD.Add("get MANIFEST set platformBuildVersionCode=ATTR_NULL");
            ManifestCMD.Add("get MANIFEST set android:versionName={version_name}");      // 添加打包信息，版本名
            ManifestCMD.Add("get MANIFEST set android:versionCode={version_code}");      // 添加打包信息，版本

            // config.txt作为默认修改配置，ApkCombine中执行修改逻辑，不复制
            //ingnoreDir.Add(@"assets\ltsdk_res\config.txt");
            
            // config.txt 属性信息配置
            configs.Add("lt_usejoymenglogin={use_joymeng_login}");
            configs.Add("lt_platformautologin={platform_auto_login}");
            configs.Add("lt_appid={GAMEID}");
            configs.Add("lt_channelid={CHANNELID}");
            //configs.Add("access_platform={access_platform}");



            //// 渠道配置参数信息
            //param_Data channel_param = channel.channel_param;
            //if(!channel_param.lenovo_appkey.Equals(""))
            //    set.configs.Add("lenovo_appkey=" + channel_param.lenovo_appkey);

            //if(channel.access_platform.Equals("qihoo")) // 360
            //{
            //    set.ManifestCMD.Add("get meta-data:QHOPENSDK_APPID set android:value=\"" + channel_param.QHOPENSDK_APPID + "\"");
            //    set.ManifestCMD.Add("get meta-data:QHOPENSDK_APPKEY set android:value=\"" + channel_param.QHOPENSDK_APPKEY + "\"");
            //    set.ManifestCMD.Add("get meta-data:QHOPENSDK_PRIVATEKEY set android:value=\"" + channel_param.QHOPENSDK_PRIVATEKEY + "\"");
            //    set.ManifestCMD.Add("get meta-data:QHOPENSDK_WEIXIN_APPID set android:value=\"" + channel_param.QHOPENSDK_WEIXIN_APPID + "\"");
            //}
        }

        /// <summary>
        /// 替换ManifestCMD和configs配置的参数中，以｛关键字｝代替的在线配置信息
        /// </summary>
        public void ReplaceLateParams()
        {
            List<List<String>> All = new List<List<string>>();

            All.Add(deletGameDir);
            All.Add(ingnoreDir);
            All.Add(addCopyDir);
            All.Add(ManifestCMD);
            All.Add(configs);
            All.Add(keyReplaceFiles);

            for (int j = 0; j < All.Count; j++)
            {
                List<String> Iteam = All[j];

                for (int i = 0; i < Iteam.Count; i++)
                {
                    String cmd = Iteam[i];
                    while (cmd.Contains("{") && cmd.Contains("}"))
                    {
                        int S = cmd.IndexOf("{") + 1, E = cmd.IndexOf("}", S);
                        String name = cmd.Substring(S, E - S);


                        if (channel_param.Keys.Contains(name))
                        {
                            if (channel_param[name].Equals("")) cmd = "";
                            else cmd = cmd.Replace("{" + name + "}", channel_param[name]);
                        }
                        else if (channel_param.Keys.Contains(name.Trim()))
                        {
                            if (channel_param[name.Trim()].Equals("")) cmd = "";
                            else cmd = cmd.Replace("{" + name + "}", channel_param[name.Trim()]);
                        }
                        else
                        {
                            if (cmd.Contains("VALUE_CONST:"))           // 若为静态值，则不进行值解析
                            {
                                cmd = cmd.Replace("VALUE_CONST:", "");
                                break;
                            }
                            else
                            {
                                if (call != null) call("【E】 " + "未获取到渠道参数：" + name);
                                cmd = "";           // 清除该条cmd信息
                                continue;
                            }
                        }
                    }
                    Iteam[i] = cmd;
                }
            }
        }

        /// <summary>
        /// 替换包含在其他文件中的渠道参数，为在线获取值
        /// </summary>
        public void ReplaceFiles_ChannelParams(String Target_Dir)
        {
            if (!Target_Dir.Equals("") && channel_param != null)
            {
                if (call != null) call("【I3】 " + "其他文件中的渠道参数替换开始");
                foreach (String name0 in keyReplaceFiles)
                {
                    String name = name0.Trim();

                    //<resources>
                    //  <string name="app_name">铠甲勇士之英雄降临</string>
                    //</resources>

                    // 对指定路径下的xml文件，执行修改命令，如： 
                    // res/values/strings.xml AS_XML get resources/string:app_name setValue {replace_app_name}
                    // 修改strings.xml中的app_name
                    if (name.Contains(" AS_XML "))
                    {
                        string[] A = Manifest.split(name, " AS_XML ");
                        string filePath = Target_Dir + "\\" + A[0].Replace("/", "\\");
                        string cmd = A[1];

                        XML_File.modify(filePath, cmd);     // 执行修改逻辑
                        if (call != null) call("【I3】 " + "对文件" + A[0] + "，执行修改逻辑" + cmd);
                    }
                    // replace smali\com\ltgame\cs\vivo\wxapi\WXPayEntryActivity.smali:com/ltgame/cs/vivo to {package_PATH2}
                    else if(name.StartsWith("replace "))
                    {
                        name = name.Substring("replace ".Length).Trim();

                        string[] A = Manifest.split(name, " to ");
                        string[] B = Manifest.split(A[0], ":");

                        String filePath = Target_Dir + "\\" + B[0];
                        String key = B[1];
                        String value = A[1];

                        ReplaceFile_ChannelParams(filePath, key, value, Target_Dir);
                    }
                    else
                    {
                        String filePath = Target_Dir + "\\" + name;
                        ReplaceFile_ChannelParams(filePath, channel_param, Target_Dir);
                    }
                }
                if (call != null) call("【I3】 " + "其他文件中的渠道参数替换完成");
            }
        }

        [JsonIgnore]
        public static string gameIcon = "";    // 游戏icon名，如： @drawable/icon 或 icon

        [JsonIgnore]
        public static string gameLabel = "";   // 游戏显示名称，如： @string/app_name 或 指挥官联盟

        /// <summary>
        /// 对游戏的icon进行处理，添加角标，并适配为android所有尺寸
        /// </summary>
        public void ProcessIcon(String Target_Dir)
        {
            if (!autoProcessIcon) return;

            // 获取渠道配置信息游戏ICON、渠道角标、游戏LOGO
            String GAME_ICON = "GAME_ICON", CHANNEL_ICON = "CHANNEL_ICON", GAME_LOGO = "GAME_LOGO";
            if (channel_param.Keys.Contains("GAME_ICON")) GAME_ICON = channel_param["GAME_ICON"];
            if (channel_param.Keys.Contains("CHANNEL_ICON")) CHANNEL_ICON = channel_param["CHANNEL_ICON"];
            
            // 替换游戏Icon
            IconProcesser.ReplaceDrawable(Target_Dir, gameIcon, GAME_ICON, call);   // 使用指定的游戏Icon，替换游戏原有Icon 

            if (channel_param.Keys.Contains("GAME_LOGO")) GAME_LOGO = channel_param["GAME_LOGO"];

            // 替换游戏logo
            String logoPath = IconProcesser.getDrawable(Target_Dir, GAME_LOGO);
            if (!logoPath.Equals(""))
            {
                String gameLogo = "";
                if (channel_param.Keys.Contains("platform_logo_img")) gameLogo = channel_param["platform_logo_img"].Trim();
                if (gameLogo.Equals(""))
                {
                    String configPath = Target_Dir + @"\assets\ltsdk_res\config.txt";

                    LT_config config = new LT_config();
                    config.load_ConfigFile(configPath);   // 载入目标config配置信息

                    config.configPath = configPath;
                    config.SetValue("platform_logo_img", GAME_LOGO);    // 修改配置中的属性
                    config.save();                                      // 保存配置到文件
                }
                else IconProcesser.ReplaceDrawable(Target_Dir, gameLogo, GAME_LOGO, call);
            }

            // 处理游戏icon
            IconProcesser.AutoConfigeIcon(Target_Dir, gameIcon, CHANNEL_ICON, call);
        }

        /// <summary>
        /// 载入path对应的文件，并替换文件中所有｛关键字｝为channel_param中的对应值
        /// </summary>
        private void ReplaceFile_ChannelParams(String path, Dictionary<String, String> dic, String Base_Dir = null)
        {
            if (File.Exists(path))
            {
                String data = FileProcess.fileToString(path);   // 获取文件内容

                if (data.Contains("{") && data.Contains("}"))
                {
                    // 替换文件中的所有渠道参数信息
                    foreach (String key in dic.Keys)
                    {
                        String variable = "{" + key + "}";
                        if (data.Contains(variable))
                        {
                            data = data.Replace(variable, dic[key]);

                            String relative = Base_Dir == null ? path : ApkCombine.relativePath(path, Base_Dir);
                            if (call != null) call("【I3】 " + "替换渠道参数" + variable + "为" + dic[key] + "\r\n文件位置：" + relative);
                        }
                        else if (call != null) call("【E】 " + "未获取到渠道参数：" + key);
                    }
                }

                FileProcess.SaveProcess(data, path);           // 保存对文件的修改
            }
        }

        /// <summary>
        /// 载入path对应的文件，并替换文件中所有key为value
        /// </summary>
        private void ReplaceFile_ChannelParams(String path, String key, String value, String Base_Dir = null, List<string> Ignores = null)
        {
            if (File.Exists(path))
            {
                String data = FileProcess.fileToString(path);   // 获取文件内容

                if (data.Contains(key))
                {
                    // 若当前待修改的文件在忽略列表中，则不替换
                    String relative = Base_Dir == null ? path : ApkCombine.relativePath(path, Base_Dir);
                    if (Ignores != null && Ignores.Contains(relative))
                    {
                        if (call != null) call("【I2】 " + "忽略文件" + relative + "中 " + key + " 的替换");
                    }
                    else
                    {
                        // 替换文件中的所有关键字信息
                        data = data.Replace(key, value);
                        if (call != null) call("【I3】 " + "替换文件" + relative + "中所有 " + key + " 为 " + value);

                        FileProcess.SaveProcess(data, path);           // 保存对文件的修改
                    }
                }
            }
        }

        /// <summary>
        /// 载入dirPath目录和其子目录下的所有文件，并替换文件中所有key为value
        /// </summary>
        public void ReplaceDir_ChannelParams(String dirPath, String key, String value, String Base_Dir = null, List<string> Ignores=null)
        {
            //检查是否存在目的目录
            if (!Directory.Exists(dirPath)) return;

            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            FileInfo[] files = directoryInfo.GetFiles();

            //替换文件中所有文件的所有key值
            foreach (FileInfo file in files)
            {
                ReplaceFile_ChannelParams(file.FullName, key, value, Base_Dir, Ignores);
            }

            //替换子目录中的所有文件中的key值
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                ReplaceDir_ChannelParams(dir.FullName, key, value, Base_Dir, Ignores);
            }
        }


        // 规整化目录格式
        private void format(List<string> list, bool output = true)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (output)
                {
                    if (list[i].StartsWith("\\")) list[i] = list[i].TrimStart('\\');
                    if (list[i].EndsWith("\\")) list[i] = list[i].TrimEnd('\\');

                    //list[i] = list[i].Replace("\\", "/");
                }
                //else list[i] = list[i].Replace("/", "\\");
            }
        }

        // 规整化目录格式
        private void formatDir(bool output = true)
        {
            format(ingnoreDir, output);
            format(addCopyDir, output);
            format(keyReplaceFiles, output);
        }

        // 保存当前设置信息
        public void save()
        {
            formatDir();

            if (Path.Equals("")) Path = System.AppDomain.CurrentDomain.BaseDirectory + "默认配置.txt";
            if (Path.EndsWith(".txt")) Path = Path.Substring(0, Path.LastIndexOf(".txt"));
            string str = FileProcess.fileToString(Path + ".txt");

            // 若配置信息变动，则保存当前设置信息
            string Json = ToJson();
            if (!Json.Equals(str)) FileProcess.SaveProcess(Json, Path + ".txt");

            // 生成配置说明
            string tipInfo = Path.Substring(0,Path.LastIndexOf('\\')) + "\\APK_Tool配置说明.txt";
            DependentFiles.SaveFile(Properties.Resources.tipInfo, tipInfo);
        }

        # endregion


        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            String Str = JsonConvert.SerializeObject(this);
            Str = Str.Replace("\\\\", "\\");
            return Str;
        }

        // 数组的反序列化，返回Data_daily数组
        public static String ToJson(List<Settings> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        public static Settings Parse(string JsonStr)
        {
            JsonStr = JsonStr.Replace("\\", "\\\\");
            return JsonConvert.DeserializeObject<Settings>(JsonStr);
        }

        // 数组的反序列化，返回Data_daily数组
        public static List<Settings> Iteams(string JsonStr)
        {
            try
            {
                List<Settings> iteams = JsonConvert.DeserializeObject<List<Settings>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<Settings>(); }
        }


        // 获取指定路径下的文件为Settings
        public static Settings getSettings(String filePath)
        {
            Settings set = null;
            if (File.Exists(filePath))
            {
                string str = FileProcess.fileToString(filePath);
                set = Settings.Parse(str);
            }
            else
            {
                set = new Settings(false);
            }

            set.Path = filePath;
            return set;
        }

        // 将sourceSetting中的信息添加至targetSetting中
        public static void AppendSettingsTo(String sourceSetting, String targetSetting)
        {
            if (File.Exists(sourceSetting))
            {
                Settings Scource = getSettings(sourceSetting);
                Settings Target = getSettings(targetSetting);

                Target.addSettings(Scource);
                Target.save();
            }
        }
    }

    public class FileProcess
    {
        #region 文件读取与保存

        /// <summary>
        /// 获取文件中的数据串
        /// </summary>
        public static string fileToString(String filePath)
        {
            string str = "";

            //获取文件内容
            if (System.IO.File.Exists(filePath))
            {
                bool defaultEncoding = filePath.EndsWith(".txt");

                System.IO.StreamReader file1;

                file1 = new System.IO.StreamReader(filePath);                  //读取文件中的数据
                //if (defaultEncoding) file1 = new System.IO.StreamReader(filePath, Encoding.Default);//读取文件中的数据
                //else file1 = new System.IO.StreamReader(filePath);                  //读取文件中的数据

                str = file1.ReadToEnd();                                            //读取文件中的全部数据

                file1.Close();
                file1.Dispose();
            }
            return str;
        }

        /// <summary>
        /// 保存数据data到文件处理过程，返回值为保存的文件名
        /// </summary>
        public static String SaveProcess(String data, String filePath, Encoding encoding = null)
        {
            //不存在该文件时先创建
            System.IO.StreamWriter file1 = null;
            if(encoding == null) file1 = new System.IO.StreamWriter(filePath, false/*, System.Text.Encoding.UTF8*/);     //文件已覆盖方式添加内容
            else file1 = new System.IO.StreamWriter(filePath, false, Encoding.Default);     // 使用指定的格式进行保存

            file1.Write(data);                                                              //保存数据到文件

            file1.Close();                                                                  //关闭文件
            file1.Dispose();                                                                //释放对象

            return filePath;
        }

        /// <summary>
        /// 获取当前运行目录
        /// </summary>
        public static string CurDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion
    }

    /// <summary>
    /// 此类用于保存打包工具相关的配置信息
    /// </summary>
    public class ToolSetting
    {
        //[JsonProperty("游戏裸包根目录")]
        //public string gameAPK_dir = "F:\\APK_Base\\游戏裸包";

        //[JsonProperty("渠道计费包根目录")]
        //public string chargeAPK_dir = "F:\\APK_Base\\渠道计费包";

        //[JsonProperty("apk输出目录")]
        //public string outputAPK_dir = "F:\\APK_Base\\apk输出目录";

        //[JsonProperty("工具缓存目录")]
        //public string ProcessTmp_dir = "F:\\APK_Base\\工具缓存目录";

        [JsonIgnore]
        public string serverRoot = AppDomain.CurrentDomain.BaseDirectory + @"APK_Base\";

        [JsonProperty("游戏裸包根目录")]
        public string gameAPK_dir = AppDomain.CurrentDomain.BaseDirectory + @"APK_Base\" + "游戏裸包";

        [JsonProperty("渠道计费包根目录")]
        public string chargeAPK_dir = AppDomain.CurrentDomain.BaseDirectory + @"APK_Base\" + "渠道计费包";

        [JsonProperty("apk输出目录")]
        public string outputAPK_dir = AppDomain.CurrentDomain.BaseDirectory + "apk输出目录";

        [JsonProperty("工具缓存目录")]
        public string ProcessTmp_dir = AppDomain.CurrentDomain.BaseDirectory + "工具缓存目录";

        [JsonProperty("使用bat模式")]
        public bool useBatMode = false;

        [JsonProperty("显示R文件id修改信息")]
        public bool showRsmaliModify = false;

        [JsonProperty("本地渠道信息")]
        public List<string> channelsInfo = new List<string>();       // config.txt附加添加或修改的信息内容

        [JsonProperty("R资源文件id处理")]
        public bool R_Process = true;                               // 是否自动对R资源文件进行处理

        [JsonProperty("游戏R资源文件id处理")]
        public bool R_Process_Game = true;                            // 是否自动对R资源文件进行处理

        [JsonProperty("修改apktool.yml")]
        public bool apktool_yml_Process = false;                     // 是否修改apktool.yml

        /// <summary>
        /// 向ChannelList中添加本地配置的渠道信息
        /// </summary>
        /// <param name="channelList"></param>
        public void AppendLocalChannels(Dictionary<String, String> channelList)
        {
            foreach (String iteam in channelsInfo)
            {
                if (!iteam.Contains("_")) continue;
                int index = iteam.IndexOf("_");
                String channelId = iteam.Substring(0, index).Trim();
                String channelName = iteam.Substring(index).Trim();
                if(!channelList.ContainsKey(channelId)) channelList.Add(channelId, channelName);
            }
        }

        [JsonIgnore]
        private bool useLocalBase = true;
        /// <summary>
        /// 检测目录信息，是否存在
        /// </summary>
        public void checkPath()
        {
            //@"\\10.80.3.252\测试专用（勿动)\APK_Base\";
            //String Ip = GetLocalIp();
            //String netPath = @"\\" + Ip.Substring(0, Ip.LastIndexOf(".") + 1) + "252" + @"\测试专用（勿动)";
            //if (Directory.Exists(netPath)) serverRoot = netPath + @"\APK_Base\";

            // 若未获取到服务器目录，则获取本地工具目录
            //if (useLocalBase /* || !Directory.Exists(serverRoot)*/ )
            //{
            //    serverRoot = AppDomain.CurrentDomain.BaseDirectory + @"APK_Base\";
            //}

            //gameAPK_dir = serverRoot + "游戏裸包";
            //chargeAPK_dir = serverRoot + "渠道计费包";

            //outputAPK_dir = serverRoot + "apk输出目录";

            // 根据渠道计费包路径，确定APK_Base所在路径
            serverRoot = chargeAPK_dir.Substring(0, chargeAPK_dir.Length - "渠道计费包".Length);

            confirmDir(gameAPK_dir);
            confirmDir(chargeAPK_dir);
            confirmDir(outputAPK_dir);
            confirmDir(ProcessTmp_dir);
        }

        //获取本地ip地址,优先取内网ip
        private String GetLocalIp()
        {
            String[] Ips = GetLocalIpAddress();

            foreach (String ip in Ips) if (ip.StartsWith("10.80.")) return ip;
            foreach (String ip in Ips) if (ip.Contains(".")) return ip;

            return "127.0.0.1";
        }

        //获取本地ip地址，多个ip
        private String[] GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();                    //获取主机名称  
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址  

            string[] IP = new string[addresses.Length];             //转换为字符串形式  
            for (int i = 0; i < addresses.Length; i++) IP[i] = addresses[i].ToString();

            return IP;
        }  


        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            String JsonStr = JsonConvert.SerializeObject(this);
            JsonStr = JsonStr.Replace("\\\\", "\\");
            return JsonStr;
        }

        // 数组的反序列化，返回Data_daily数组
        public static String ToJson(List<ToolSetting> Iteams)
        {
            
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        public static ToolSetting Parse(string JsonStr)
        {
            JsonStr = JsonStr.Replace("\\", "\\\\");
            return JsonConvert.DeserializeObject<ToolSetting>(JsonStr);
        }

        // 数组的反序列化，返回Data_daily数组
        public static List<ToolSetting> Iteams(string JsonStr)
        {
            try
            {
                List<ToolSetting> iteams = JsonConvert.DeserializeObject<List<ToolSetting>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<ToolSetting>(); }
        }

        # region 配置信息的保存与载入逻辑

        [JsonIgnore]
        public static string path = AppDomain.CurrentDomain.BaseDirectory + "tools";
        [JsonIgnore]
        public static string filePath = path + "\\工具配置信息.txt";

        [JsonIgnore]
        public static ToolSetting instance;   // 配置信息的一个实例对象

        /// <summary>
        /// 确保目录下存在目录
        /// </summary>
        public static void confirmDir(String path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static String InstanceStr = "";
        /// <summary>
        /// 载入配置信息
        /// </summary>
        public static ToolSetting Instance()
        {
            if (instance == null)
            {
                ToolSetting set;
                confirmDir(path);

                // 载入配置信息
                string str = FileProcess.fileToString(filePath);
                if (str.Contains(@"\测试专用（勿动)")) str = "";   // 清除指向共享的配置信息

                if (!str.Equals(""))            // 载入配置信息
                {
                    InstanceStr = str;
                    set = Parse(str);
                    set.checkPath();            // 检测目录是否存在
                }
                else
                {
                    set = new ToolSetting();    // 未获取到配置，则生成
                    set.checkPath();            // 检测目录是否存在
                    set.save();                 // 保存
                }

                instance = set;
            }

            Cmd.useBatMode = instance.useBatMode;   // 是否使用bat模式运行工具

            return instance;
        }

        // 保存当前配置信息
        public void save()
        {
            // 载入配置信息
            string str = FileProcess.fileToString(filePath);

            // 若配置信息变动，则保存当前设置信息
            string Json = ToJson();
            Json = Json.Replace("{", "{\r\n\t").Replace(",", ",\r\n\t").Replace("}", "\r\n}");    //Json配置格式化
            if (!Json.Equals(str)) FileProcess.SaveProcess(Json, filePath);
        }

        # endregion
    }


    // 示例：scimence( Name1(6JSO-F2CM-4LQJ-JN8P) )scimence
    // string url = "https://git.oschina.net/scimence/easyIcon/wikis/OnlineSerial";
    // 
    // string data = getWebData(url);
    // string str1 = getNodeData(data, "scimence", false);
    // string str2 = getNodeData(str1, "Name1", true);
    public class WebData
    {
        #region 网络数据的读取

        //从给定的网址中获取数据
        public static string getWebData(string url)
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.Default;
                string data = client.DownloadString(url);
                return data;
            }
            catch (Exception) { return ""; }
        }

        #endregion


        // 从自定义格式的数据data中，获取nodeName对应的节点数据
        //p>scimence(&#x000A;NeedToRegister(false)NeedToRegister&#x000A;RegisterPrice(1)RegisterPrice&#x000A;)scimence</p>&#x000A;</div>
        // NeedToRegister(false)&#x000A;RegisterPrice(1)   finalNode的数据格式
        public static string getNodeData(string data, string nodeName, bool finalNode)
        {
            try
            {
                string S = nodeName + "(", E = ")" + (finalNode ? "" : nodeName);
                int indexS = data.IndexOf(S) + S.Length;
                int indexE = data.IndexOf(E, indexS);

                return data.Substring(indexS, indexE - indexS);
            }
            catch (Exception) { return data; }
        }
    }

}
