using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APK_Tool
{
    //json数据格式：
    //{
    //    "status": 1,
    //    "msg": "success",
    //    "data": {
    //        "0000843": {
    //            "package": "com.joymeng.qihoo",
    //            "version_name": "3",
    //            "version_code": "1",
    //            "is_landscape": "1",
    //            "use_joymeng_login": "1",
    //            "platform_auto_login": "0",
    //            "access_platform": "qihoo",
    //            "channel_param": {
    //                "AppId": "adf",
    //                "AppKey": "df"
    //            },
    //            "replace_app_name": "卡丁车ddd",
    //            "remark": ""
    //        }
    //    }
    //}

    /// <summary>
    /// 渠道配置参数
    /// </summary>
    public class param_Data
    {
        public String appId, appKey, lenovo_appkey, QHOPENSDK_APPID, QHOPENSDK_APPKEY, QHOPENSDK_PRIVATEKEY, QHOPENSDK_WEIXIN_APPID;

        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // 数组的反序列化，返回param_Data数组
        public static String ToJson(List<param_Data> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        public static param_Data Parse(string JsonStr)
        {
            return JsonConvert.DeserializeObject<param_Data>(JsonStr);
        }

        // 数组的反序列化，返回param_Data数组
        public static List<param_Data> Iteams(string JsonStr)
        {
            try
            {
                List<param_Data> iteams = JsonConvert.DeserializeObject<List<param_Data>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<param_Data>(); }
        }
    }

    /// <summary>
    /// 指定渠道的游戏参数信息
    /// </summary>
    public class channel_Data
    {
        public String package, version_name, version_code, is_landscape, use_joymeng_login, platform_auto_login, access_platform, create_time, replace_app_name, remark;
        public param_Data channel_param;

        //// 解析channel_param
        //public param_Data getChannel_param()
        //{
        //    return param_Data.Parse(channel_param);
        //}

        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // 数组的反序列化，返回channel_Data数组
        public static String ToJson(List<channel_Data> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        public static channel_Data Parse(string JsonStr)
        {
            return JsonConvert.DeserializeObject<channel_Data>(JsonStr);
        }

        // 数组的反序列化，返回channel_Data数组
        public static List<channel_Data> Iteams(string JsonStr)
        {
            try
            {
                List<channel_Data> iteams = JsonConvert.DeserializeObject<List<channel_Data>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<channel_Data>(); }
        }
    }

    /// <summary>
    /// 使用channel_Id封装，channel_Data参数信息
    /// </summary>
    public class channel_Id_Data
    {
        public channel_Data channel_Id;

        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // 数组的反序列化，返回channel_Id_Data数组
        public static String ToJson(List<channel_Id_Data> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        public static channel_Id_Data Parse(string JsonStr)
        {
            return JsonConvert.DeserializeObject<channel_Id_Data>(JsonStr);
        }

        // 数组的反序列化，返回channel_Id_Data数组
        public static List<channel_Id_Data> Iteams(string JsonStr)
        {
            try
            {
                List<channel_Id_Data> iteams = JsonConvert.DeserializeObject<List<channel_Id_Data>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<channel_Id_Data>(); }
        }
    }

    /// <summary>
    /// 渠道配置参数
    /// </summary>
    public class online_Data
    {
    //"channel_param": {
    //            "appId": "123",
    //            "appKey": "test"
    //        },

        [JsonIgnore]    // 存储渠道参数映射表
        public Dictionary<String, String> channel_param = new Dictionary<string, string>();

        public String status, msg;
        public channel_Id_Data data;

        //// 解析data
        //public channel_Data getData()
        //{
        //    return channel_Data.Parse(data);
        //}

        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // 数组的反序列化，返回online_Data数组
        public static String ToJson(List<online_Data> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        public static online_Data Parse(string JsonStr, string channelId)
        {
            JsonStr = JsonStr.Replace("\"" + channelId + "\":", "\"channel_Id\":");
            return JsonConvert.DeserializeObject<online_Data>(JsonStr);
        }

        /// <summary>
        /// 解析JsonStr中的所有channel_param属性到Dictionary中存储
        /// </summary>
        public void ParseParams(string JsonStr)
        {
            // 获取渠道所有参数
            channel_param = Tools.ParseParams(JsonStr, "channel_param");

            // 获取游戏配置参数
            getGameParams();
        }

        //"package": "com.joymeng.qihoo",
        //"version_name": "3",
        //"version_code": "1",
        //"is_landscape": "1",
        //"use_joymeng_login": "1",
        //"platform_auto_login": "0",
        //"access_platform": "qihoo",
        //"channel_param": {
        //    "AppId": "adf",
        //    "AppKey": "df"
        //},
        //"replace_app_name": "卡丁车ddd",
        //"remark": ""

        /// <summary>
        /// 将Vaule值转化为"true"或"false"
        /// </summary>
        private String ToBoolStr(String Value)
        {
            bool b = Value.Equals("1") || Value.Equals("true");
            return (b ? "true" : "false");
        }

        /// <summary>
        /// 将Vaule值转化为"landscape"或"portrait"
        /// </summary>
        private String ToScreenOrientation(String isLandScape)
        {
            bool b = isLandScape.Equals("1") || isLandScape.Equals("true");
            return (b ? "landscape" : "portrait");
        }

        /// <summary>
        /// 获取游戏配置参数
        /// </summary>
        private void getGameParams()
        {
            if (data != null && data.channel_Id != null)
            {
                channel_Data channel = data.channel_Id;   // 获取游戏对应的渠道配置信息
                channel_param.Add("package", channel.package);
                channel_param.Add("version_name", channel.version_name);
                channel_param.Add("version_code", channel.version_code);

                channel_param.Add("is_landscape", ToBoolStr(channel.is_landscape));
                channel_param.Add("screenOrientation", ToScreenOrientation(channel.is_landscape));
                channel_param.Add("use_joymeng_login", ToBoolStr(channel.use_joymeng_login));
                channel_param.Add("platform_auto_login", ToBoolStr(channel.platform_auto_login));
                channel_param.Add("access_platform", channel.access_platform);

                channel_param.Add("create_time", channel.create_time);
                channel_param.Add("replace_app_name", channel.replace_app_name);
                channel_param.Add("remark", channel.remark);
            }
        }

        // 数组的反序列化，返回online_Data数组
        public static List<online_Data> Iteams(string JsonStr)
        {
            try
            {
                List<online_Data> iteams = JsonConvert.DeserializeObject<List<online_Data>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<online_Data>(); }
        }
    }


    /// <summary>
    /// 游戏信息列表
    /// </summary>
    public class gameList_Data
    {
        public string status, msg;

        [JsonIgnore]    // 存储渠道参数映射表
        public Dictionary<String, String> data = new Dictionary<string, string>();


        /// <summary>
        /// 在线获取网游，游戏列表信息
        /// </summary>
        public static Dictionary<String, String> getGameList(Cmd.Callback call = null)
        {
            string url = "http://netunion.joymeng.com/index.php?m=Api&c=PackTool&a=gameList";
            if (call != null) call("【I】 联网获取游戏列表信息...\r\n" + url);

            string str = WebSettings.getWebData(url);
            if (str.Equals("")) 
            { 
                if (call != null) call("【E】 获取游戏列表信息失败！请点击链接查看网络是否正常");
                return new Dictionary<string, string>();
            }
            else
            {
                gameList_Data iteam = gameList_Data.Parse(str);
                if (call != null) call("【I】 获取游戏列表信息完成");

                return iteam.data;
            }
        }

        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // 数组的反序列化，返回gameList_Data数组
        public static String ToJson(List<gameList_Data> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        private static gameList_Data Parse_0(string JsonStr)
        {
            return JsonConvert.DeserializeObject<gameList_Data>(JsonStr);
        }

        //从Json串创建Id对象
        public static gameList_Data Parse(string JsonStr)
        {
            gameList_Data iteam = Parse_0(JsonStr);
            iteam.data = Tools.ParseParams(JsonStr, "data");    // 解析data信息
            return iteam;
        }

        // 数组的反序列化，返回gameList_Data数组
        public static List<gameList_Data> Iteams(string JsonStr)
        {
            try
            {
                List<gameList_Data> iteams = JsonConvert.DeserializeObject<List<gameList_Data>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<gameList_Data>(); }
        }
    }

    /// <summary>
    /// 渠道信息列表
    /// </summary>
    public class channelList_Data
    {
        public string status, msg;

        [JsonIgnore]    // 存储渠道参数映射表
        public Dictionary<String, String> data = new Dictionary<string, string>();


        /// <summary>
        /// 在线获取网游，渠道列表信息
        /// </summary>
        public static Dictionary<String, String> getChannelList(Cmd.Callback call = null)
        {
            string url = "http://netunion.joymeng.com/index.php?m=Api&c=PackTool&a=channelList";
            if (call != null) call("【I】 联网获取渠道列表信息...\r\n" + url);
            string str = WebSettings.getWebData(url);

            if (str.Equals(""))
            {
                if (call != null) call("【E】 联网获取渠道列表信息失败！请点击链接查看网络是否正常");
                return new Dictionary<string, string>();
            }
            else
            {
                channelList_Data iteam = channelList_Data.Parse(str);

                if (call != null) call("【I】 获取渠道列表信息完成");

                return iteam.data;
            }
        }

        //将当前对象Type1的数据，转化为Json串
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // 数组的反序列化，返回channelList_Data数组
        public static String ToJson(List<channelList_Data> Iteams)
        {
            return JsonConvert.SerializeObject(Iteams);
        }

        //从Json串创建Id对象  
        private static channelList_Data Parse_0(string JsonStr)
        {
            return JsonConvert.DeserializeObject<channelList_Data>(JsonStr);
        }

        //从Json串创建Id对象
        public static channelList_Data Parse(string JsonStr)
        {
            channelList_Data iteam = Parse_0(JsonStr);
            iteam.data = Tools.ParseParams(JsonStr, "data");    // 解析data信息
            return iteam;
        }

        // 数组的反序列化，返回channelList_Data数组
        public static List<channelList_Data> Iteams(string JsonStr)
        {
            try
            {
                List<channelList_Data> iteams = JsonConvert.DeserializeObject<List<channelList_Data>>(JsonStr);
                return iteams;
            }
            catch (Exception e) { return new List<channelList_Data>(); }
        }
    }


    /// <summary>
    /// 工具函数类，定义一些通用的功能逻辑
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// 解析JsonStr中的所有keysName = "channel_param" 属性到Dictionary中存储
        /// </summary>
        public static Dictionary<String, String> ParseParams(string JsonStr, string keysName)
        {
            Dictionary<String, String> channel_param = new Dictionary<string, string>();
            
            // 获取渠道所有参数
            if (JsonStr.Contains("\"" + keysName + "\":"))
            {
                int S = JsonStr.IndexOf("\"" + keysName + "\":");
                int S1 = JsonStr.IndexOf("{", S) + 1, S2 = JsonStr.IndexOf("}", S1);
                String innerStr = JsonStr.Substring(S1, S2 - S1);

                // ｛｝匹配解析逻辑
                if (innerStr.Contains("{"))
                {
                    int ConutN = getCount(innerStr, "{");
                    S2 = getNextIndex(JsonStr, "}", ConutN, S1);
                    innerStr = JsonStr.Substring(S1, S2 - S1);
                }

                String[] A = innerStr.Split(',');
                foreach (String param in A)
                {
                    if (!param.Trim().Equals("") && param.Contains(":"))
                    {
                        //if (param.Contains("APPID"))
                        //{
                        //    String tmp = param;
                        //}

                        String[] V = split(param, ":");
                        String append = V[1].Contains("{") ? "VALUE_CONST:" : "";                    // 记录为常量值
                        channel_param.Add(V[0].Trim('"'), append + Format(V[1].Trim('"')).Trim());   // 获取渠道配置参数信息
                    }
                }
            }

            return channel_param;
        }

        /// <summary>
        /// 获取innerStr中第N个seprator的索引位置
        /// </summary>
        private static int getNextIndex(String innerStr, string seprator, int N, int StartIndex = 0)
        {
            int count = 0, index = StartIndex;
            while (innerStr.IndexOf(seprator, index) != -1)
            {
                index = innerStr.IndexOf(seprator, index);
                if (count >= N) break;

                index += seprator.Length;
                count++;
            }
            return index;
        }

        /// <summary>
        /// 获取innerStr中子串seprator数目
        /// </summary>
        private static int getCount(String innerStr, string seprator)
        {
            int count = 0, index = 0;
            while (innerStr.Contains(seprator))
            {
                count++;

                index = innerStr.IndexOf(seprator) + seprator.Length;
                innerStr = innerStr.Substring(index);
            }
            return count;
        }


        /// <summary>
        /// 将data按seprator分割为两个子串
        /// </summary>
        public static string[] split(string data, string seprator)
        {
            if (data.Contains(seprator))
            {
                int S = data.IndexOf(seprator), E = S + seprator.Length;
                string[] A = new string[2];
                A[0] = data.Substring(0, S).Trim();
                A[1] = data.Substring(E).Trim();

                return A;
            }
            else return new string[] { data };
        }


        /// <summary>
        /// 将@"\u5357\u7f8e\u89e3\u653e\u8005\u676f"; 转化为字符串形式
        /// </summary>
        public static String Format(String str)
        {
            if (str.Contains(@"\u")) str = ConvertUnicodeStringToChinese(str);

            // 对url进行校验 "https:\\/\\/ysdktest.qq.com"
            if (str.Contains(@"\") && (str.StartsWith("https:") || str.StartsWith("http:"))) 
                str = str.Replace("\\", "");

            if (str.Contains("\\")) str = str.Replace("\\", "");    // 剔除串中的符号"\"
            
            return str;
        }

        /// <summary>
        /// 将unicode转换为中文
        /// </summary>
        /// <param name="unicodeString">unicode字符串</param>
        /// <returns>unicode解码的字符串</returns>
        public static string ConvertUnicodeStringToChinese(string unicodeString)
        {
            if (string.IsNullOrEmpty(unicodeString))
                return string.Empty;

            string outStr = unicodeString;

            Regex re = new Regex("\\\\u[0123456789abcdef]{4}", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(unicodeString);
            foreach (Match ma in mc)
            {
                outStr = outStr.Replace(ma.Value, ConverUnicodeStringToChar(ma.Value).ToString());
            }
            return outStr;
        }

        private static char ConverUnicodeStringToChar(string str)
        {
            char outStr = Char.MinValue;
            outStr = (char)int.Parse(str.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
            return outStr;
        }
    }

}
